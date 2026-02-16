// Cyotek Color Picker controls library
// Copyright © 2013-2021 Cyotek Ltd.
// http://cyotek.com/blog/tag/colorpicker

// Licensed under the MIT License. See license.txt for the full text.

// If you use this code in your applications, donations or attribution are welcome

namespace Cyotek.Windows.Forms;

public class RgbaColorSlider : ColorSlider
{
  #region Constants

  private static readonly object EventChannelChanged = new();

  private static readonly object EventColorChanged = new();

  #endregion

  #region Fields

  private Brush? _cellBackgroundBrush;

  private RgbaChannel _channel;

  private Color _color;

  #endregion

  #region Constructors

  public RgbaColorSlider()
  {
    base.BarStyle = ColorBarStyle.Custom;
    base.Maximum = 255;
    Color = Color.Black;
    CreateScale();
  }

  #endregion

  #region Events

  [Category("Property Changed")]
  public event EventHandler ChannelChanged
  {
    add => Events.AddHandler(EventChannelChanged, value);
    remove => Events.RemoveHandler(EventChannelChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler ColorChanged
  {
    add => Events.AddHandler(EventColorChanged, value);
    remove => Events.RemoveHandler(EventColorChanged, value);
  }

  #endregion

  #region Properties

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override ColorBarStyle BarStyle
  {
    get => base.BarStyle;
    set => base.BarStyle = value;
  }

  [Category("Appearance")]
  [DefaultValue(typeof(RgbaChannel), "Red")]
  public virtual RgbaChannel Channel
  {
    get => _channel;
    set
    {
      if (Channel != value)
      {
        _channel = value;

        OnChannelChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(typeof(Color), "Black")]
  public virtual Color Color
  {
    get => _color;
    set
    {
      if (Color != value)
      {
        _color = value;

        OnColorChanged(EventArgs.Empty);
      }
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override Color Color1
  {
    get => base.Color1;
    set => base.Color1 = value;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override Color Color2
  {
    get => base.Color2;
    set => base.Color2 = value;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override Color Color3
  {
    get => base.Color3;
    set => base.Color3 = value;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override float Maximum
  {
    get => base.Maximum;
    set => base.Maximum = value;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override float Minimum
  {
    get => base.Minimum;
    set => base.Minimum = value;
  }

  public override float Value
  {
    get => base.Value;
    set => base.Value = (int)value;
  }

  #endregion

  #region Methods

  protected virtual void CreateScale()
  {
    var custom = new ColorCollection();
    var color = Color;
    var channel = Channel;

    for (int i = 0; i < 254; i++)
    {
      int a = color.A;
      int r = color.R;
      int g = color.G;
      int b = color.B;

      switch (channel)
      {
        case RgbaChannel.Red:
          r = i;
          break;
        case RgbaChannel.Green:
          g = i;
          break;
        case RgbaChannel.Blue:
          b = i;
          break;
        case RgbaChannel.Alpha:
          a = i;
          break;
      }

      custom.Add(Color.FromArgb(a, r, g, b));
    }

    CustomColors = custom;
  }

  protected virtual Brush CreateTransparencyBrush()
  {
    return new TextureBrush(ResourceManager.CellBackground, WrapMode.Tile);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (_cellBackgroundBrush != null)
      {
        _cellBackgroundBrush.Dispose();
      }
    }

    base.Dispose(disposing);
  }

  /// <summary>
  /// Raises the <see cref="ChannelChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnChannelChanged(EventArgs e)
  {
    CreateScale();
    Invalidate();

    var handler = (EventHandler)Events[EventChannelChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="ColorChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnColorChanged(EventArgs e)
  {
    CreateScale();
    Invalidate();

    var handler = (EventHandler)Events[EventColorChanged];

    handler?.Invoke(this, e);
  }

  protected override void PaintBar(PaintEventArgs e)
  {
    if (Color.A != 255)
    {
      if (_cellBackgroundBrush == null)
      {
        _cellBackgroundBrush = CreateTransparencyBrush();
      }

      e.Graphics.FillRectangle(_cellBackgroundBrush, BarBounds);
    }

    base.PaintBar(e);
  }

  #endregion
}
