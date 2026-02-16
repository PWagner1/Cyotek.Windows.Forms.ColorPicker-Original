namespace Cyotek.Windows.Forms;

public class ColorPickerEditor : UITypeEditor
{
  public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

  public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
  {
    IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

    if (editorService != null)
    {
        return null;
    }

    ScreenColorPicker picker = new ScreenColorPicker(editorService);

    picker.Size = new Size(100, 100);

    picker.Dock = DockStyle.Fill;

    editorService.DropDownControl(picker);

    return picker.Color;
  } 
}
