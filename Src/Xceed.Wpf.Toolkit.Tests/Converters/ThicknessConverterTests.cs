using System;
using System.Globalization;
using System.Windows;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests.Converters
{
  public class IntToThicknessConverterTests
  {
    private readonly IntToThicknessConverter _converter = new IntToThicknessConverter();

    [Fact]
    public void Convert_WithoutParameter_ReturnsUniformThickness()
    {
      var result = _converter.Convert( 5, typeof( Thickness ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 5 ), result );
    }

    [Fact]
    public void Convert_NullValue_ReturnsZeroThickness()
    {
      var result = _converter.Convert( null, typeof( Thickness ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 0 ), result );
    }

    [Theory]
    [InlineData( "LEFT" )]
    [InlineData( "left" )]
    [InlineData( "Left" )]
    public void Convert_LeftParameter_IsCaseInsensitiveAndOnlySetsLeftSide( string parameter )
    {
      var result = _converter.Convert( 5, typeof( Thickness ), parameter, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 5, 0, 0, 0 ), result );
    }

    [Fact]
    public void Convert_UnknownParameter_FallsBackToUniformThickness()
    {
      var result = _converter.Convert( 5, typeof( Thickness ), "RIGHT", CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 5 ), result );
    }

    [Fact]
    public void ConvertBack_Throws()
    {
      Assert.Throws<NotImplementedException>(
        () => { _converter.ConvertBack( new Thickness( 1 ), typeof( int ), null, CultureInfo.InvariantCulture ); } );
    }
  }

  public class ThicknessToDoubleConverterTests
  {
    private readonly ThicknessToDoubleConverter _converter = new ThicknessToDoubleConverter();

    [Fact]
    public void Convert_ReturnsTopSide()
    {
      var result = _converter.Convert( new Thickness( 2, 3, 4, 5 ), typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 3d, result );
    }

    [Fact]
    public void Convert_NullValue_ReturnsOne()
    {
      var result = _converter.Convert( null, typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 1d, result );
    }

    [Fact]
    public void ConvertBack_ReturnsUniformThickness()
    {
      var result = _converter.ConvertBack( 4d, typeof( Thickness ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 4 ), result );
    }

    [Fact]
    public void ConvertBack_NullValue_ReturnsThicknessOfOne()
    {
      var result = _converter.ConvertBack( null, typeof( Thickness ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 1 ), result );
    }
  }

  public class BorderThicknessToStrokeThicknessConverterTests
  {
    private readonly BorderThicknessToStrokeThicknessConverter _converter = new BorderThicknessToStrokeThicknessConverter();

    [Fact]
    public void Convert_ReturnsAverageOfAllSides()
    {
      var result = _converter.Convert( new Thickness( 1, 2, 3, 4 ), typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 2.5d, result );
    }

    [Fact]
    public void ConvertBack_ReturnsUniformThickness()
    {
      var result = _converter.ConvertBack( ( int? )3, typeof( Thickness ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 3 ), result );
    }

    [Fact]
    public void ConvertBack_NullValue_ReturnsZeroThickness()
    {
      var result = _converter.ConvertBack( null, typeof( Thickness ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( 0 ), result );
    }
  }

  public class ThicknessSideRemovalConverterTests
  {
    private readonly ThicknessSideRemovalConverter _converter = new ThicknessSideRemovalConverter();

    [Theory]
    [InlineData( "0", 0d, 2d, 3d, 4d )]
    [InlineData( "1", 1d, 0d, 3d, 4d )]
    [InlineData( "2", 1d, 2d, 0d, 4d )]
    [InlineData( "3", 1d, 2d, 3d, 0d )]
    public void Convert_RemovesRequestedSide( string parameter, double left, double top, double right, double bottom )
    {
      var result = _converter.Convert( new Thickness( 1, 2, 3, 4 ), typeof( Thickness ), parameter, CultureInfo.InvariantCulture );

      Assert.Equal( new Thickness( left, top, right, bottom ), result );
    }

    [Fact]
    public void Convert_SideOutOfRange_Throws()
    {
      Assert.Throws<InvalidContentException>(
        () => { _converter.Convert( new Thickness( 1 ), typeof( Thickness ), "4", CultureInfo.InvariantCulture ); } );
    }
  }

  public class CornerRadiusToDoubleConverterTests
  {
    private readonly CornerRadiusToDoubleConverter _converter = new CornerRadiusToDoubleConverter();

    [Fact]
    public void Convert_ReturnsTopLeftCorner()
    {
      var result = _converter.Convert( new CornerRadius( 1, 2, 3, 4 ), typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 1d, result );
    }

    [Fact]
    public void Convert_NullValue_ReturnsZero()
    {
      var result = _converter.Convert( null, typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 0d, result );
    }

    [Fact]
    public void ConvertBack_ReturnsUniformCornerRadius()
    {
      var result = _converter.ConvertBack( 6d, typeof( CornerRadius ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new CornerRadius( 6 ), result );
    }
  }

  public class WindowContentBorderMarginConverterTests
  {
    private readonly WindowContentBorderMarginConverter _converter = new WindowContentBorderMarginConverter();

    private object Convert( string parameter )
    {
      return _converter.Convert( new object[] { 7d, 9d }, typeof( Thickness ), parameter, CultureInfo.InvariantCulture );
    }

    [Fact]
    public void Convert_ContentBorder_UsesHorizontalOffsetOnBothSides()
    {
      Assert.Equal( new Thickness( 7d, 0d, 7d, 9d ), Convert( "0" ) );
    }

    [Fact]
    public void Convert_ThumbGrip_OnlyUsesRightAndBottomOffsets()
    {
      Assert.Equal( new Thickness( 0d, 0d, 7d, 9d ), Convert( "1" ) );
    }

    [Fact]
    public void Convert_HeaderButtons_OnlyUsesRightOffset()
    {
      Assert.Equal( new Thickness( 0d, 0d, 7d, 0d ), Convert( "2" ) );
    }

    [Fact]
    public void Convert_UnknownParameter_Throws()
    {
      Assert.Throws<NotSupportedException>( () => { Convert( "3" ); } );
    }
  }
}
