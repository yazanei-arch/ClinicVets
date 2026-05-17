using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    internal static class ThemeHelper
    {
        internal static ClinicHeaderBar ApplyThemedShell(
            Form form,
            CardPanel contentCard = null,
            bool showCardPaws = false,
            bool centerCardVertically = false,
            string headerSubtitle = null,
            FormBackgroundStyle backgroundStyle = FormBackgroundStyle.RegisterFocus,
            bool showHeader = true)
        {
            if (form == null)
            {
                return null;
            }

            WinFormsUi.SetDoubleBuffered(form);
            form.Font = ClinicUiTheme.BaseFont;
            VetBackgroundHelper.ApplyPremiumBackground(form);

            EnsureAmbientBackground(form, backgroundStyle);
            ClinicHeaderBar header = showHeader ? EnsureHeader(form, headerSubtitle) : null;

            if (contentCard != null)
            {
                StyleContentCard(contentCard, showCardPaws);
            }

            form.Resize += (sender, args) => RelayoutThemedCard(form, contentCard, centerCardVertically);
            RelayoutThemedCard(form, contentCard, centerCardVertically);

            if (contentCard != null)
            {
                contentCard.BringToFront();
            }

            if (header != null)
            {
                header.BringToFront();
            }

            FormTransitionHelper.FadeIn(form);
            return header;
        }

        private static void RelayoutThemedCard(Form form, CardPanel card, bool centerCardVertically)
        {
            if (card == null)
            {
                return;
            }

            FormBackgroundStyle style = GetBackgroundStyle(form);

            if (centerCardVertically && style == FormBackgroundStyle.LoginHero)
            {
                LayoutLoginHeroCard(form, card);
            }
            else if (centerCardVertically)
            {
                CenterCardInClient(form, card);
            }
            else
            {
                LayoutContentBelowHeader(form, card, style);
            }
        }

        private static FormBackgroundStyle GetBackgroundStyle(Form form)
        {
            AmbientBackgroundPanel ambient = form.Controls.OfType<AmbientBackgroundPanel>().FirstOrDefault();
            return ambient?.Style ?? FormBackgroundStyle.RegisterFocus;
        }

        private static void LayoutLoginHeroCard(Form form, CardPanel card)
        {
            int splitX = (int)(form.ClientSize.Width * 0.48f);
            int top = GetContentTop(form);
            int available = Math.Max(0, form.ClientSize.Height - top - 16);
            card.Left = Math.Max(28, (splitX - card.Width) / 2);
            card.Top = top + Math.Max(0, (available - card.Height) / 2);
        }

        internal static void StyleContentCard(CardPanel card, bool showDecor = false)
        {
            if (card == null)
            {
                return;
            }

            card.CornerRadius = ClinicUiTheme.PanelRadius;
            card.ShowCornerDecorations = showDecor;
            card.Padding = new Padding(
                ClinicUiTheme.ContentPadding,
                24,
                ClinicUiTheme.ContentPadding,
                28);
        }

        internal static void ApplyStandardLabels(
            Label title,
            Label subtitle,
            params Label[] fieldLabels)
        {
            if (title != null)
            {
                if (title.Font.Size >= 20f)
                {
                    ClinicUiTheme.ApplyPageTitleLabel(title);
                }
                else
                {
                    ClinicUiTheme.ApplyTitleLabel(title);
                }
            }

            ClinicUiTheme.ApplySubtitleLabel(subtitle);

            if (fieldLabels != null)
            {
                foreach (Label label in fieldLabels.Where(l => l != null))
                {
                    ClinicUiTheme.ApplyFieldLabel(label);
                }
            }
        }

        internal static void ApplyStandardButtons(params RoundedActionButton[] buttons)
        {
            if (buttons == null)
            {
                return;
            }

            foreach (RoundedActionButton button in buttons.Where(b => b != null))
            {
                if (button.IsOutlineStyle)
                {
                    ClinicUiTheme.ApplyOutlineButton(button);
                }
                else
                {
                    ClinicUiTheme.ApplyPrimaryButton(button);
                }
            }
        }

        internal static void ApplyErrorLabels(params Label[] labels)
        {
            if (labels == null)
            {
                return;
            }

            foreach (Label label in labels.Where(l => l != null))
            {
                ClinicUiTheme.ApplyErrorLabel(label);
            }
        }

        internal static void CenterCardInClient(Form form, CardPanel card, int verticalBias = 0)
        {
            if (form == null || card == null)
            {
                return;
            }

            int top = GetContentTop(form);
            int available = Math.Max(0, form.ClientSize.Height - top - 16);
            card.Left = Math.Max(12, (form.ClientSize.Width - card.Width) / 2);
            card.Top = top + Math.Max(0, (available - card.Height) / 2) + verticalBias;
        }

        internal static void LayoutContentBelowHeader(Form form, CardPanel card)
        {
            LayoutContentBelowHeader(form, card, GetBackgroundStyle(form));
        }

        private static void LayoutContentBelowHeader(Form form, CardPanel card, FormBackgroundStyle style)
        {
            if (form == null || card == null)
            {
                return;
            }

            int top = GetContentTop(form);
            int left = Math.Max(12, (form.ClientSize.Width - card.Width) / 2);

            if (style == FormBackgroundStyle.DashboardWorkspace)
            {
                left = Math.Max(ClinicUiTheme.SidebarDecorWidth + 16, left);
            }

            card.Left = left;
            card.Top = top + 8;
        }

        internal static int GetHeaderBottom(Form form)
        {
            return GetContentTop(form);
        }

        private static int GetContentTop(Form form)
        {
            ClinicHeaderBar header = form.Controls.OfType<ClinicHeaderBar>().FirstOrDefault();
            return header != null ? header.Bottom : 0;
        }

        private static ClinicHeaderBar EnsureHeader(Form form, string subtitle)
        {
            ClinicHeaderBar existing = form.Controls.OfType<ClinicHeaderBar>().FirstOrDefault();
            if (existing != null)
            {
                if (!string.IsNullOrWhiteSpace(subtitle))
                {
                    existing.SubtitleText = subtitle;
                }

                existing.ShowUserChip = SessionManager.CurrentUser != null;
                existing.BringToFront();
                return existing;
            }

            var header = new ClinicHeaderBar();
            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                header.SubtitleText = subtitle;
            }

            header.ShowUserChip = SessionManager.CurrentUser != null;
            form.Controls.Add(header);
            header.BringToFront();
            return header;
        }

        private static void EnsureAmbientBackground(Form form, FormBackgroundStyle style)
        {
            AmbientBackgroundPanel ambient = form.Controls.OfType<AmbientBackgroundPanel>().FirstOrDefault();
            if (ambient == null)
            {
                ambient = new AmbientBackgroundPanel(style) { Dock = DockStyle.Fill };
                form.Controls.Add(ambient);
                ambient.SendToBack();
            }
            else
            {
                ambient.Style = style;
            }

            form.Resize += (sender, args) => ambient.Invalidate();
        }

        private sealed class AmbientBackgroundPanel : Panel
        {
            internal FormBackgroundStyle Style { get; set; }

            internal AmbientBackgroundPanel(FormBackgroundStyle style)
            {
                Style = style;
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint,
                    true);
                WinFormsUi.SetDoubleBuffered(this);
                TabStop = false;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Image hero = null;
                ClinicDecorations.PaintFormBackground(e.Graphics, ClientRectangle, Style, hero);
            }
        }
    }
}
