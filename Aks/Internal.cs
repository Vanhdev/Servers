using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aks
{
    public static class InternalContext
    {
        static string _templatePath;
        static DirectoryInfo _requestFolder;
        static DirectoryInfo _responseFolder;

        static DirectoryInfo GetFolder(string path)
        {
            var di = new DirectoryInfo(path);
            if (!di.Exists) { di.Create(); }

            return di;
        }
        public static void SetTemplatePath(string path)
        {
            _templatePath = path;
            if (path[path.Length - 1] != '\\')
            {
                _templatePath += '\\';
            }

            GetFolder(_templatePath);
            _requestFolder = GetFolder(_templatePath + "Request");

            _responseFolder = GetFolder(_templatePath + "Response");
            foreach (var fi in _responseFolder.GetFiles())
            {
                fi.Delete();
            }
        }

        public static List<DataContext> LoadRequests()
        {
            var lst = new List<DataContext>();
            foreach (var fi in _requestFolder.GetFiles())
            {
                using (var sr = new StreamReader(fi.FullName))
                {
                    var s = sr.ReadToEnd();
                    var context = DataContext.Parse(s);

                    context.Push("#file", GetResponseFileName(fi));
                    lst.Add(context);
                }

                fi.Delete();
            }
            return lst;
        }
        public static string GetResponseFileName(FileInfo fi)
        {
            return _responseFolder.FullName + '\\' + fi.Name;
        }
        public static FileInfo SendRequest(string content)
        {
            var fi = new FileInfo(_requestFolder.FullName + string.Format($"\\{DateTime.Now.Ticks}.json"));
            using (var sw = fi.CreateText())
            {
                sw.Write(content);
            }
            return new FileInfo(GetResponseFileName(fi));
        }

        public static Vst.Context ServerNotFound => new Vst.Context { Code = 503, Message = "Server not found" };
        public static Vst.Context ServerBusy => new Vst.Context { Code = 500, Message = "Server is busy" };
        public static Vst.Context UnknowError = new Vst.Context { Code = 400, Message = "Unknow Error" };
    }
}
