using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemo
{
    class Device
    {
        public byte[] Status { get; set; } = new byte[4];
        string _command { get; set; }
        string[] _args;
        public bool Run(string command)
        {
            var i = command.IndexOf('(');
            var j = command.LastIndexOf(')');

            _command = command.Substring(0, i);
            _args = command.Substring(i + 1, j - i - 1).Split(',');
            
            var method = this.GetType().GetMethod(_command);
            if (method != null)
            {
                method.Invoke(this, new object[] { });
                return true;
            }
            return false;
        }
        public void ClearAlarm()
        {
            Status = new byte[4];
        }
        public void Demo(string name, int index)
        {
            switch (name)
            {
                case "ARM":
                    ClearAlarm();
                    if (index != 0) { index = 3 - index; }
                    Status[0] = (byte)(index << 2);
                    break;

                case "CLA":
                    byte b = Status[0];

                    ClearAlarm();
                    Status[0] = b;
                    break;

                case "INPUT":
                    Status[2] |= (byte)(1 << index);
                    Status[1] |= (byte)(1 << index);
                    break;

                case "ZONE":
                    Status[3] |= (byte)(1 << index);
                    Status[1] |= (byte)(1 << index);
                    break;
            }
        }
        public void ARM()
        {
            Demo(_command, int.Parse(_args[0]));
        }
        public void DISARM() { Demo(_command, 0); }
        public void CLA() { Demo(_command, 0); }
    }
}
