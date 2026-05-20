using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
  /// <summary>
  /// Inline validation errors with red labels and field border highlighting.
  /// </summary>
  public sealed class ValidationFieldBinder
  {
    private const int ErrorHeight = 18;
    private const int GapAfterError = 8;

    private readonly Panel _panel;
    private readonly List<FieldEntry> _entries = new List<FieldEntry>();

    public ValidationFieldBinder(Panel panel)
    {
      _panel = panel ?? throw new ArgumentNullException(nameof(panel));
    }

    public FieldEntry BindTextBox(TextBox textBox, Func<string, string> validator, Label captionLabel = null)
    {
      if (textBox == null)
      {
        throw new ArgumentNullException(nameof(textBox));
      }

      Label errorLabel = CreateErrorLabel();
      _panel.Controls.Add(errorLabel);

      var entry = new FieldEntry
      {
        InputControl = textBox,
        HostControl = GetFieldHost(textBox),
        CaptionLabel = captionLabel,
        ErrorLabel = errorLabel,
        Validator = validator,
        ReadValue = () => textBox.Text
      };

      _entries.Add(entry);
      textBox.TextChanged += Input_TextChanged;
      return entry;
    }

    public FieldEntry BindComboBox(ComboBox comboBox, Func<string, string> validator, Label captionLabel = null)
    {
      if (comboBox == null)
      {
        throw new ArgumentNullException(nameof(comboBox));
      }

      Label errorLabel = CreateErrorLabel();
      _panel.Controls.Add(errorLabel);

      var entry = new FieldEntry
      {
        InputControl = comboBox,
        HostControl = GetFieldHost(comboBox),
        CaptionLabel = captionLabel,
        ErrorLabel = errorLabel,
        Validator = validator,
        ReadValue = () => comboBox.SelectedItem?.ToString() ?? string.Empty
      };

      _entries.Add(entry);
      comboBox.SelectedIndexChanged += Input_TextChanged;
      comboBox.TextChanged += Input_TextChanged;
      return entry;
    }

    public void BindExisting(TextBox textBox, Label errorLabel, Func<string, string> validator)
    {
      if (textBox == null || errorLabel == null)
      {
        throw new ArgumentNullException();
      }

      var entry = new FieldEntry
      {
        InputControl = textBox,
        HostControl = GetFieldHost(textBox),
        ErrorLabel = errorLabel,
        Validator = validator,
        ReadValue = () => textBox.Text
      };

      _entries.Add(entry);
      textBox.TextChanged += Input_TextChanged;
    }

    public bool ValidateAll()
    {
      bool isValid = true;
      foreach (FieldEntry entry in _entries)
      {
        string error = entry.Validator(entry.ReadValue());
        if (error != null)
        {
          ShowError(entry, error);
          isValid = false;
        }
        else
        {
          ClearError(entry);
        }
      }

      return isValid;
    }

    public void ClearAll()
    {
      foreach (FieldEntry entry in _entries)
      {
        ClearError(entry);
      }
    }

    public void ClearField(Control inputControl)
    {
      FieldEntry entry = _entries.FirstOrDefault(e => e.InputControl == inputControl);
      if (entry != null)
      {
        ClearError(entry);
      }
    }

    public void ReflowSingleColumn(int startY, int left, int width, params Control[] trailingControls)
    {
      int y = startY;
      foreach (FieldEntry entry in _entries)
      {
        y = LayoutFieldGroup(entry, left, width, y);
      }

      if (trailingControls != null)
      {
        y += 8;
        foreach (Control control in trailingControls)
        {
          if (control == null)
          {
            continue;
          }

          control.Left = left;
          control.Top = y;
          control.Width = width;
          y = control.Bottom + 12;
        }
      }

      _panel.Height = Math.Max(_panel.Height, y + _panel.Padding.Bottom + 8);
    }

    public void ReflowTwoColumn(
      int startY,
      int left,
      int columnWidth,
      int columnGap,
      IReadOnlyList<FieldEntry> leftColumn,
      IReadOnlyList<FieldEntry> rightColumn,
      int fullWidth,
      params Control[] trailingControls)
    {
      ReflowTwoColumn(startY, left, columnWidth, columnGap, leftColumn, rightColumn, fullWidth, 8, 6, trailingControls);
    }

    public void ReflowTwoColumn(
      int startY,
      int left,
      int columnWidth,
      int columnGap,
      IReadOnlyList<FieldEntry> leftColumn,
      IReadOnlyList<FieldEntry> rightColumn,
      int fullWidth,
      int fieldGapAfterError,
      int controlSpacing,
      params Control[] trailingControls)
    {
      int rows = Math.Max(leftColumn?.Count ?? 0, rightColumn?.Count ?? 0);
      int y = startY;
      int rightLeft = left + columnWidth + columnGap;

      for (int i = 0; i < rows; i++)
      {
        int rowStart = y;
        int leftBottom = rowStart;
        int rightBottom = rowStart;

        if (leftColumn != null && i < leftColumn.Count)
        {
          leftBottom = LayoutFieldGroup(leftColumn[i], left, columnWidth, rowStart, fieldGapAfterError);
        }

        if (rightColumn != null && i < rightColumn.Count)
        {
          rightBottom = LayoutFieldGroup(rightColumn[i], rightLeft, columnWidth, rowStart, fieldGapAfterError);
        }

        y = Math.Max(leftBottom, rightBottom);
      }

      if (trailingControls != null)
      {
        y += controlSpacing;
        foreach (Control control in trailingControls)
        {
          if (control == null)
          {
            continue;
          }

          control.Left = left;
          control.Top = y;
          control.Width = fullWidth;
          y = control.Bottom + controlSpacing;
        }
      }

      _panel.Height = Math.Max(_panel.Height, y + _panel.Padding.Bottom + 8);
    }

    public IReadOnlyList<FieldEntry> Entries => _entries;

    private static int LayoutFieldGroup(FieldEntry entry, int left, int width, int y)
    {
      return LayoutFieldGroup(entry, left, width, y, GapAfterError);
    }

    private static int LayoutFieldGroup(FieldEntry entry, int left, int width, int y, int gapAfterError)
    {
      if (entry.CaptionLabel != null)
      {
        entry.CaptionLabel.Left = left;
        entry.CaptionLabel.Top = y;
        y = entry.CaptionLabel.Bottom + 3;
      }

      entry.HostControl.Left = left;
      entry.HostControl.Top = y;
      entry.HostControl.Width = width;
      y = entry.HostControl.Bottom + 2;

      entry.ErrorLabel.Left = left;
      entry.ErrorLabel.Top = y;
      entry.ErrorLabel.Width = width;
      y = entry.ErrorLabel.Bottom + gapAfterError;

      return y;
    }

    private void Input_TextChanged(object sender, EventArgs e)
    {
      Control input = sender as Control;
      if (input != null)
      {
        ClearField(input);
      }
    }

    private static Label CreateErrorLabel()
    {
      return new Label
      {
        AutoSize = false,
        BackColor = Color.Transparent,
        ForeColor = ClinicUiTheme.ErrorText,
        Font = ClinicUiTheme.ErrorFont,
        Height = ErrorHeight,
        Visible = false,
        Text = string.Empty
      };
    }

    private static Control GetFieldHost(Control input)
    {
      if (input.Parent is ChromeTextPlate plate)
      {
        return plate;
      }

      return input;
    }

    private static void ShowError(FieldEntry entry, string message)
    {
      entry.ErrorLabel.Text = message;
      entry.ErrorLabel.Visible = true;
      SetInvalidState(entry.HostControl, true);
    }

    private static void ClearError(FieldEntry entry)
    {
      entry.ErrorLabel.Text = string.Empty;
      entry.ErrorLabel.Visible = false;
      SetInvalidState(entry.HostControl, false);
    }

    private static void SetInvalidState(Control host, bool invalid)
    {
      if (host is ChromeTextPlate plate)
      {
        plate.IsInvalid = invalid;
        return;
      }

      if (host is TextBox textBox)
      {
        textBox.BackColor = invalid ? ClinicUiTheme.FieldInvalidFill : ClinicUiTheme.FieldFill;
      }
    }

    public sealed class FieldEntry
    {
      internal Control InputControl { get; set; }
      internal Control HostControl { get; set; }
      internal Label CaptionLabel { get; set; }
      internal Label ErrorLabel { get; set; }
      internal Func<string, string> Validator { get; set; }
      internal Func<string> ReadValue { get; set; }
    }
  }
}
