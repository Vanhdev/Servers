using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System
{
    public class RGB
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; } = 255;

        static int nibble(char c)
        {
            if (c >= 'a') return c - 'a' + 10;
            if (c >= 'A') return c - 'A' + 10;

            return c & 15;
        }
        static byte double_nib(char c)
        {
            var v = nibble(c);
            return (byte)((v << 4) | v);
        }
        static byte nib_nib(char h, char l)
        {
            return (byte)((nibble(h) << 4) | nibble(l));
        }

        public RGB() { }
        public RGB(int r, int g, int b)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }
        public RGB(int r, int g, int b, int a)
            : this(r, g, b)
        {
            A = (byte)a;
        }
        public RGB(string text)
        {
            var s = Parse(text);
            A = s.A;
            R = s.R;
            G = s.G;
            B = s.B;
        }
        public static RGB Parse(int value)
        {
            return new RGB(value >> 16, value >> 8, value);
        }
        public static RGB Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            if (text[0] != '#') { return Parse(int.Parse(text)); }
            if (text.Length == 4)
            {
                return new RGB(double_nib(text[1]), double_nib(text[2]), double_nib(text[3]));
            }
            int a = 255, r, g, b;
            if (text.Length == 5)
            {
                a = double_nib(text[1]);
                r = double_nib(text[2]);
                g = double_nib(text[3]);
                b = double_nib(text[4]);
                return new RGB(r, g, b, a);
            }

            int i = 1;
            if (text.Length == 9)
            {
                a = nib_nib(text[i], text[i + 1]);
                i += 2;
            }
            r = nib_nib(text[i], text[i + 1]); i += 2;
            g = nib_nib(text[i], text[i + 1]); i += 2;
            b = nib_nib(text[i], text[i + 1]);
            return new RGB(r, g, b, a);
        }
        public static implicit operator RGB(string text) { return RGB.Parse(text); }
        public static implicit operator Brush(RGB c) 
        {
            if (c == null) return null;
            return new SolidColorBrush(new Color { 
                A = c.A,
                R = c.R,
                G = c.G,
                B = c.B,
            }); 
        }
    }
}
