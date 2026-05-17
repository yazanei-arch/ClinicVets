using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Attaches a compact Show/Hide toggle to a password textbox inside a chrome plate.</summary>
    internal static class PasswordVisibilityHelper
    {
        private const int ToggleRightPadding = 10;

        internal static LinkLabel Attach(TextBox passwordBox)
        {
            if (passwordBox == null)
            {
                return null;
            }

            passwordBox.UseSystemPasswordChar = true;

            var toggle = new LinkLabel
            {
                Text = "Show",
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                LinkColor = ClinicUiTheme.AccentBlue,
                ActiveLinkColor = ClinicUiTheme.AccentBlueHover,
                VisitedLinkColor = ClinicUiTheme.AccentBlue,
                LinkBehavior = LinkBehavior.HoverUnderline,
                Font = new Font("Segoe UI", 8.75f, FontStyle.Regular),
                TabStop = false
            };

            bool isVisible = false;
            toggle.LinkClicked += (sender, args) =>
            {
                isVisible = !isVisible;
                passwordBox.UseSystemPasswordChar = !isVisible;
                toggle.Text = isVisible ? "Hide" : "Show";
            };

            if (passwordBox.Parent is ChromeTextPlate plate)
            {
                plate.Controls.Add(toggle);
                plate.Resize += (sender, args) => LayoutToggle(plate, passwordBox, toggle);
                LayoutToggle(plate, passwordBox, toggle);
            }
            else
            {
                Control parent = passwordBox.Parent;
                if (parent != null)
                {
                    parent.Controls.Add(toggle);
                    toggle.BringToFront();
                    toggle.Location = new Point(
                        passwordBox.Right - toggle.PreferredSize.Width - 6,
                        passwordBox.Top + (passwordBox.Height - toggle.Height) / 2);
                }
            }

            return toggle;
        }

        private static void LayoutToggle(ChromeTextPlate plate, TextBox passwordBox, LinkLabel toggle)
        {
            toggle.AutoSize = true;
            int toggleWidth = toggle.PreferredWidth + ToggleRightPadding;
            toggle.Location = new Point(
                Math.Max(plate.Padding.Left, plate.ClientSize.Width - toggleWidth),
                Math.Max(0, (plate.ClientSize.Height - toggle.Height) / 2));

            passwordBox.Location = new Point(plate.Padding.Left + 2, plate.Padding.Top + 2);
            passwordBox.Width = Math.Max(10, plate.ClientSize.Width - toggleWidth - plate.Padding.Horizontal - 4);
            passwordBox.Height = Math.Max(10, plate.ClientSize.Height - plate.Padding.Vertical - 3);
        }
    }
}
