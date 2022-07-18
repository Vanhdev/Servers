using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemo
{
    class ScreenLine : LinkedList<string>
    {
        public int Capacity { get; set; } = 5;
        public ScreenLine Add(string s)
        {
            if (Count == Capacity)
            {
                RemoveFirst();
            }
            AddLast(s);
            return this;
        }
        public override string ToString()
        {
            return string.Join("\n", this);
        }
    }
}
