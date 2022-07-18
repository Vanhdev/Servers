using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public class MyToolbarButton : MyUserControl<StackPanel>
    {

#pragma warning disable CS0067
        public event EventHandler Click;
#pragma warning restore CS0067

        public MyImage Image { get; private set; }
        public MyToolbarButton(MyImage image)
        {
            Image = image;
        }
        public MyToolbarButton(string imgName)
        {
            Image = (MyImage)this.GetImage(imgName);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Image.Parent == null)
            {
                Content.Orientation = Orientation.Horizontal;
                Image.VerticalAlignment = VerticalAlignment.Center;
                
                Content.Children.Insert(0, Image);
                if (Image.Stroke == Brushes.Black)
                {
                    Image.Stroke = this.Foreground;
                }
                this.ApplyMouseClickAction(null);
            }
            Image.Width = Image.Height = FontSize;
            return base.MeasureOverride(constraint);
        }

    }
    public class MyToolBar : MyUserControl<StackPanel>
    {
        public MyToolBar()
        {
            Content.Orientation = Orientation.Horizontal;
        }
        public void Add(UIElement e)
        {
            Content.Children.Add(e);
        }
        public void AddRange(UIElement[] items)
        {
            foreach (var item in items)
            {
                Content.Children.Add(item);
            }
        }
    }
}
