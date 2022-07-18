using BsonData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Controllers
{
    internal abstract class DataController<T> : BaseController
        where T : Document, new()
    {
        public abstract Collection DataCollection { get; }
        public virtual IEnumerable<T> GetDataList(Func<Document, bool> where, Func<T, bool> check)
        {
            var lst = new List<T>();
            foreach (var doc in DataCollection.Select(where))
            {
                var item = doc.ToObject<T>();
                if (check?.Invoke(item) == false) { continue; }
                
                lst.Add(item);
            }
            return lst;
        }
        public virtual object Index()
        {
            var newContext = new T();
            ViewData["new-context"] = newContext;
            ViewData["key-name"] = typeof(BsonData.ObjectId).Name;

            return View(GetDataList(null, null));
        }
        public object Add()
        {
            return FormView(new T());
        }

        public object Edit(string id)
        {
            var obj = DataCollection.Find<T>(id);
            return FormView(obj);
        }
        public virtual object Update(EditingContext context)
        {
            DataCollection.InsertOrUpdate((Document)context.Value);
            return GoFirst();
        }
        protected virtual void DeleteObject(string id)
        {
            DataCollection.FindAndDelete(id, context => {
            
            });
        }
        public virtual object Delete(EditingContext context)
        {
            foreach (var id in context.Value.Keys)
            {
                DeleteObject(id);
            }
            return null;
        }
    }
}
