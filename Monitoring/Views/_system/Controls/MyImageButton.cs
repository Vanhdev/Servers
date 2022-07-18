using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public partial class MyImageButton : MyButtonBase
    {
        public string ImageSource { get; set; }
        protected DrawingPen painter;
        public MyImageButton()
        {
            this.MouseMove += (s, e) => {
                InvalidateMeasure();
            };
            this.MouseLeave += (s, e) => {
                InvalidateMeasure();
            };
        }
        protected override Size MeasureOverride(Size constraint)
        {
            if (painter == null)
            {
                painter = this.GetImage(ImageSource, FontSize);
                painter.Thickness = 2;

                painter.ApplyMouseClickAction(() => base.Activate());

                this.Content.Add(painter);
            }
            painter.Stroke = Foreground;
            return base.MeasureOverride(constraint);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //painter.Invalidate();
        }
    }
}
