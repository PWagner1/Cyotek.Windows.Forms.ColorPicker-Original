// Cyotek Color Picker Controls Library
// http://cyotek.com/blog/tag/colorpicker

// Copyright © 2013-2021 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.Drawing;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Cyotek.Windows.Forms.ColorPicker.Tests
{
  /// <summary>
  /// Tests for the <see cref="ColorCollection"/> class
  /// </summary>
  [TestFixture]
  public class ColorCollectionTests : TestBase
  {
    #region  Tests

    [Test]
    public void CloneFromInterfaceTest()
    {
      // arrange
      ColorCollection actual;
      ColorCollection expected;

      expected = this.CreateDawnBringer32Palette(false);

      // act
      actual = (ColorCollection)((ICloneable)expected).Clone();

      // assert
      CollectionAssert.AreEqual(actual, expected);
    }

    [Test]
    public void CloneTest()
    {
      // arrange
      ColorCollection actual;
      ColorCollection expected;

      expected = this.CreateDawnBringer32Palette(false);

      // act
      actual = expected.Clone();

      // assert
      CollectionAssert.AreEqual(actual, expected);
    }

    [Test]
    public void EqualsNegativeTest()
    {
      // arrange
      ColorCollection target;
      ColorCollection other;
      bool actual;

      target = ColorPalettes.QbColors;
      other = ColorPalettes.QbColors;

      other[5] = Color.CornflowerBlue;

      // act
      actual = other == target;

      // assert
      ClassicAssert.IsFalse(actual);
    }

    [Test]
    public void EqualsNullTest()
    {
      // arrange
      ColorCollection target;
      bool actual;

      target = new ColorCollection();

      // act
      actual = target.Equals(null);

      // assert
      ClassicAssert.IsFalse(actual);
    }

    [Test]
    public void EqualsObjectTest()
    {
      // arrange
      ColorCollection target;
      object other;
      bool actual;

      target = new ColorCollection();
      other = target;

      // act
      actual = target.Equals(other);

      // assert
      ClassicAssert.IsTrue(actual);
    }

    [Test]
    public void EqualsObjectWrongTypeTest()
    {
      // arrange
      ColorCollection target;
      object other;
      bool actual;

      target = new ColorCollection();
      other = 42;

      // act
      actual = target.Equals(other);

      // assert
      ClassicAssert.IsFalse(actual);
    }

    [Test]
    public void EqualsSameContentsTest()
    {
      // arrange
      ColorCollection target;
      ColorCollection other;
      bool actual;

      target = ColorPalettes.QbColors;
      other = ColorPalettes.QbColors;

      // act
      actual = other == target;

      // assert
      ClassicAssert.IsTrue(actual);
      ClassicAssert.AreNotSame(target, other);
    }

    [Test]
    public void EqualsSameReferenceTest()
    {
      // arrange
      ColorCollection target;
      ColorCollection other;
      bool actual;

      target = new ColorCollection();
      other = target;

      // act
      actual = other == target;

      // assert
      ClassicAssert.IsTrue(actual);
      ClassicAssert.AreSame(target, other);
    }

    [Test]
    public void FindIgnoreAlphaNegativeTest()
    {
      // arrange
      ColorCollection target;
      int expected;
      int actual;

      target = new ColorCollection();
      target.Add(Color.Black);
      target.Add(Color.CornflowerBlue);
      target.Add(Color.Violet);
      expected = -1;

      // act
      actual = target.Find(Color.FromArgb(128, Color.CornflowerBlue), false);

      // assert
      ClassicAssert.AreEqual(expected, actual);
    }

    [Test]
    public void FindIgnoreAlphaTest()
    {
      // arrange
      ColorCollection target;
      int expected;
      int actual;

      target = new ColorCollection();
      target.Add(Color.Black);
      target.Add(Color.CornflowerBlue);
      target.Add(Color.Violet);
      expected = 1;

      // act
      actual = target.Find(Color.FromArgb(128, Color.CornflowerBlue), true);

      // assert
      ClassicAssert.AreEqual(expected, actual);
    }

    [Test]
    public void FindNegativeTest()
    {
      // arrange
      ColorCollection target;
      int expected;
      int actual;

      target = new ColorCollection();
      target.Add(Color.Black);
      target.Add(Color.CornflowerBlue);
      target.Add(Color.Violet);
      expected = -1;

      // act
      actual = target.Find(Color.Yellow);

      // assert
      ClassicAssert.AreEqual(expected, actual);
    }

    [Test]
    public void FindTest()
    {
      // arrange
      ColorCollection target;
      int expected;
      int actual;

      target = new ColorCollection();
      target.Add(Color.Black);
      target.Add(Color.CornflowerBlue);
      target.Add(Color.Violet);
      expected = 1;

      // act
      actual = target.Find(Color.FromArgb(100, 149, 237));

      // assert
      ClassicAssert.AreEqual(expected, actual);
    }

    #endregion
  }
}
