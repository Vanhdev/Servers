using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Bitwise
    {
        public int Value { get; set; } = 0;
        public bool this[int index]
        {
            get => (Value & (1 << index)) != 0;
            set
            {
                int v = Value;
                if (value)
                {
                    v |= (1 << index);
                }
                else
                {
                    v &= ~(1 << index);
                }
                if (v != Value)
                {
                    Value = v;
                }
            }
        }
        public static bool operator ==(Bitwise s, int flags)
        {
            return (flags == 0 && s.Value == 0) || (s.Value & flags) != 0;
        }
        public static bool operator !=(Bitwise s, int flags)
        {
            return (flags == 0 && s.Value != 0) || (s.Value & flags) == 0;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        static public implicit operator Bitwise(int value)
        {
            return new Bitwise { Value = value };
        }

        static public int Hex2Dec(char c)
        {
            if (c >= 'a') return c - 'a' + 10;
            if (c >= 'A') return c - 'A' + 10;
            return c & 15;
        }
        static public byte[] Hex2Bytes(string s)
        {
            var bytes = new byte[s.Length >> 1];
            for (int i = 0, k = 0; i < bytes.Length; i++)
            {
                var h = Hex2Dec(s[k++]);
                var l = Hex2Dec(s[k++]);

                bytes[i] = (byte)((h << 4) | l);
            }
            return bytes;
        }
    }
}
