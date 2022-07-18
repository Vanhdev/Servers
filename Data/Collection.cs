using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BsonData
{

    public class Collection
    {
        enum UpdatingState { None, Deleted, Changed, Inserted };
        Dictionary<string, UpdatingState> _updating = new Dictionary<string, UpdatingState>();

        void _set_updating(string id, UpdatingState value)
        {
            _updating.Remove(id);
            _updating.Add(id, value);
        }
        UpdatingState _get_updating(string id)
        {
            UpdatingState s;
            _updating.TryGetValue(id, out s);
            
            return s;
        }

        public Database Database { get; private set; }
        public string Name { get; private set; }
        public Collection(string name, Database db)
        {
            Database = db;
            Name = name;

            BeginRead();
        }

        #region LIST
        public bool IsBusy => _stroreThread != null && _stroreThread.IsAlive;
        int _count;
        public int Count 
        {
            get
            {
                Wait(null);
                return _count;
            }
        }
        class Node
        {
            public string Value { get; set; }
            public Node Next { get; set; }
            public Node Prev { get; set; }
        }
        Node _head, _tail;
        void _add(Document doc)
        {
            var node = new Node { Value = doc.ObjectId };
            if (_count++ == 0)
            {
                _head = node;
            }
            else
            {
                node.Prev = _tail;
                _tail.Next = node;
            }

            _tail = node;
        }
        void _remove(Node node)
        {
            var next = node.Next;
            var prev = node.Prev;

            if (next != null) next.Prev = prev;
            if (prev != null) prev.Next = next;

            if (node == _head) _head = next;
            if (node == _tail) _tail = prev;

            _count--;
        }
        void _load()
        {
            var s = Database.CollectionStorage.GetSubStorage(this.Name);
            foreach (var e in s.ReadAll())
            {
                _add(e);
                Database.Add(e);
            }
        }

        Thread _stroreThread;
        void _store()
        {
            if (_updating.Count > 0)
            {
                var s = Database.CollectionStorage.GetSubStorage(this.Name);
                foreach (var p in _updating)
                {
                    switch (p.Value)
                    {
                        case UpdatingState.Changed:
                        case UpdatingState.Inserted:
                            s.Write(Database[p.Key]);
                            break;

                        case UpdatingState.Deleted:
                            s.Delete(p.Key);
                            break;

                    }
                }
                _updating.Clear();
            }
        }

        public void BeginRead()
        {
            try
            {
                _stroreThread = new Thread(() => _load());
                _stroreThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void BeginWrite()
        {
            try
            {
                _stroreThread = new Thread(() => _store());
                _stroreThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region FINDING
        public Document Find(string objectId, Action<Document> callback)
        {
            Document doc = Database[objectId];
            if (doc != null)
            {
                callback?.Invoke(doc);
            }
            return doc;
        }
        public T Find<T>(string objectId) 
            where T: Document, new()
        {
            var doc = Database[objectId];
            if (doc == null) { return null; }

            if (doc.GetType() == typeof(T)) { return (T)doc; }

            var context = new T();
            context.Copy(doc);

            Database[objectId] = context;

            return context;
        }
        public void FindAndDelete(string objectId, Action<Document> before)
        {
            Find(objectId, doc => {
                before?.Invoke(doc);

                Database.Remove(objectId);

                _set_updating(objectId, UpdatingState.Deleted);
            });
        }
        public void FindAndUpdate(string objectId, Action<Document> before)
        {
            Find(objectId, doc => {
                before?.Invoke(doc);
                _set_updating(objectId, UpdatingState.Changed);
            });
        }
        #endregion

        #region DB
        public void Wait(Action callback)
        {
            while (IsBusy) { }
            callback?.Invoke();
        }
        public IEnumerable<Document> Select(Func<Document, bool> where)
        {
            var lst = Select();
            if (where != null)
            {
                lst = lst.Where(where);
            }
            return lst;
        }
        public IEnumerable<Document> Select()
        {
            var lst = new List<Document>();
            Wait(() => {
                var node = _head;
                while (node != null)
                {
                    var next = node.Next;
                    var documentId = node.Value;

                    if (_get_updating(documentId)  == UpdatingState.Deleted)
                    {
                        _remove(node);
                    }
                    else
                    {
                        var doc = Database[documentId];
                        if (doc == null)
                        {
                            _remove(node);
                        }
                        else
                        {
                            lst.Add(doc);
                        }
                    }
                    node = next;
                }
            });
            return lst;
        }
        public void Insert(string id, Document doc)
        {
            doc.ObjectId = id;
            Insert(doc);
        }
        public bool Insert(Document doc)
        {
            var id = doc.ObjectId;
            if (Database.ContainsKey(id)) { return false; }

            Database[id] = doc;

            _add(doc);
            _set_updating(id, UpdatingState.Inserted);

            return true;
        }
        public bool Update(Document doc)
        {
            var id = doc.ObjectId;
            var res = false;

            FindAndUpdate(id, current =>
            {
                res = true;
                if (doc != current)
                {
                    foreach (var p in doc)
                    {
                        current.Push(p.Key, p.Value);
                    }
                }
            });

            return res;
        }
        public bool Delete(Document doc)
        {
            var res = false;
            FindAndDelete(doc.ObjectId, exist => res = true);

            return res;
        }
        public void InsertOrUpdate(Document doc)
        {
            var id = doc.ObjectId;
            Document old = Database[id];
            if (old != null)
            {
                foreach (var p in doc)
                {
                    old.Push(p.Key, p.Value);
                }
            }
            else
            {
                Database.Add(id, doc);
                _add(doc);
            }
            _set_updating(id, UpdatingState.Changed);
        }
        #endregion
    }
}
