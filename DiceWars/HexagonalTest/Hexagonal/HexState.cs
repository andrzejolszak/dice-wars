using System.Drawing;

namespace Hexagonal
{
    public class HexState
    {
        private System.Drawing.Color backgroundColor;

        public HexState(Color color)
        {
            this.backgroundColor = color;
        }

        public System.Drawing.Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            internal set
            {
                backgroundColor = value;
            }
        }
    }
}