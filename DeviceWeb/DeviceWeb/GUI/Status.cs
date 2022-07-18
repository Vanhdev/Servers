using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public partial class Status
    {
        int val = 0;
        public event Action Changed;
        public bool this[int index]
        {
            get => (val & (1 << index)) != 0;
            set
            {
                int v = val;
                if (value)
                {
                    v |= (1 << index);
                }
                else
                {
                    v &= ~(1 << index);
                }
                if (v != val)
                {
                    val = v;
                    Changed?.Invoke();
                }
            }
        }
        public void Clear()
        {
            if (val != 0)
            {
                val = 0;
                Changed?.Invoke();
            }
        }
        public static bool operator ==(Status s, int flags)
        {
            return (flags == 0 && s.val == 0) || (s.val & flags) != 0;
        }
        public static bool operator !=(Status s, int flags)
        {
            return (flags == 0 && s.val != 0) || (s.val & flags) == 0;
        }
        public static implicit operator Status(int value)
        {
            return new Status { val = value };
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return val.Equals(obj);
        }
    }
}
