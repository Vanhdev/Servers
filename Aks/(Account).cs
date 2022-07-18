using BsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vst;

namespace Aks
{
    public partial class Account
    {
        static BsonDataMap<Type> _actorMap;
        static BsonDataMap<User> _users = new BsonDataMap<User>();
        static public void LoadActorConfig(string path)
        {
            Console.WriteLine("Load actor config: " + path);

            _actorMap = new BsonDataMap<Type>();
            _actorMap.Add("account", typeof(Account));
            _actorMap.Add("admin", typeof(Admin));

            try
            {
                foreach (var fi in new System.IO.DirectoryInfo(path).GetFiles())
                {
                    var dll = Assembly.LoadFile(fi.Name + "Actor" + ".dll");
                    foreach (var type in dll.GetTypes())
                    {
                        if (type.FullName.Contains("Actor"))
                        {
                            _actorMap[type.Name] = type;
                        }
                    }
                }
            }
            catch
            {
            }
        }
        static public User CreateActor(Account acc)
        {
            var time = DateTime.Now;
            var token = acc.UserName.JoinMD5(time.Ticks);

            var type = _actorMap[acc.Role];
            if (type == null)
            {
                type = typeof(User);
            }

            acc.Push("#token", token);
            var user = Activator.CreateInstance(type) as User;
            user.Copy(acc);

            user.Push("#login-time", time);
            

            _users[token] = user;
            return user;
        }
        static public User FindActorByToken(string token)
        {
            return _users[token];
        }

        static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        static string GetMethodKey(Type type, string name) => type.Name + '/' + name;
        public MethodInfo FindMethod(string name, params Type[] args)
        {
            name = name.ToLower();
            var type = this.GetType();
            string key = GetMethodKey(type, name);

            MethodInfo info = null;
            if (_methods.TryGetValue(key, out info))
            {
                return info;
            }

            foreach (var method in type.GetMethods())
            {
                if (method.Name.ToLower() == name)
                {
                    var param = method.GetParameters();
                    if (param.Length == args.Length)
                    {
                        int i = 0;
                        for (; i < args.Length; i++)
                        {
                            if (param[i].ParameterType != args[i])
                            {
                                break;
                            }
                        }
                        if (i == param.Length)
                        {
                            _methods.Add(key, method);
                            return method;
                        }
                    }
                }
            }
            return null;
        }
    }
}

namespace Aks
{
    public partial class Account : Document
    {
        #region Attributes
        public string UserName { get => GetString(DB.UserName); set => Push(DB.UserName, value); }
        public string Password { get => GetString(DB.Password); set => Push(DB.Password, value); }
        public string Role { get => GetString(DB.Role); set => Push(DB.Role, value); }
        public string Avatar => Profile.GetString("Avatar");
        DataContext _profile;
        public DataContext Profile
        {
            get
            {
                if (_profile == null)
                {
                    _profile = GetObject<DataContext>("Profile") ?? new DataContext();
                }
                return _profile;
            }
            set
            {
                _profile = value;
                DB.Accounts.FindAndUpdate(UserName,
                    acc => acc.SetObject("Profile", value));
            }
        }
        public string LastLogin
        {
            get => GetString("LastLogin");
            set => SetString("LastLogin", value);
        }
        #endregion

        #region API RESPONSE
        protected Context CreateApiResponse(int code, string message, object value)
        {
            var context = new Context
            {
                Code = code,
                Message = message,
                Value = value,
            };
            return context;
        }
        public virtual Context Ok(object value)
        {
            return CreateApiResponse(0, null, value);
        }
        public virtual Context Ok()
        {
            return Ok(null);
        }
        public virtual Context Error(int code, string message)
        {
            return CreateApiResponse(code, message, null);
        }
        #endregion
        public object UpdateProfile(DataContext context)
        {
            Profile = context;
            return Ok();
        }
        public object Login(DataContext context)
        {
            var acc = context.ChangeType<Account>();
            int code = acc.TryLogin();

            if (code != 0)
            {
                return Error(code, null);
            }
            var user = CreateActor(acc);
            return Ok(user);
        }
        int TryLogin()
        {
            var un = UserName?.ToLower();
            if (un == null) { return 400; }

            var acc = DB.Accounts.Find(un, null);
            if (acc == null) { return -1; }

            var ps = un.JoinMD5(Password);
            if (acc.GetString(DB.Password) != ps) { return -2; }

            this.Copy(acc);
            this.Remove(DB.Password);

            return 0;
        }
        public object ChangePassword(DataContext context)
        {
            var ps = context.GetString("NewPass");
            var co = context.GetString("Confirm");
            if (ps != co)
            {
                return Error(-2, "Not match");
            }

            var un = UserName.ToLower();
            var acc = DB.Accounts.Find(un, null);

            var current = un.JoinMD5(context.GetString(DB.Password));
            if (acc.GetString(DB.Password) != current)
            {
                return Error(-1, "Password invalid");
            }
            acc.SetString(DB.Password, un.JoinMD5(ps));

            DB.Accounts.Update(acc);
            return Ok();
        }
        public object Logout(DataContext context)
        {
            var token = context.GetString("#token");
            if (token != null)
            {
                _users.Remove(token);
            }
            return Ok();
        }

        public static int CreateAccount(string userName, string password, string role)
        {
            return CreateAccount(new Account
            {
                UserName = userName,
                Password = password,
                Role = role,
            });
        }
        public static int CreateAccount(DataContext context)
        {
            var acc = context.ChangeType<Account>();
            var userName = acc.UserName.ToLower();
            var password = acc.Password;
            var role = acc.Role;

            var db = DB.Accounts;

            if (_actorMap[role] == null)
            {
                return -2;
            }

            int code = -1;
            if (db.Find(userName, null) == null)
            {
                if (password == null)
                {
                    password = userName;
                }

                acc.UserName = userName;
                acc.Password = userName.JoinMD5(password);

                DB.Accounts.Insert(userName, acc);
                code = 0;
            }
            return code;
        }
        public object Execute(string name, DataContext context)
        {
            return this.FindMethod(name, typeof(DataContext))?
                .Invoke(this, new object[] { context });
        }
    }
    partial class DB
    {
        public const string Admin = "Admin";
        public const string Partner = "Partner";
        public const string Customer = "Customer";

        public const string UserName = "UserName";
        public const string Password = "Password";
        public const string Role = "Role";

        static Collection _accounts;
        public static Collection Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    _accounts = GetCollection<Account>();
                }
                return _accounts;
            }
        }
    }
}
