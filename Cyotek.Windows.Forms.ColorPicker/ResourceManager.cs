// Cyotek Color Picker controls library
// http://cyotek.com/blog/tag/colorpicker

// Copyright © 2021 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.Windows.Forms;

internal static class ResourceManager
{
  #region Public Properties

  public static Image CellBackground => GetResourceImage("cellbackground.png");

  public static Cursor EyeDropper => GetResourceCursor("eyedropper.cur");

  public static Image LoadPalette => GetResourceImage("palette-load.png");

  public static Image SavePalette => GetResourceImage("palette-save.png");

  public static Image ScreenPicker => GetResourceImage("eyedropper.png");

  #endregion Public Properties

  #region Private Methods

  private static Cursor GetResourceCursor(string name) => new(GetResourceStream(name));

  private static Icon GetResourceIcon(string name) => new(GetResourceStream(name));

  private static Bitmap GetResourceImage(string name) => new(GetResourceStream(name));

  private static Stream GetResourceStream(string name)
  {
    var type = typeof(ResourceManager);
    var assembly = type.Assembly;
    var resourceName = type.Namespace + ".Resources." + name;
    var stream = assembly.GetManifestResourceStream(resourceName);

    if (stream == null)
    {
      throw new ArgumentException($"Cannot find resource '{resourceName}'.");
    }

    return stream;
  }

  #endregion Private Methods
}
