using System.Drawing;
using System.Windows.Forms;

namespace BackTranslationHelper
{
    /// <summary>
    /// A Button that, when it has a ContextMenuStrip assigned, paints a small drop-down arrow
    /// on its right edge and pops that menu immediately when the arrow region is clicked -
    /// mimicking the split-button affordance seen in many modern apps (e.g. Word's Paste
    /// button). Clicking anywhere else on the button still raises the normal Click event.
    /// Right-click still shows the ContextMenuStrip anywhere on the button, as any Control
    /// with a ContextMenuStrip does by default.
    /// </summary>
    public class SplitButton : Button
    {
        private const int ArrowRegionWidth = 20;

        private Rectangle ArrowRectangle =>
            new Rectangle(ClientRectangle.Width - ArrowRegionWidth, 0, ArrowRegionWidth, ClientRectangle.Height);

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // reserve the arrow region on the right so the button centers its text over the
            // label area only, not over the label-plus-arrow width
            var desiredPadding = ContextMenuStrip != null ? new Padding(0, 0, ArrowRegionWidth, 0) : Padding.Empty;
            if (Padding != desiredPadding)
                Padding = desiredPadding;

            base.OnPaint(pevent);

            if (ContextMenuStrip == null)
                return; // nothing to hint at

            var arrowRect = ArrowRectangle;

            // separator line between the label and the arrow
            using (var pen = new Pen(SystemColors.ControlDark))
            {
                pevent.Graphics.DrawLine(pen, arrowRect.Left, 4, arrowRect.Left, Height - 5);
            }

            // small down-arrow glyph, centered in the arrow region
            var color = Enabled ? SystemColors.ControlText : SystemColors.GrayText;
            var cx = arrowRect.Left + arrowRect.Width / 2;
            var cy = arrowRect.Top + arrowRect.Height / 2;
            var glyph = new[]
            {
                new Point(cx - 4, cy - 2),
                new Point(cx + 4, cy - 2),
                new Point(cx, cy + 3)
            };
            using (var brush = new SolidBrush(color))
            {
                pevent.Graphics.FillPolygon(brush, glyph);
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left && ContextMenuStrip != null && ArrowRectangle.Contains(mevent.Location))
            {
                // pop the menu right where a split-button's drop-down would, and swallow the
                // click so it doesn't also fire the button's normal Click handler
                ContextMenuStrip.Show(this, new Point(0, Height));
                return;
            }

            base.OnMouseDown(mevent);
        }
    }
}
