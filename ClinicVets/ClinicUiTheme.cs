using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Unified premium dark veterinary dashboard palette.</summary>
    internal static class ClinicUiTheme
    {
        // Text
        internal static readonly Color TitleText = Color.FromArgb(232, 244, 255);
        internal static readonly Color BodyText = Color.FromArgb(148, 163, 184);
        internal static readonly Color LabelAccent = Color.FromArgb(103, 232, 249);
        internal static readonly Color MutedText = Color.FromArgb(100, 116, 139);

        // Accents
        internal static readonly Color AccentCyan = Color.FromArgb(34, 211, 238);
        internal static readonly Color AccentBlue = Color.FromArgb(56, 189, 248);
        internal static readonly Color AccentBlueHover = Color.FromArgb(125, 211, 252);
        internal static readonly Color NeonLine = Color.FromArgb(56, 189, 248);
        internal static readonly Color GlowCyan = Color.FromArgb(64, 34, 211, 238);

        // Page backgrounds
        internal static readonly Color BgTop = Color.FromArgb(10, 18, 36);
        internal static readonly Color BgMid = Color.FromArgb(14, 26, 48);
        internal static readonly Color BgBottom = Color.FromArgb(8, 20, 40);
        internal static readonly Color SidebarFill = Color.FromArgb(28, 45, 72);
        internal static readonly Color SidebarGlow = Color.FromArgb(48, 56, 189, 248);

        // Orbs
        internal static readonly Color OrbCyan = Color.FromArgb(55, 34, 211, 238);
        internal static readonly Color OrbBlue = Color.FromArgb(45, 37, 99, 235);
        internal static readonly Color OrbViolet = Color.FromArgb(40, 99, 102, 241);

        // Cards (dark glass)
        internal static readonly Color GlassFill = Color.FromArgb(210, 22, 36, 58);
        internal static readonly Color SolidCardFill = Color.FromArgb(255, 18, 30, 50);
        internal static readonly Color GlassBorder = Color.FromArgb(90, 56, 189, 248);
        internal static readonly Color GlassHighlight = Color.FromArgb(50, 103, 232, 249);
        internal static readonly Color CardShadow = Color.FromArgb(50, 0, 0, 0);
        internal static readonly Color DecorPaw = Color.FromArgb(35, 56, 189, 248);

        // Header
        internal static readonly Color HeaderGlassTop = Color.FromArgb(220, 16, 28, 46);
        internal static readonly Color HeaderGlassBottom = Color.FromArgb(200, 12, 22, 40);
        internal static readonly Color HeaderTitle = Color.FromArgb(224, 242, 254);
        internal static readonly Color HeaderSubtitle = Color.FromArgb(148, 163, 184);
        internal static readonly Color HeaderUserChip = Color.FromArgb(180, 30, 58, 92);
        internal static readonly Color HeaderUserText = Color.FromArgb(186, 230, 253);

        // Fields
        internal static readonly Color FieldFill = Color.FromArgb(245, 18, 30, 50);
        internal static readonly Color FieldInvalidFill = Color.FromArgb(245, 40, 28, 36);
        internal static readonly Color FieldBorder = Color.FromArgb(80, 51, 65, 85);
        internal static readonly Color FieldBorderFocus = Color.FromArgb(180, 34, 211, 238);
        internal static readonly Color FieldBorderError = Color.FromArgb(200, 248, 113, 113);
        internal static readonly Color FieldFocusGlow = Color.FromArgb(70, 34, 211, 238);
        internal static readonly Color ErrorText = Color.FromArgb(252, 165, 165);

        // Buttons
        internal static readonly Color ButtonPrimaryStart = Color.FromArgb(14, 165, 233);
        internal static readonly Color ButtonPrimaryEnd = Color.FromArgb(6, 182, 212);
        internal static readonly Color ButtonPrimaryPress = Color.FromArgb(2, 132, 199);
        internal static readonly Color ButtonGlow = Color.FromArgb(90, 34, 211, 238);
        internal static readonly Color ButtonOutlineFill = Color.FromArgb(30, 22, 36, 58);
        internal static readonly Color ButtonOutlineHover = Color.FromArgb(60, 30, 58, 92);

        // Nav / menu cards
        internal static readonly Color NavFill = Color.FromArgb(200, 22, 36, 58);
        internal static readonly Color NavFillHover = Color.FromArgb(230, 30, 58, 92);
        internal static readonly Color NavBorder = Color.FromArgb(100, 56, 189, 248);

        // Grid
        internal static readonly Color GridHeaderBack = Color.FromArgb(28, 45, 72);
        internal static readonly Color GridHeaderFore = Color.FromArgb(186, 230, 253);
        internal static readonly Color GridLine = Color.FromArgb(45, 51, 65, 85);
        internal static readonly Color GridRow = Color.FromArgb(22, 34, 54);
        internal static readonly Color GridRowAlt = Color.FromArgb(18, 30, 48);
        internal static readonly Color GridSelectionBack = Color.FromArgb(50, 14, 116, 144);
        internal static readonly Color GridSelectionFore = Color.FromArgb(224, 242, 254);
        internal static readonly Color GridContainerFill = Color.FromArgb(16, 26, 44);

        // Radii & layout
        internal const int PanelRadius = 16;
        internal const int FieldRadius = 10;
        internal const int ButtonRadius = 10;
        internal const int NavCardRadius = 14;
        internal const int HeaderHeight = 68;
        internal const int ButtonHeight = 44;
        internal const int FieldHeight = 36;
        internal const int ContentPadding = 40;
        internal const int SectionGap = 12;
        internal const int SidebarDecorWidth = 200;

        // Typography
        internal static readonly Font BaseFont = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Point);
        internal static readonly Font TitleFont = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point);
        internal static readonly Font PageTitleFont = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
        internal static readonly Font SubtitleFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        internal static readonly Font LabelFont = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
        internal static readonly Font ButtonFont = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
        internal static readonly Font ErrorFont = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        internal static readonly Font HeaderTitleFont = new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point);
        internal static readonly Font HeaderSubtitleFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        internal static readonly Font HeaderUserFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        // Legacy aliases used by some controls
        internal static readonly Color TitleBlue = TitleText;
        internal static readonly Color LabelBlue = LabelAccent;

        internal static void ApplyDataGridView(DataGridView grid)
        {
            if (grid == null)
            {
                return;
            }

            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = GridLine;
            grid.BackgroundColor = GridContainerFill;
            grid.EnableHeadersVisualStyles = false;
            grid.Font = BaseFont;

            grid.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderBack;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = GridHeaderFore;
            grid.ColumnHeadersDefaultCellStyle.Font = LabelFont;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = GridHeaderBack;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = GridHeaderFore;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 38;

            grid.DefaultCellStyle.BackColor = GridRow;
            grid.DefaultCellStyle.ForeColor = TitleText;
            grid.DefaultCellStyle.SelectionBackColor = GridSelectionBack;
            grid.DefaultCellStyle.SelectionForeColor = GridSelectionFore;

            grid.AlternatingRowsDefaultCellStyle.BackColor = GridRowAlt;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = TitleText;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridSelectionBack;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = GridSelectionFore;

            grid.RowTemplate.Height = 34;
        }

        internal static void ApplyTitleLabel(Label label)
        {
            if (label == null)
            {
                return;
            }

            label.Font = TitleFont;
            label.ForeColor = TitleText;
            label.BackColor = Color.Transparent;
        }

        internal static void ApplyPageTitleLabel(Label label)
        {
            if (label == null)
            {
                return;
            }

            label.Font = PageTitleFont;
            label.ForeColor = TitleText;
            label.BackColor = Color.Transparent;
        }

        internal static void ApplySubtitleLabel(Label label)
        {
            if (label == null)
            {
                return;
            }

            label.Font = SubtitleFont;
            label.ForeColor = BodyText;
            label.BackColor = Color.Transparent;
        }

        internal static void ApplyFieldLabel(Label label)
        {
            if (label == null)
            {
                return;
            }

            label.Font = LabelFont;
            label.ForeColor = LabelAccent;
            label.BackColor = Color.Transparent;
        }

        internal static void ApplyErrorLabel(Label label)
        {
            if (label == null)
            {
                return;
            }

            label.Font = ErrorFont;
            label.ForeColor = ErrorText;
            label.BackColor = Color.Transparent;
        }

        internal static void ApplyPrimaryButton(RoundedActionButton button)
        {
            if (button == null)
            {
                return;
            }

            button.IsOutlineStyle = false;
            button.Font = ButtonFont;
            button.CornerRadius = ButtonRadius;
            button.Height = ButtonHeight;
            button.FlatAppearance.BorderSize = 0;
        }

        internal static void ApplyOutlineButton(RoundedActionButton button)
        {
            if (button == null)
            {
                return;
            }

            button.IsOutlineStyle = true;
            button.Font = ButtonFont;
            button.CornerRadius = ButtonRadius;
            button.Height = ButtonHeight;
            button.FlatAppearance.BorderSize = 0;
        }
    }
}
