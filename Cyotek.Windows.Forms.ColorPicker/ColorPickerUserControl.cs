// Cyotek Color Picker controls library
// Copyright © 2013-2017 Cyotek Ltd.
// http://cyotek.com/blog/tag/colorpicker

// Licensed under the MIT License. See license.txt for the full text.

// If you use this code in your applications, donations or attribution are welcome

namespace Cyotek.Windows.Forms;

[DefaultEvent("PreviewColorChanged")]
[DefaultProperty("Color")]
public partial class ColorPickerUserControl : UserControl
{
  #region Constants

  private static readonly object EventPreviewColorChanged = new object();

  #endregion

  #region Fields

  private Brush? _textureBrush;

  #endregion

  #region Constructors

  public ColorPickerUserControl()
  {
    ShowAlphaChannel = true;
    Font = SystemFonts.DialogFont;
  }

  #endregion

  #region Events

  [Category("Property Changed")]
  public event EventHandler PreviewColorChanged
  {
    add => Events.AddHandler(EventPreviewColorChanged, value);
    remove => Events.RemoveHandler(EventPreviewColorChanged, value);
  }

  #endregion

  #region Properties

  [DefaultValue(typeof(Color), "Black")]
  public Color Color
  {
    get => colorEditorManager.Color;
    set => colorEditorManager.Color = value;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public bool ShowAlphaChannel { get; set; }

  #endregion

  #region Methods

  /// <summary>
  /// Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (_textureBrush != null)
      {
        _textureBrush.Dispose();
        _textureBrush = null;
      }
    }

    base.Dispose(disposing);
  }

  /// <summary>
  /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
  /// </summary>
  /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    colorEditor.ShowAlphaChannel = ShowAlphaChannel;

    if (!ShowAlphaChannel)
    {
      for (int i = 0; i < colorGrid.Colors.Count; i++)
      {
        var color = colorGrid.Colors[i];
        if (color.A != 255)
        {
          colorGrid.Colors[i] = Color.FromArgb(255, color);
        }
      }
    }
  }

  /// <summary>
  /// Raises the <see cref="PreviewColorChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnPreviewColorChanged(EventArgs e)
  {
    var handler = (EventHandler)Events[EventPreviewColorChanged];

    handler?.Invoke(this, e);
  }

  private void cancelButton_Click(object sender, EventArgs e)
  {
    //this.DialogResult = DialogResult.Cancel;
    //this.Close();
  }

  private void colorEditorManager_ColorChanged(object sender, EventArgs e)
  {
    previewPanel.Invalidate();

    OnPreviewColorChanged(e);
  }

  private void colorGrid_EditingColor(object sender, EditColorCancelEventArgs e)
  {
    e.Cancel = true;

    using (ColorDialog dialog = new ColorDialog())
    {
      dialog.FullOpen = true;
      dialog.Color = e.Color;
      if (dialog.ShowDialog(this) == DialogResult.OK)
      {
        colorGrid.Colors[e.ColorIndex] = dialog.Color;
      }
    }
  }

  private void loadPaletteButton_Click(object sender, EventArgs e)
  {
    using (FileDialog dialog = new OpenFileDialog())
    {
      dialog.Filter = PaletteSerializer.DefaultOpenFilter;
      dialog.DefaultExt = "pal";
      dialog.Title = @"Open Palette File";

      if (dialog.ShowDialog(this) == DialogResult.OK)
      {
        try
        {
          var serializer = PaletteSerializer.GetSerializer(dialog.FileName);
          if (serializer != null)
          {
            ColorCollection? palette;

            if (!serializer.CanRead)
            {
              throw new InvalidOperationException("Serializer does not support reading palettes.");
            }

            using (FileStream file = File.OpenRead(dialog.FileName))
            {
              palette = serializer.Deserialize(file);
            }

            if (palette != null)
            {
              // we can only display 96 colors in the color grid due to it's size, so if there's more, bin them
              while (palette.Count > 96)
              {
                palette.RemoveAt(palette.Count - 1);
              }

              // or if we have less, fill in the blanks
              while (palette.Count < 96)
              {
                palette.Add(Color.White);
              }

              colorGrid.Colors = palette;
            }
          }
          else
          {
            MessageBox.Show(@"Sorry, unable to open palette, the file format is not supported or is not recognized.", @"Load Palette", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show(@$"Sorry, unable to open palette. {ex.GetBaseException().Message}", @"Load Palette", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }
  }

  private void okButton_Click(object sender, EventArgs e)
  {
    //this.DialogResult = DialogResult.OK;
    //this.Close();
  }

  private void previewPanel_Paint(object sender, PaintEventArgs e)
  {
    var region = previewPanel.ClientRectangle;

    if (Color.A != 255)
    {
      if (_textureBrush == null)
      {
        using (Bitmap background = new Bitmap(GetType().Assembly.GetManifestResourceStream(string.Concat(GetType().Namespace, ".Resources.cellbackground.png"))!))
        {
          _textureBrush = new TextureBrush(background, WrapMode.Tile);
        }
      }

      e.Graphics.FillRectangle(_textureBrush, region);
    }

    using (Brush brush = new SolidBrush(Color))
    {
      e.Graphics.FillRectangle(brush, region);
    }

    e.Graphics.DrawRectangle(SystemPens.ControlText, region.Left, region.Top, region.Width - 1, region.Height - 1);
  }

  private void savePaletteButton_Click(object sender, EventArgs e)
  {
    using (FileDialog dialog = new SaveFileDialog())
    {
      dialog.Filter = PaletteSerializer.DefaultSaveFilter;
      dialog.DefaultExt = "pal";
      dialog.Title = @"Save Palette File As";
      if (dialog.ShowDialog(this) == DialogResult.OK)
      {
        var serializer = PaletteSerializer.AllSerializers.Where(s => s.CanWrite).ElementAt(dialog.FilterIndex - 1);
        if (serializer != null)
        {
          if (!serializer.CanWrite)
          {
            throw new InvalidOperationException("Serializer does not support writing palettes.");
          }

          try
          {
            using (FileStream file = File.OpenWrite(dialog.FileName))
            {
              serializer.Serialize(file, colorGrid.Colors);
            }
          }
          catch (Exception ex)
          {
            MessageBox.Show(@$"Sorry, unable to save palette. {ex.GetBaseException().Message}", @"Save Palette", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
        else
        {
          MessageBox.Show(@"Sorry, unable to save palette, the file format is not supported or is not recognized.", @"Save Palette", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
      }
    }
  }

  #endregion
}
