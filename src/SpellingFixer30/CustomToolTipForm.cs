using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpellingFixer30
{
    internal class CustomToolTipForm : Form
    {
        private Label label;

        public CustomToolTipForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.LightYellow;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            label = new Label
            {
                AutoSize = true,
                BackColor = Color.LightYellow,
                Padding = new Padding(4),
            };
            Controls.Add(label);

            // Hide tooltip when clicked anywhere on the form or label
            this.MouseDown += (s, e) => HideToolTip();
            label.MouseDown += (s, e) => HideToolTip();
        }

        public void ShowToolTip(string text, Font font, Point location)
        {
            label.Text = text;
            label.Font = font;
            Location = location;
            // Optionally, adjust size and position here
            Show();
            BringToFront();
        }

        public void HideToolTip()
        {
            Hide();
        }
    }
}