using System;
using System.Globalization;
using System.Windows;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests.Converters
{
  public class InverseBoolConverterTests
  {
    private readonly InverseBoolConverter _converter = new InverseBoolConverter();

    [Theory]
    [InlineData( true, false )]
    [InlineData( false, true )]
    public void Convert_NegatesValue( bool value, bool expected )
    {
      var result = _converter.Convert( value, typeof( bool ), null, CultureInfo.InvariantCulture );

      Assert.Equal( expected, result );
    }

    [Fact]
    public void ConvertBack_Throws()
    {
      Assert.Throws<NotImplementedException>(
        () => { _converter.ConvertBack( true, typeof( bool ), null, CultureInfo.InvariantCulture ); } );
    }
  }

  public class NullToBoolConverterTests
  {
    private readonly NullToBoolConverter _converter = new NullToBoolConverter();

    [Fact]
    public void Convert_NullValue_ReturnsTrue()
    {
      Assert.Equal( true, _converter.Convert( null, typeof( bool ), null, CultureInfo.InvariantCulture ) );
    }

    [Fact]
    public void Convert_NonNullValue_ReturnsFalse()
    {
      Assert.Equal( false, _converter.Convert( new object(), typeof( bool ), null, CultureInfo.InvariantCulture ) );
    }
  }

  public class VisibilityToBoolConverterTests
  {
    [Theory]
    [InlineData( Visibility.Visible, true )]
    [InlineData( Visibility.Collapsed, false )]
    [InlineData( Visibility.Hidden, false )]
    public void Convert_MapsVisibilityToBool( Visibility visibility, bool expected )
    {
      var converter = new VisibilityToBoolConverter();

      var result = converter.Convert( visibility, typeof( bool ), null, CultureInfo.InvariantCulture );

      Assert.Equal( expected, result );
    }

    [Fact]
    public void Convert_WithNot_InvertsResult()
    {
      var converter = new VisibilityToBoolConverter { Not = true };

      Assert.Equal( false, converter.Convert( Visibility.Visible, typeof( bool ), null, CultureInfo.InvariantCulture ) );
      Assert.Equal( true, converter.Convert( Visibility.Collapsed, typeof( bool ), null, CultureInfo.InvariantCulture ) );
    }

    [Fact]
    public void Convert_WhenInverted_MapsBoolToVisibility()
    {
      var converter = new VisibilityToBoolConverter { Inverted = true };

      Assert.Equal( Visibility.Visible, converter.Convert( true, typeof( Visibility ), null, CultureInfo.InvariantCulture ) );
      Assert.Equal( Visibility.Collapsed, converter.Convert( false, typeof( Visibility ), null, CultureInfo.InvariantCulture ) );
    }

    [Fact]
    public void ConvertBack_MapsBoolToVisibility()
    {
      var converter = new VisibilityToBoolConverter();

      Assert.Equal( Visibility.Visible, converter.ConvertBack( true, typeof( Visibility ), null, CultureInfo.InvariantCulture ) );
      Assert.Equal( Visibility.Collapsed, converter.ConvertBack( false, typeof( Visibility ), null, CultureInfo.InvariantCulture ) );
    }

    [Fact]
    public void Convert_WrongValueType_Throws()
    {
      var converter = new VisibilityToBoolConverter();

      Assert.Throws<InvalidOperationException>(
        () => { converter.Convert( "not a visibility", typeof( bool ), null, CultureInfo.InvariantCulture ); } );
    }

    [Fact]
    public void ConvertBack_WrongValueType_Throws()
    {
      var converter = new VisibilityToBoolConverter();

      Assert.Throws<InvalidOperationException>(
        () => { converter.ConvertBack( "not a bool", typeof( Visibility ), null, CultureInfo.InvariantCulture ); } );
    }
  }

  public class CalculatorMemoryToVisibilityConverterTests
  {
    private readonly CalculatorMemoryToVisibilityConverter _converter = new CalculatorMemoryToVisibilityConverter();

    [Fact]
    public void Convert_EmptyMemory_IsHidden()
    {
      var result = _converter.Convert( decimal.Zero, typeof( Visibility ), null, CultureInfo.InvariantCulture );

      Assert.Equal( Visibility.Hidden, result );
    }

    [Fact]
    public void Convert_NonEmptyMemory_IsVisible()
    {
      var result = _converter.Convert( 12.5m, typeof( Visibility ), null, CultureInfo.InvariantCulture );

      Assert.Equal( Visibility.Visible, result );
    }
  }
}
