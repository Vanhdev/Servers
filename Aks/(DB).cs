using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BsonData;

namespace Aks
{
    public static class DataExtension
    {
        public static string GetDataKey(this string s)
        {
            return s.ToUpper();
        }
    }
    public partial class DB
    {
        public const int DeleteAll = -2;
        public const int Delete = -1;
        public const int Update = 0;
        public const int Insert = 1;

        static public Database Main { get; set; }
        static public Collection GetCollection<T>() 
        { 
            return AsyncGetCollection<T>(100); 
        }
        static public Collection AsyncGetCollection<T>(int wait)
        {
            System.Threading.Thread.Sleep(wait);
            return Main.GetCollection(typeof(T).Name);
        }
    }
}