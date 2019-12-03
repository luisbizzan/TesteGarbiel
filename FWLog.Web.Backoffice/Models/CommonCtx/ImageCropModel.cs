using System.Drawing;

namespace FWLog.Web.Backoffice.Models.CommonCtx
{
    public class ImageCropModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle Rectangle { get => new Rectangle(X, Y, Width, Height); }
    }
}