using System;
using System.Globalization;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests.Converters
{
  public class ColorToSolidColorBrushConverterTests
  {
    private readonly ColorToSolidColorBrushConverter _converter = new ColorToSolidColorBrushConverter();

    [Fact]
    public void Convert_ReturnsBrushWithSameColor()
    {
      var result = _converter.Convert( Colors.Red, typeof( Brush ), null, CultureInfo.InvariantCulture );

      var brush = Assert.IsType<SolidColorBrush>( result );
      Assert.Equal( Colors.Red, brush.Color );
    }

    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
      Assert.Null( _converter.Convert( null, typeof( Brush ), null, CultureInfo.InvariantCulture ) );
    }

    [Fact]
    public void ConvertBack_ReturnsBrushColor()
    {
      var result = _converter.ConvertBack( new SolidColorBrush( Colors.Lime ), typeof( Color ), null, CultureInfo.InvariantCulture );

      Assert.Equal( Colors.Lime, result );
    }

    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
      Assert.Null( _converter.ConvertBack( null, typeof( Color ), null, CultureInfo.InvariantCulture ) );
    }
  }

  public class SolidColorBrushToColorConverterTests
  {
    private readonly SolidColorBrushToColorConverter _converter = new SolidColorBrushToColorConverter();

    [Fact]
    public void Convert_ReturnsBrushColor()
    {
      var result = _converter.Convert( new SolidColorBrush( Colors.Blue ), typeof( Color ), null, CultureInfo.InvariantCulture );

      Assert.Equal( Colors.Blue, result );
    }

    [Fact]
    public void Convert_NonBrushValue_ReturnsNull()
    {
      Assert.Null( _converter.Convert( "not a brush", typeof( Color ), null, CultureInfo.InvariantCulture ) );
    }

    [Fact]
    public void ConvertBack_ReturnsBrushWithSameColor()
    {
      var result = _converter.ConvertBack( Colors.Blue, typeof( Brush ), null, CultureInfo.InvariantCulture );

      var brush = Assert.IsType<SolidColorBrush>( result );
      Assert.Equal( Colors.Blue, brush.Color );
    }

    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
      Assert.Null( _converter.ConvertBack( null, typeof( Brush ), null, CultureInfo.InvariantCulture ) );
    }
  }

  public class ColorBlendConverterTests
  {
    [Fact]
    public void Convert_HalfRatio_BlendsChannelsEvenly()
    {
      var converter = new ColorBlendConverter { BlendedColor = Colors.Blue, BlendedColorRatio = 0.5d };

      var result = converter.Convert( Colors.Red, typeof( Color ), null, CultureInfo.InvariantCulture );

      Assert.Equal( Color.FromArgb( 255, 128, 0, 128 ), result );
    }

    [Fact]
    public void Convert_QuarterRatio_WeightsTheSourceColorMore()
    {
      var converter = new ColorBlendConverter { BlendedColor = Colors.Blue, BlendedColorRatio = 0.25d };

      var result = converter.Convert( Colors.Red, typeof( Color ), null, CultureInfo.InvariantCulture );

      Assert.Equal( Color.FromArgb( 255, 191, 0, 64 ), result );
    }

    [Fact]
    public void Convert_ZeroRatio_KeepsSourceColor()
    {
      var converter = new ColorBlendConverter { BlendedColor = Colors.Blue };

      var result = converter.Convert( Colors.Red, typeof( Color ), null, CultureInfo.InvariantCulture );

      Assert.Equal( Colors.Red, result );
    }

    [Fact]
    public void Convert_NonColorValue_ReturnsNull()
    {
      var converter = new ColorBlendConverter();

      Assert.Null( converter.Convert( "not a color", typeof( Color ), null, CultureInfo.InvariantCulture ) );
      Assert.Null( converter.Convert( null, typeof( Color ), null, CultureInfo.InvariantCulture ) );
    }

    [Theory]
    [InlineData( -0.1d )]
    [InlineData( 1.1d )]
    public void BlendedColorRatio_OutOfRange_Throws( double ratio )
    {
      var converter = new ColorBlendConverter();

      Assert.Throws<ArgumentException>( () => { converter.BlendedColorRatio = ratio; } );
    }
  }

  public class ColorModeToTabItemSelectedConverterTests
  {
    private readonly ColorModeToTabItemSelectedConverter _converter = new ColorModeToTabItemSelectedConverter();

    [Theory]
    [InlineData( ColorMode.ColorPalette, 0 )]
    [InlineData( ColorMode.ColorCanvas, 1 )]
    public void Convert_MapsColorModeToTabIndex( ColorMode mode, int expectedIndex )
    {
      var result = _converter.Convert( mode, typeof( int ), null, CultureInfo.InvariantCulture );

      Assert.Equal( expectedIndex, result );
    }

    [Theory]
    [InlineData( 0, ColorMode.ColorPalette )]
    [InlineData( 1, ColorMode.ColorCanvas )]
    public void ConvertBack_MapsTabIndexToColorMode( int index, ColorMode expectedMode )
    {
      var result = _converter.ConvertBack( index, typeof( ColorMode ), null, CultureInfo.InvariantCulture );

      Assert.Equal( expectedMode, result );
    }
  }
}
