// Cyotek Color Picker Controls Library
// http://cyotek.com/blog/tag/colorpicker

// Copyright (c) 2013-2021 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.Windows.Forms;

/// <summary>
/// Represents a grid control, which displays a collection of colors using different styles.
/// </summary>
[DefaultProperty("Color")]
[DefaultEvent("ColorChanged")]
[ToolboxBitmap(typeof(ColorGrid), "ColorGridToolboxBitmap.bmp")]
[ToolboxItem(true)]
public class ColorGrid : Control, IColorEditor
{
  #region Constants

  public const int INVALID_INDEX = -1;

  private readonly IDictionary<int, Rectangle> _colorRegions;

  private static readonly object EventAutoAddColorsChanged = new();

  private static readonly object EventAutoFitChanged = new();

  private static readonly object EventCellBorderColorChanged = new();

  private static readonly object EventCellBorderStyleChanged = new();

  private static readonly object EventCellContextMenuStripChanged = new();

  private static readonly object EventCellSizeChanged = new();

  private static readonly object EventColorChanged = new();

  private static readonly object EventColorIndexChanged = new();

  private static readonly object EventColorsChanged = new();

  private static readonly object EventColumnsChanged = new();

  private static readonly object EventCustomColorsChanged = new();

  private static readonly object EventEditingColor = new();

  private static readonly object EventEditModeChanged = new();

  private static readonly object EventHotIndexChanged = new();

  private static readonly object EventPaletteChanged = new();

  private static readonly object EventSelectedCellStyleChanged = new();

  private static readonly object EventShowCustomColorsChanged = new();

  private static readonly object EventShowToolTipsChanged = new();

  private static readonly object EventSpacingChanged = new();

  #endregion

  #region Fields

  private bool _autoAddColors;

  private bool _autoFit;

  private Brush _cellBackgroundBrush;

  private Color _cellBorderColor;

  private ColorCellBorderStyle _cellBorderStyle;

  private ContextMenuStrip _cellContextMenuStrip;

  private Size _cellSize;

  private Color _color;

  private int _colorIndex;

  private ColorCollection _colors;

  private int _columns;

  private ColorCollection _customColors;

  private ColorEditingMode _editMode;

  private bool _layoutRequired;

  private int _hotIndex;

  private ColorPalette _palette;

  private int _previousColorIndex;

  private int _previousHotIndex;

  private ColorGridSelectedCellStyle _selectedCellStyle;

  private bool _showCustomColors;

  private bool _showToolTips;

  private Size _spacing;

  private ToolTip _toolTip;

  private int _updateCount;

  #endregion

  #region Constructors

  public ColorGrid()
  {
    SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick | ControlStyles.SupportsTransparentBackColor, true);
    _previousHotIndex = INVALID_INDEX;
    _previousColorIndex = INVALID_INDEX;
    _hotIndex = INVALID_INDEX;

    _colorRegions = new Dictionary<int, Rectangle>();
    _colors = ColorPalettes.NamedColors;
    _customColors = new ColorCollection(Enumerable.Repeat(Color.White, 16));
    _showCustomColors = true;
    _cellSize = new Size(12, 12);
    _spacing = new Size(3, 3);
    _columns = 16;
    base.AutoSize = true;
    Padding = new Padding(5);
    _autoAddColors = true;
    _cellBorderColor = SystemColors.ButtonShadow;
    _showToolTips = true;
    _toolTip = new ToolTip();
    SeparatorHeight = 8;
    _editMode = ColorEditingMode.CustomOnly;
    _color = Color.Black;
    _cellBorderStyle = ColorCellBorderStyle.FixedSingle;
    _selectedCellStyle = ColorGridSelectedCellStyle.Zoomed;
    _palette = ColorPalette.Named;

    AddEventHandlers(_colors);
    AddEventHandlers(_customColors);

    SetScaledCellSize();
    RefreshColors();
  }

  #endregion

  #region Events

  [Category("Property Changed")]
  public event EventHandler AutoAddColorsChanged
  {
    add => Events.AddHandler(EventAutoAddColorsChanged, value);
    remove => Events.RemoveHandler(EventAutoAddColorsChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler AutoFitChanged
  {
    add => Events.AddHandler(EventAutoFitChanged, value);
    remove => Events.RemoveHandler(EventAutoFitChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler CellBorderColorChanged
  {
    add => Events.AddHandler(EventCellBorderColorChanged, value);
    remove => Events.RemoveHandler(EventCellBorderColorChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler CellBorderStyleChanged
  {
    add => Events.AddHandler(EventCellBorderStyleChanged, value);
    remove => Events.RemoveHandler(EventCellBorderStyleChanged, value);
  }

  /// <summary>
  /// Occurs when the CellContextMenuStrip property value changes
  /// </summary>
  [Category("Property Changed")]
  public event EventHandler CellContextMenuStripChanged
  {
    add => Events.AddHandler(EventCellContextMenuStripChanged, value);
    remove => Events.RemoveHandler(EventCellContextMenuStripChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler CellSizeChanged
  {
    add => Events.AddHandler(EventCellSizeChanged, value);
    remove => Events.RemoveHandler(EventCellSizeChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler ColorIndexChanged
  {
    add => Events.AddHandler(EventColorIndexChanged, value);
    remove => Events.RemoveHandler(EventColorIndexChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler ColorsChanged
  {
    add => Events.AddHandler(EventColorsChanged, value);
    remove => Events.RemoveHandler(EventColorsChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler ColumnsChanged
  {
    add => Events.AddHandler(EventColumnsChanged, value);
    remove => Events.RemoveHandler(EventColumnsChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler CustomColorsChanged
  {
    add => Events.AddHandler(EventCustomColorsChanged, value);
    remove => Events.RemoveHandler(EventCustomColorsChanged, value);
  }

  [Category("Action")]
  public event EventHandler<EditColorCancelEventArgs> EditingColor
  {
    add => Events.AddHandler(EventEditingColor, value);
    remove => Events.RemoveHandler(EventEditingColor, value);
  }

  [Category("Property Changed")]
  public event EventHandler EditModeChanged
  {
    add => Events.AddHandler(EventEditModeChanged, value);
    remove => Events.RemoveHandler(EventEditModeChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler HotIndexChanged
  {
    add => Events.AddHandler(EventHotIndexChanged, value);
    remove => Events.RemoveHandler(EventHotIndexChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler PaletteChanged
  {
    add => Events.AddHandler(EventPaletteChanged, value);
    remove => Events.RemoveHandler(EventPaletteChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler SelectedCellStyleChanged
  {
    add => Events.AddHandler(EventSelectedCellStyleChanged, value);
    remove => Events.RemoveHandler(EventSelectedCellStyleChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler ShowCustomColorsChanged
  {
    add => Events.AddHandler(EventShowCustomColorsChanged, value);
    remove => Events.RemoveHandler(EventShowCustomColorsChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler ShowToolTipsChanged
  {
    add => Events.AddHandler(EventShowToolTipsChanged, value);
    remove => Events.RemoveHandler(EventShowToolTipsChanged, value);
  }

  [Category("Property Changed")]
  public event EventHandler SpacingChanged
  {
    add => Events.AddHandler(EventSpacingChanged, value);
    remove => Events.RemoveHandler(EventSpacingChanged, value);
  }

  #endregion

  #region Properties

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int ActualColumns { get; protected set; }

  [Category("Behavior")]
  [DefaultValue(true)]
  public virtual bool AutoAddColors
  {
    get => _autoAddColors;
    set
    {
      if (AutoAddColors != value)
      {
        _autoAddColors = value;

        OnAutoAddColorsChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(false)]
  public virtual bool AutoFit
  {
    get => _autoFit;
    set
    {
      if (AutoFit != value)
      {
        _autoFit = value;

        OnAutoFitChanged(EventArgs.Empty);
      }
    }
  }

  [Browsable(true)]
  [DefaultValue(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
  public override bool AutoSize
  {
    get => base.AutoSize;
    set => base.AutoSize = value;
  }

  [Category("Appearance")]
  [DefaultValue(typeof(Color), "ButtonShadow")]
  public virtual Color CellBorderColor
  {
    get => _cellBorderColor;
    set
    {
      if (CellBorderColor != value)
      {
        _cellBorderColor = value;

        OnCellBorderColorChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(typeof(ColorCellBorderStyle), "FixedSingle")]
  public virtual ColorCellBorderStyle CellBorderStyle
  {
    get => _cellBorderStyle;
    set
    {
      if (CellBorderStyle != value)
      {
        _cellBorderStyle = value;

        OnCellBorderStyleChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Behavior")]
  [DefaultValue(typeof(ContextMenuStrip), null!)]
  public ContextMenuStrip CellContextMenuStrip
  {
    get => _cellContextMenuStrip;
    set
    {
      if (_cellContextMenuStrip != value)
      {
        _cellContextMenuStrip = value;

        OnCellContextMenuStripChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(typeof(Size), "12, 12")]
  public virtual Size CellSize
  {
    get => _cellSize;
    set
    {
      if (_cellSize != value)
      {
        _cellSize = value;

        OnCellSizeChanged(EventArgs.Empty);
      }
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public virtual int ColorIndex
  {
    get => _colorIndex;
    set
    {
      if (ColorIndex != value)
      {
        _previousColorIndex = _colorIndex;
        _colorIndex = value;

        if (value != INVALID_INDEX)
        {
          Color = GetColor(value);
        }

        OnColorIndexChanged(EventArgs.Empty);
      }
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public virtual ColorCollection? Colors
  {
    get => _colors;
    set
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }

      if (Colors != value)
      {
        RemoveEventHandlers(_colors);

        _colors = value;

        OnColorsChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(16)]
  public virtual int Columns
  {
    get => _columns;
    set
    {
      if (value < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(value), value, "Number of columns cannot be less than zero.");
      }

      if (Columns != value)
      {
        _columns = value;
        CalculateGridSize();

        OnColumnsChanged(EventArgs.Empty);
      }
    }
  }

  [Browsable(false)]
  public Point CurrentCell => GetCell(ColorIndex);

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public virtual ColorCollection CustomColors
  {
    get => _customColors;
    set
    {
      if (CustomColors != value)
      {
        RemoveEventHandlers(_customColors);

        _customColors = value;

        OnCustomColorsChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Behavior")]
  [DefaultValue(typeof(ColorEditingMode), "CustomOnly")]
  public virtual ColorEditingMode EditMode
  {
    get => _editMode;
    set
    {
      if (EditMode != value)
      {
        _editMode = value;

        OnEditModeChanged(EventArgs.Empty);
      }
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override Font Font
  {
    get => base.Font;
    set => base.Font = value;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override Color ForeColor
  {
    get => base.ForeColor;
    set => base.ForeColor = value;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public virtual int HotIndex
  {
    get => _hotIndex;
    set
    {
      if (HotIndex != value)
      {
        _previousHotIndex = HotIndex;
        _hotIndex = value;

        OnHotIndexChanged(EventArgs.Empty);
      }
    }
  }

  [DefaultValue(typeof(Padding), "5, 5, 5, 5")]
  public new Padding Padding
  {
    get => base.Padding;
    set => base.Padding = value;
  }

  [Category("Appearance")]
  [DefaultValue(typeof(ColorPalette), "Named")]
  public virtual ColorPalette Palette
  {
    get => _palette;
    set
    {
      if (Palette != value)
      {
        _palette = value;

        OnPaletteChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(typeof(ColorGridSelectedCellStyle), "Zoomed")]
  public virtual ColorGridSelectedCellStyle SelectedCellStyle
  {
    get => _selectedCellStyle;
    set
    {
      if (SelectedCellStyle != value)
      {
        _selectedCellStyle = value;

        OnSelectedCellStyleChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(true)]
  public virtual bool ShowCustomColors
  {
    get => _showCustomColors;
    set
    {
      if (ShowCustomColors != value)
      {
        _showCustomColors = value;

        OnShowCustomColorsChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Behavior")]
  [DefaultValue(true)]
  public virtual bool ShowToolTips
  {
    get => _showToolTips;
    set
    {
      if (ShowToolTips != value)
      {
        _showToolTips = value;

        OnShowToolTipsChanged(EventArgs.Empty);
      }
    }
  }

  [Category("Appearance")]
  [DefaultValue(typeof(Size), "3, 3")]
  public virtual Size Spacing
  {
    get => _spacing;
    set
    {
      if (Spacing != value)
      {
        _spacing = value;

        OnSpacingChanged(EventArgs.Empty);
      }
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override string Text
  {
    get => base.Text;
    set => base.Text = value;
  }

  /// <summary>
  ///   Gets a value indicating whether painting of the control is allowed.
  /// </summary>
  /// <value>
  ///   <c>true</c> if painting of the control is allowed; otherwise, <c>false</c>.
  /// </value>
  protected virtual bool AllowPainting => _updateCount == 0;

  protected IDictionary<int, Rectangle> ColorRegions => _colorRegions;

  protected int CustomRows { get; set; }

  protected int PrimaryRows { get; set; }

  protected int SeparatorHeight { get; set; }

  protected bool WasKeyPressed { get; set; }

  #endregion

  #region Methods

  public virtual int AddCustomColor(Color value)
  {
    int newIndex;

    newIndex = GetColorIndex(value);

    if (newIndex == INVALID_INDEX)
    {
      if (AutoAddColors)
      {
        CustomColors.Add(value);
      }
      else
      {
        if (CustomColors == null)
        {
          CustomColors = new ColorCollection();
          CustomColors.Add(value);
        }
        else
        {
          CustomColors[0] = value;
        }

        newIndex = GetColorIndex(value);
      }

      if (_showCustomColors)
      {
        RefreshColors();
      }
    }

    return newIndex;
  }

  /// <summary>
  ///   Disables any redrawing of the image box
  /// </summary>
  public virtual void BeginUpdate()
  {
    _updateCount++;
  }

  /// <summary>
  ///   Enables the redrawing of the image box
  /// </summary>
  public virtual void EndUpdate()
  {
    if (_updateCount > 0)
    {
      _updateCount--;
    }

    if (AllowPainting)
    {
      if (_layoutRequired)
      {
        RefreshColors();
        _layoutRequired = false;
      }
      else
      {
        Invalidate();
      }
    }
  }

  /// <summary>
  /// Returns the <see cref="Rectangle"/> describing the bounds of a single color cell
  /// </summary>
  /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
  /// required range.</exception>
  /// <param name="index">Zero-based index of the color cell to return.</param>
  /// <returns>
  /// The cell bounds.
  /// </returns>
  public Rectangle GetCellBounds(int index)
  {
    if (index < 0 || index > Colors.Count + CustomColors.Count - 1)
    {
      throw new ArgumentOutOfRangeException(nameof(index));
    }

    return _colorRegions[index];
  }

  public Color GetColor(int index)
  {
    Color result;
    int colorCount;
    int customColorCount;

    colorCount = Colors != null ? Colors.Count : 0;
    customColorCount = CustomColors != null ? CustomColors.Count : 0;

    if (index < 0 || index > colorCount + customColorCount)
    {
      result = Color.Empty;
    }
    else
    {
      result = index > colorCount - 1 ? CustomColors![index - colorCount] : Colors![index];
    }

    return result;
  }

  public ColorSource GetColorSource(int colorIndex)
  {
    ColorSource result;
    int colorCount;
    int customColorCount;

    colorCount = Colors != null ? Colors.Count : 0;
    customColorCount = CustomColors != null ? CustomColors.Count : 0;

    if (colorCount < 0 || colorIndex > colorCount + customColorCount)
    {
      result = ColorSource.None;
    }
    else
    {
      result = colorIndex > colorCount - 1 ? ColorSource.Custom : ColorSource.Standard;
    }

    return result;
  }

  public ColorSource GetColorSource(Color color)
  {
    int index;
    ColorSource result;

    index = Colors.IndexOf(color);
    if (index != INVALID_INDEX)
    {
      result = ColorSource.Standard;
    }
    else
    {
      index = CustomColors.IndexOf(color);
      result = index != INVALID_INDEX ? ColorSource.Custom : ColorSource.None;
    }

    return result;
  }

  public override Size GetPreferredSize(Size proposedSize)
  {
    return AutoSize ? GetAutoSize() : base.GetPreferredSize(proposedSize);
  }

  public ColorHitTestInfo HitTest(Point point)
  {
    ColorHitTestInfo result;
    int colorIndex;

    result = new ColorHitTestInfo();
    colorIndex = INVALID_INDEX;

    foreach (KeyValuePair<int, Rectangle> pair in _colorRegions.Where(pair => pair.Value.Contains(point)))
    {
      colorIndex = pair.Key;
      break;
    }

    result.Index = colorIndex;
    if (colorIndex != INVALID_INDEX)
    {
      result.Color = colorIndex < Colors.Count + CustomColors.Count ? GetColor(colorIndex) : Color.White;
      result.Source = GetColorSource(colorIndex);
    }
    else
    {
      result.Source = ColorSource.None;
    }

    return result;
  }

  public void Invalidate(int index)
  {
    if (AllowPainting && index != INVALID_INDEX)
    {
      Rectangle bounds;

      if (_colorRegions.TryGetValue(index, out bounds))
      {
        if (SelectedCellStyle == ColorGridSelectedCellStyle.Zoomed)
        {
          bounds.Inflate(Padding.Left, Padding.Top);
        }

        Invalidate(bounds);
      }
    }
  }

  public void Navigate(int offsetX, int offsetY)
  {
    Navigate(offsetX, offsetY, NavigationOrigin.Current);
  }

  public virtual void Navigate(int offsetX, int offsetY, NavigationOrigin origin)
  {
    Point cellLocation;
    Point offsetCellLocation;
    int row;
    int column;
    int index;

    switch (origin)
    {
      case NavigationOrigin.Begin:
        cellLocation = Point.Empty;
        break;
      case NavigationOrigin.End:
        cellLocation = new Point(ActualColumns - 1, PrimaryRows + CustomRows - 1);
        break;
      default:
        cellLocation = CurrentCell;
        break;
    }

    if (cellLocation.X == -1 && cellLocation.Y == -1)
    {
      cellLocation = Point.Empty; // If no cell is selected, assume the first one is for the purpose of keyboard navigation
    }

    offsetCellLocation = GetCellOffset(cellLocation, offsetX, offsetY);
    row = offsetCellLocation.Y;
    column = offsetCellLocation.X;
    index = GetCellIndex(column, row);
    if (index != INVALID_INDEX)
    {
      ColorIndex = index;
    }
  }

  protected virtual void CalculateCellSize()
  {
    int w;
    int h;

    w = (ClientSize.Width - Padding.Horizontal) / ActualColumns - Spacing.Width;
    h = (ClientSize.Height - Padding.Vertical) / (PrimaryRows + CustomRows) - Spacing.Height;

    if (w > 0 && h > 0)
    {
      CellSize = new Size(w, h);
    }
  }

  protected virtual void CalculateGridSize()
  {
    int primaryRows;
    int customRows;

    ActualColumns = Columns != 0 ? Columns : (ClientSize.Width + Spacing.Width - Padding.Vertical) / (_scaledCellSize.Width + Spacing.Width);
    if (ActualColumns < 1)
    {
      ActualColumns = 1;
    }

    primaryRows = GetRows(Colors != null ? Colors.Count : 0);
    if (primaryRows == 0)
    {
      primaryRows = 1;
    }

    customRows = ShowCustomColors ? GetRows(CustomColors != null ? CustomColors.Count : 0) : 0;

    PrimaryRows = primaryRows;
    CustomRows = customRows;
  }

  protected virtual Brush CreateTransparencyBrush()
  {
    return new TextureBrush(ResourceManager.CellBackground, WrapMode.Tile);
  }

  protected void DefineColorRegions(ColorCollection colors, int rangeStart, int offset)
  {
    if (colors != null)
    {
      int rows;
      int index;

      rows = GetRows(colors.Count);
      index = 0;

      for (int row = 0; row < rows; row++)
      {
        for (int column = 0; column < ActualColumns; column++)
        {
          if (index < colors.Count)
          {
            _colorRegions.Add(rangeStart + index, new Rectangle(Padding.Left + column * (_scaledCellSize.Width + Spacing.Width), offset + row * (_scaledCellSize.Height + Spacing.Height), _scaledCellSize.Width, _scaledCellSize.Height));
          }

          index++;
        }
      }
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      RemoveEventHandlers(_colors);
      RemoveEventHandlers(_customColors);

      _toolTip.Dispose();

      _cellBackgroundBrush?.Dispose();
    }

    base.Dispose(disposing);
  }

  protected virtual void EditColor(int colorIndex)
  {
    using (ColorPickerDialog dialog = new ColorPickerDialog())
    {
      dialog.Color = GetColor(colorIndex);
      if (dialog.ShowDialog(this) == DialogResult.OK)
      {
        BeginUpdate();
        SetColor(colorIndex, dialog.Color);
        Color = dialog.Color;
        EndUpdate();
      }
    }
  }

  protected Size GetAutoSize()
  {
    int offset;
    int width;

    offset = CustomRows != 0 ? SeparatorHeight : 0;
    if (Columns != 0)
    {
      width = (_scaledCellSize.Width + Spacing.Width) * ActualColumns + Padding.Horizontal - Spacing.Width;
    }
    else
    {
      width = ClientSize.Width;
    }

    return new Size(width, (_scaledCellSize.Height + Spacing.Height) * (PrimaryRows + CustomRows) + offset + Padding.Vertical - Spacing.Height);
  }

  protected int GetCellIndex(Point point)
  {
    return GetCellIndex(point.X, point.Y);
  }

  protected virtual int GetCellIndex(int column, int row)
  {
    int result;

    if (column >= 0 && column < ActualColumns && row >= 0 && row < PrimaryRows + CustomRows)
    {
      int lastStandardRowOffset;

      lastStandardRowOffset = PrimaryRows * ActualColumns - Colors.Count;
      result = row * ActualColumns + column;
      if (row == PrimaryRows - 1 && column >= ActualColumns - lastStandardRowOffset)
      {
        result -= lastStandardRowOffset;
      }
      if (row >= PrimaryRows)
      {
        result -= lastStandardRowOffset;
      }

      if (result > Colors.Count + CustomColors.Count - 1)
      {
        result = INVALID_INDEX;
      }
    }
    else
    {
      result = INVALID_INDEX;
    }

    return result;
  }

  protected Point GetCellOffset(int columnOffset, int rowOffset)
  {
    return GetCellOffset(CurrentCell, columnOffset, rowOffset);
  }

  protected Point GetCellOffset(Point cell, int columnOffset, int rowOffset)
  {
    int row;
    int column;
    int lastStandardRowOffset;
    int lastStandardRowLastColumn;

    lastStandardRowOffset = PrimaryRows * ActualColumns - Colors.Count;
    lastStandardRowLastColumn = ActualColumns - lastStandardRowOffset;
    column = cell.X + columnOffset;
    row = cell.Y + rowOffset;

    // if the row is the last row, but there aren't enough columns to fill the row - nudge it to the last available
    if (row == PrimaryRows - 1 && column >= lastStandardRowLastColumn)
    {
      column = lastStandardRowLastColumn - 1;
    }

    // wrap the column to the end of the previous row
    if (column < 0)
    {
      column = ActualColumns - 1;
      row--;
      if (row == PrimaryRows - 1)
      {
        column = ActualColumns - (lastStandardRowOffset + 1);
      }
    }

    // wrap to column to the start of the next row
    if (row == PrimaryRows - 1 && column >= ActualColumns - lastStandardRowOffset || column >= ActualColumns)
    {
      column = 0;
      row++;
    }

    return new Point(column, row);
  }

  protected virtual int GetColorIndex(Color value)
  {
    int index;

    index = Colors != null ? Colors.IndexOf(value) : INVALID_INDEX;
    if (index == INVALID_INDEX && ShowCustomColors && CustomColors != null)
    {
      index = CustomColors.IndexOf(value);
      if (index != INVALID_INDEX)
      {
        index += Colors.Count;
      }
    }

    return index;
  }

  protected virtual ColorCollection GetPredefinedPalette()
  {
    return ColorPalettes.GetPalette(Palette);
  }

  protected int GetRows(int count)
  {
    int rows;

    if (count != 0 && ActualColumns > 0)
    {
      rows = count / ActualColumns;
      if (count % ActualColumns != 0)
      {
        rows++;
      }
    }
    else
    {
      rows = 0;
    }

    return rows;
  }

  protected override bool IsInputKey(Keys keyData)
  {
    bool result;

    if (keyData == Keys.Left || keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Right || keyData == Keys.Enter || keyData == Keys.Home || keyData == Keys.End)
    {
      result = true;
    }
    else
    {
      result = base.IsInputKey(keyData);
    }

    return result;
  }

  /// <summary>
  /// Raises the <see cref="AutoAddColorsChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnAutoAddColorsChanged(EventArgs e)
  {
    EventHandler handler;

    handler = (EventHandler)Events[EventAutoAddColorsChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="AutoFitChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnAutoFitChanged(EventArgs e)
  {
    EventHandler handler;

    if (AutoFit && AutoSize)
    {
      AutoSize = false;
    }

    RefreshColors();

    handler = (EventHandler)Events[EventAutoFitChanged];

    handler?.Invoke(this, e);
  }

  protected override void OnAutoSizeChanged(EventArgs e)
  {
    if (AutoSize && AutoFit)
    {
      AutoFit = false;
    }

    base.OnAutoSizeChanged(e);

    if (AutoSize)
    {
      SizeToFit();
    }
  }

  /// <summary>
  /// Raises the <see cref="CellBorderColorChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnCellBorderColorChanged(EventArgs e)
  {
    EventHandler handler;

    if (AllowPainting)
    {
      Invalidate();
    }

    handler = (EventHandler)Events[EventCellBorderColorChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="CellBorderStyleChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnCellBorderStyleChanged(EventArgs e)
  {
    EventHandler handler;

    if (AllowPainting)
    {
      Invalidate();
    }

    handler = (EventHandler)Events[EventCellBorderStyleChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="CellContextMenuStripChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnCellContextMenuStripChanged(EventArgs e)
  {
    EventHandler handler;

    handler = (EventHandler)Events[EventCellContextMenuStripChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="CellSizeChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnCellSizeChanged(EventArgs e)
  {
    EventHandler handler;

    SetScaledCellSize();

    if (AutoSize)
    {
      SizeToFit();
    }

    if (AllowPainting)
    {
      RefreshColors();
      Invalidate();
    }

    handler = (EventHandler)Events[EventCellSizeChanged];

    handler?.Invoke(this, e);
  }

  private void SetScaledCellSize()
  {
    Point dpi;
    float scaleX;
    float scaleY;

    dpi = NativeMethods.GetDesktopDpi();
    scaleX = dpi.X / 96F;
    scaleY = dpi.Y / 96F;

    if (scaleX > 1 && scaleY > 1)
    {
      _scaledCellSize = new Size((int)(_cellSize.Width * scaleX), (int)(_cellSize.Height * scaleY));
    }
    else
    {
      _scaledCellSize = _cellSize;
    }
  }

  /// <summary>
  /// Raises the <see cref="ColorChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnColorChanged(EventArgs e)
  {
    EventHandler handler;

    handler = (EventHandler)Events[EventColorChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="ColorIndexChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnColorIndexChanged(EventArgs e)
  {
    EventHandler handler;

    if (AllowPainting)
    {
      Invalidate(_previousColorIndex);
      Invalidate(ColorIndex);
    }

    handler = (EventHandler)Events[EventColorIndexChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="ColorsChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnColorsChanged(EventArgs e)
  {
    EventHandler handler;

    AddEventHandlers(Colors);

    RefreshColors();

    handler = (EventHandler)Events[EventColorsChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="ColumnsChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnColumnsChanged(EventArgs e)
  {
    EventHandler handler;

    RefreshColors();

    handler = (EventHandler)Events[EventColumnsChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="CustomColorsChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnCustomColorsChanged(EventArgs e)
  {
    EventHandler handler;

    AddEventHandlers(CustomColors);
    RefreshColors();

    handler = (EventHandler)Events[EventCustomColorsChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="EditingColor" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EditColorCancelEventArgs" /> instance containing the event data.</param>
  protected virtual void OnEditingColor(EditColorCancelEventArgs e)
  {
    EventHandler<EditColorCancelEventArgs> handler;

    handler = (EventHandler<EditColorCancelEventArgs>)Events[EventEditingColor];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="EditModeChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnEditModeChanged(EventArgs e)
  {
    EventHandler handler;

    handler = (EventHandler)Events[EventEditModeChanged];

    handler?.Invoke(this, e);
  }

  protected override void OnGotFocus(EventArgs e)
  {
    base.OnGotFocus(e);

    if (AllowPainting)
    {
      Invalidate(ColorIndex);
    }
  }

  /// <summary>
  /// Raises the <see cref="HotIndexChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnHotIndexChanged(EventArgs e)
  {
    EventHandler handler;

    SetToolTip();

    if (AllowPainting)
    {
      Invalidate(_previousHotIndex);
      Invalidate(HotIndex);
    }

    handler = (EventHandler)Events[EventHotIndexChanged];

    handler?.Invoke(this, e);
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    WasKeyPressed = true;

    switch (e.KeyData)
    {
      case Keys.Down:
        Navigate(0, 1);
        e.Handled = true;
        break;
      case Keys.Up:
        Navigate(0, -1);
        e.Handled = true;
        break;
      case Keys.Left:
        Navigate(-1, 0);
        e.Handled = true;
        break;
      case Keys.Right:
        Navigate(1, 0);
        e.Handled = true;
        break;
      case Keys.Home:
        Navigate(0, 0, NavigationOrigin.Begin);
        e.Handled = true;
        break;
      case Keys.End:
        Navigate(0, 0, NavigationOrigin.End);
        e.Handled = true;
        break;
    }

    base.OnKeyDown(e);
  }

  protected override void OnKeyUp(KeyEventArgs e)
  {
    if (WasKeyPressed && ColorIndex != INVALID_INDEX)
    {
      switch (e.KeyData)
      {
        case Keys.Enter:
          ColorSource source;

          source = GetColorSource(ColorIndex);

          if (source == ColorSource.Custom && EditMode != ColorEditingMode.None || source == ColorSource.Standard && EditMode == ColorEditingMode.Both)
          {
            e.Handled = true;

            StartColorEdit(ColorIndex);
          }
          break;
        case Keys.Apps:
        case Keys.F10 | Keys.Shift:
          int x;
          int y;
          Point location;

          location = _colorRegions[_colorIndex].Location;
          x = location.X;
          y = location.Y + _cellSize.Height;

          ShowContextMenu(new Point(x, y));
          break;
      }
    }

    WasKeyPressed = false;

    base.OnKeyUp(e);
  }

  private Size _scaledCellSize;

  protected override void OnLostFocus(EventArgs e)
  {
    base.OnLostFocus(e);

    if (AllowPainting)
    {
      Invalidate(ColorIndex);
    }
  }

  protected override void OnMouseDoubleClick(MouseEventArgs e)
  {
    ColorHitTestInfo hitTest;

    base.OnMouseDoubleClick(e);

    hitTest = HitTest(e.Location);

    if (e.Button == MouseButtons.Left && (hitTest.Source == ColorSource.Custom && EditMode != ColorEditingMode.None || hitTest.Source == ColorSource.Standard && EditMode == ColorEditingMode.Both))
    {
      StartColorEdit(hitTest.Index);
    }
  }

  protected override void OnMouseDown(MouseEventArgs e)
  {
    base.OnMouseDown(e);

    if (!Focused && TabStop)
    {
      Focus();
    }

    ProcessMouseClick(e);
  }

  protected override void OnMouseLeave(EventArgs e)
  {
    base.OnMouseLeave(e);

    HotIndex = INVALID_INDEX;
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    base.OnMouseMove(e);

    HotIndex = HitTest(e.Location).Index;

    ProcessMouseClick(e);
  }

  protected override void OnMouseUp(MouseEventArgs e)
  {
    base.OnMouseUp(e);

    if (e.Button == MouseButtons.Right)
    {
      int index;

      index = HitTest(e.Location).Index;

      if (index != INVALID_INDEX)
      {
        Focus();
        ColorIndex = index;

        ShowContextMenu(e.Location);
      }
    }
  }

  protected override void OnPaddingChanged(EventArgs e)
  {
    base.OnPaddingChanged(e);

    if (AllowPainting)
    {
      RefreshColors();
      Invalidate();
    }
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    base.OnPaint(e);

    if (AllowPainting)
    {
      int colorCount;

      colorCount = Colors.Count;

      OnPaintBackground(e); // HACK: Easiest way of supporting things like BackgroundImage, BackgroundImageLayout etc as the PaintBackground event is no longer being called

      // draw a design time dotted grid
      if (DesignMode)
      {
        using (Pen pen = new Pen(SystemColors.ButtonShadow)
               {
                 DashStyle = DashStyle.Dot
               })
        {
          e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
      }

      // draw cells for all current colors
      for (int i = 0; i < colorCount; i++)
      {
        Rectangle bounds;

        bounds = _colorRegions[i];
        if (e.ClipRectangle.IntersectsWith(bounds))
        {
          PaintCell(e, i, i, Colors[i], bounds);
        }
      }

      if (CustomColors.Count != 0 && ShowCustomColors)
      {
        // draw a separator
        PaintSeparator(e);

        // and the custom colors
        for (int i = 0; i < CustomColors.Count; i++)
        {
          Rectangle bounds;

          if (_colorRegions.TryGetValue(colorCount + i, out bounds) && e.ClipRectangle.IntersectsWith(bounds))
          {
            PaintCell(e, i, colorCount + i, CustomColors[i], bounds);
          }
        }
      }

      // draw the selected color
      if (SelectedCellStyle != ColorGridSelectedCellStyle.None && ColorIndex >= 0)
      {
        Rectangle bounds;

        if (_colorRegions.TryGetValue(ColorIndex, out bounds) && e.ClipRectangle.IntersectsWith(bounds))
        {
          PaintSelectedCell(e, ColorIndex, Color, bounds);
        }
      }
    }
  }

  /// <summary>
  /// Raises the <see cref="PaletteChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnPaletteChanged(EventArgs e)
  {
    EventHandler handler;

    Colors = GetPredefinedPalette();

    handler = (EventHandler)Events[EventPaletteChanged];

    handler?.Invoke(this, e);
  }

  protected override void OnResize(EventArgs e)
  {
    RefreshColors();

    base.OnResize(e);
  }

  /// <summary>
  /// Raises the <see cref="SelectedCellStyleChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnSelectedCellStyleChanged(EventArgs e)
  {
    EventHandler handler;

    if (AllowPainting)
    {
      Invalidate();
    }

    handler = (EventHandler)Events[EventSelectedCellStyleChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="ShowCustomColorsChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnShowCustomColorsChanged(EventArgs e)
  {
    EventHandler handler;

    RefreshColors();

    handler = (EventHandler)Events[EventShowCustomColorsChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="ShowToolTipsChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnShowToolTipsChanged(EventArgs e)
  {
    EventHandler handler;

    if (ShowToolTips)
    {
      _toolTip = new ToolTip();
    }
    else if (_toolTip != null)
    {
      _toolTip.Dispose();
      _toolTip = null;
    }

    handler = (EventHandler)Events[EventShowToolTipsChanged];

    handler?.Invoke(this, e);
  }

  /// <summary>
  /// Raises the <see cref="SpacingChanged" /> event.
  /// </summary>
  /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
  protected virtual void OnSpacingChanged(EventArgs e)
  {
    EventHandler handler;

    if (AutoSize)
    {
      SizeToFit();
    }

    if (AllowPainting)
    {
      RefreshColors();
      Invalidate();
    }

    handler = (EventHandler)Events[EventSpacingChanged];

    handler?.Invoke(this, e);
  }

  protected virtual void PaintCell(PaintEventArgs e, int colorIndex, int cellIndex, Color color, Rectangle bounds)
  {
    if (color.A != 255)
    {
      PaintTransparentCell(e, bounds);
    }

    using (Brush brush = new SolidBrush(color))
    {
      e.Graphics.FillRectangle(brush, bounds);
    }

    switch (CellBorderStyle)
    {
      case ColorCellBorderStyle.FixedSingle:
        using (Pen pen = new Pen(CellBorderColor))
        {
          e.Graphics.DrawRectangle(pen, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
        }
        break;
      case ColorCellBorderStyle.DoubleSoft:
        HslColor shadedOuter;
        HslColor shadedInner;

        shadedOuter = new HslColor(color);
        shadedOuter.L -= 0.50;

        shadedInner = new HslColor(color);
        shadedInner.L -= 0.20;

        using (Pen pen = new Pen(CellBorderColor))
        {
          e.Graphics.DrawRectangle(pen, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
        }
        e.Graphics.DrawRectangle(Pens.White, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3);
        using (Pen pen = new Pen(Color.FromArgb(32, shadedOuter.ToRgbColor())))
        {
          e.Graphics.DrawRectangle(pen, bounds.Left + 2, bounds.Top + 2, bounds.Width - 5, bounds.Height - 5);
        }
        using (Pen pen = new Pen(Color.FromArgb(32, shadedInner.ToRgbColor())))
        {
          e.Graphics.DrawRectangle(pen, bounds.Left + 3, bounds.Top + 3, bounds.Width - 7, bounds.Height - 7);
        }
        break;
    }

    if (HotIndex != INVALID_INDEX && HotIndex == cellIndex)
    {
      e.Graphics.DrawRectangle(Pens.Black, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
      e.Graphics.DrawRectangle(Pens.White, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3);
    }
  }

  protected virtual void PaintSelectedCell(PaintEventArgs e, int colorIndex, Color color, Rectangle bounds)
  {
    switch (SelectedCellStyle)
    {
      case ColorGridSelectedCellStyle.Standard:
        if (Focused)
        {
          ControlPaint.DrawFocusRectangle(e.Graphics, bounds);
        }
        else
        {
          e.Graphics.DrawRectangle(Pens.Black, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
        }
        break;
      case ColorGridSelectedCellStyle.Zoomed:
        // make the cell larger according to the padding
        if (SelectedCellStyle == ColorGridSelectedCellStyle.Zoomed)
        {
          bounds.Inflate(Padding.Left, Padding.Top);
        }

        // fill the inner
        e.Graphics.FillRectangle(Brushes.White, bounds);
        if (SelectedCellStyle == ColorGridSelectedCellStyle.Zoomed)
        {
          bounds.Inflate(-3, -3);
        }
        if (color.A != 255)
        {
          PaintTransparentCell(e, bounds);
        }

        using (Brush brush = new SolidBrush(color))
        {
          e.Graphics.FillRectangle(brush, bounds);
        }

        // draw a border
        if (Focused)
        {
          bounds = new Rectangle(bounds.Left - 2, bounds.Top - 2, bounds.Width + 4, bounds.Height + 4);
          ControlPaint.DrawFocusRectangle(e.Graphics, bounds);
        }
        else
        {
          bounds = new Rectangle(bounds.Left - 2, bounds.Top - 2, bounds.Width + 3, bounds.Height + 3);

          using (Pen pen = new Pen(CellBorderColor))
          {
            e.Graphics.DrawRectangle(pen, bounds);
          }
        }
        break;
    }
  }

  protected virtual void PaintSeparator(PaintEventArgs e)
  {
    int x1;
    int y1;
    int x2;
    int y2;

    x1 = Padding.Left;
    x2 = ClientSize.Width - Padding.Right;
    y1 = SeparatorHeight / 2 + Padding.Top + PrimaryRows * (_scaledCellSize.Height + Spacing.Height) + 1 - Spacing.Height;
    y2 = y1;

    using (Pen pen = new Pen(CellBorderColor))
    {
      e.Graphics.DrawLine(pen, x1, y1, x2, y2);
    }
  }

  protected virtual void PaintTransparentCell(PaintEventArgs e, Rectangle bounds)
  {
    if (_cellBackgroundBrush == null)
    {
      _cellBackgroundBrush = CreateTransparencyBrush();
    }

    e.Graphics.FillRectangle(_cellBackgroundBrush, bounds);
  }

  protected virtual void ProcessMouseClick(MouseEventArgs e)
  {
    if (e.Button == MouseButtons.Left)
    {
      ColorHitTestInfo hitTest;

      hitTest = HitTest(e.Location);

      if (hitTest.Source != ColorSource.None)
      {
        Color = hitTest.Color;
        ColorIndex = hitTest.Index;
      }
    }
  }

  protected virtual void RefreshColors()
  {
    if (AllowPainting)
    {
      CalculateGridSize();
      if (AutoFit)
      {
        CalculateCellSize();
      }
      else if (AutoSize)
      {
        SizeToFit();
      }

      _colorRegions.Clear();

      if (Colors != null)
      {
        DefineColorRegions(Colors, 0, Padding.Top);
        if (ShowCustomColors)
        {
          DefineColorRegions(CustomColors, Colors.Count, Padding.Top + SeparatorHeight + (_scaledCellSize.Height + Spacing.Height) * PrimaryRows);
        }

        ColorIndex = GetColorIndex(Color);

        if (!Color.IsEmpty && ColorIndex == INVALID_INDEX && AutoAddColors && ShowCustomColors)
        {
          AddCustomColor(Color);
        }

        Invalidate();
      }
    }
    else
    {
      _layoutRequired = true;
    }
  }

  protected virtual void SetColor(int colorIndex, Color color)
  {
    int colorCount;

    colorCount = Colors.Count;

    if (colorIndex < 0 || colorIndex > colorCount + CustomColors.Count)
    {
      throw new ArgumentOutOfRangeException(nameof(colorIndex));
    }

    if (colorIndex > colorCount - 1)
    {
      CustomColors[colorIndex - colorCount] = color;
    }
    else
    {
      Colors[colorIndex] = color;
    }
  }

  private void AddEventHandlers(ColorCollection value)
  {
    if (value != null)
    {
      value.ItemInserted += ColorsCollectionChangedHandler;
      value.ItemRemoved += ColorsCollectionChangedHandler;
      value.ItemsCleared += ColorsCollectionChangedHandler;
      value.ItemReplaced += ColorsCollectionItemReplacedHandler;
    }
  }

  private void ColorsCollectionChangedHandler(object sender, ColorCollectionEventArgs e)
  {
    RefreshColors();
  }

  private void ColorsCollectionItemReplacedHandler(object sender, ColorCollectionEventArgs e)
  {
    ColorCollection collection;
    int index;

    collection = (ColorCollection)sender;
    index = _colorIndex;
    if (index != INVALID_INDEX && ReferenceEquals(collection, CustomColors))
    {
      index -= Colors.Count;
    }

    if (index >= 0 && index < collection.Count && collection[index] != Color)
    {
      _previousColorIndex = index;
      _colorIndex = -1;
      ColorIndex = index;
    }

    Invalidate(e.Index);
  }

  private Point GetCell(int index)
  {
    int row;
    int column;

    if (index == INVALID_INDEX)
    {
      row = -1;
      column = -1;
    }
    else if (index >= Colors.Count)
    {
      // custom color
      index -= Colors.Count;
      row = index / ActualColumns;
      column = index - row * ActualColumns;
      row += PrimaryRows;
    }
    else
    {
      // normal row
      row = index / ActualColumns;
      column = index - row * ActualColumns;
    }

    return new Point(column, row);
  }

  private void RemoveEventHandlers(ColorCollection value)
  {
    if (value != null)
    {
      value.ItemInserted -= ColorsCollectionChangedHandler;
      value.ItemRemoved -= ColorsCollectionChangedHandler;
      value.ItemsCleared -= ColorsCollectionChangedHandler;
      value.ItemReplaced -= ColorsCollectionItemReplacedHandler;
    }
  }

  private void SetToolTip()
  {
    if (ShowToolTips)
    {
      if (ShowToolTips)
      {
#if USENAMEHACK
        string name;

        if (this.HotIndex != InvalidIndex)
        {
          name = this.HotIndex < this.Colors.Count ? this.Colors.GetName(this.HotIndex) : this.CustomColors.GetName(this.HotIndex);

          if (string.IsNullOrEmpty(name))
          {
            name = this.GetColor(this.HotIndex).Name;
          }
        }
        else
        {
          name = null;
        }

        _toolTip.SetToolTip(this, name);
#else
        _toolTip.SetToolTip(this, HotIndex != INVALID_INDEX ? GetColor(HotIndex).Name : null);
#endif
      }
    }
  }

  private void ShowContextMenu(Point location)
  {
    _cellContextMenuStrip?.Show(this, location);
  }

  private void SizeToFit()
  {
    Size = GetAutoSize();
  }

  private void StartColorEdit(int index)
  {
    EditColorCancelEventArgs e;

    e = new EditColorCancelEventArgs(GetColor(index), index);
    OnEditingColor(e);

    if (!e.Cancel)
    {
      EditColor(index);
    }
  }

  #endregion

  #region IColorEditor Interface

  [Category("Property Changed")]
  public event EventHandler ColorChanged
  {
    add => Events.AddHandler(EventColorChanged, value);
    remove => Events.RemoveHandler(EventColorChanged, value);
  }

  [Category("Appearance")]
  [DefaultValue(typeof(Color), "Black")]
  public virtual Color Color
  {
    get => _color;
    set
    {
      int newIndex;

      _color = value;

      if (!value.IsEmpty)
      {
        // the new color matches the color at the current index, so don't change the index
        // this stops the selection hopping about if you have duplicate colors in a palette
        // otherwise, if the colors don't match, then find the index that does
        newIndex = GetColor(ColorIndex) == value ? ColorIndex : GetColorIndex(value);

        if (newIndex == INVALID_INDEX)
        {
          newIndex = AddCustomColor(value);
        }
      }
      else
      {
        newIndex = INVALID_INDEX;
      }

      ColorIndex = newIndex;

      OnColorChanged(EventArgs.Empty);
    }
  }

  #endregion
}
