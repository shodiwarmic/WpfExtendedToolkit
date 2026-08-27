using System;
using System.Globalization;
using System.Windows;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests.Converters
{
  public class AdditionConverterTests
  {
    private readonly AdditionConverter _converter = new AdditionConverter();

    [Fact]
    public void Convert_AddsParameterToValue()
    {
      var result = _converter.Convert( 1.5d, typeof( double ), "2", CultureInfo.CurrentCulture );

      Assert.Equal( 3.5d, result );
    }

    [Fact]
    public void Convert_NegativeParameter_SubtractsFromValue()
    {
      var result = _converter.Convert( 10d, typeof( double ), "-4", CultureInfo.CurrentCulture );

      Assert.Equal( 6d, result );
    }

    [Fact]
    public void Convert_NullValue_ReturnsZero()
    {
      var result = _converter.Convert( null, typeof( double ), "2", CultureInfo.CurrentCulture );

      Assert.Equal( 0d, result );
    }

    [Fact]
    public void Convert_NullParameter_ReturnsZero()
    {
      var result = _converter.Convert( 1.5d, typeof( double ), null, CultureInfo.CurrentCulture );

      Assert.Equal( 0d, result );
    }
  }

  public class HalfConverterTests
  {
    private readonly HalfConverter _converter = new HalfConverter();

    [Fact]
    public void Convert_WithoutParameter_ReturnsHalfOfValue()
    {
      var result = _converter.Convert( 10d, typeof( double ), null, CultureInfo.CurrentCulture );

      Assert.Equal( 5d, result );
    }

    [Fact]
    public void Convert_WithModifier_SubtractsModifierBeforeHalving()
    {
      var result = _converter.Convert( 10d, typeof( double ), "2", CultureInfo.CurrentCulture );

      Assert.Equal( 4d, result );
    }

    [Fact]
    public void Convert_ModifierLargerThanValue_ClampsToZero()
    {
      var result = _converter.Convert( 1d, typeof( double ), "4", CultureInfo.CurrentCulture );

      Assert.Equal( 0d, result );
    }
  }

  public class RoundedValueConverterTests
  {
    [Fact]
    public void Convert_DefaultPrecision_RoundsToWholeNumber()
    {
      var converter = new RoundedValueConverter();

      var result = converter.Convert( 1.6d, typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 2d, result );
    }

    [Fact]
    public void Convert_UsesConfiguredPrecision()
    {
      var converter = new RoundedValueConverter { Precision = 2 };

      var result = converter.Convert( 1.23456d, typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 1.23d, result );
    }

    [Fact]
    public void Convert_RoundsBothPointCoordinates()
    {
      var converter = new RoundedValueConverter { Precision = 1 };

      var result = converter.Convert( new Point( 1.24d, 3.46d ), typeof( Point ), null, CultureInfo.InvariantCulture );

      Assert.Equal( new Point( 1.2d, 3.5d ), result );
    }

    [Fact]
    public void Convert_UnsupportedType_ReturnsValueUnchanged()
    {
      var converter = new RoundedValueConverter();

      var result = converter.Convert( "unchanged", typeof( string ), null, CultureInfo.InvariantCulture );

      Assert.Equal( "unchanged", result );
    }

    [Fact]
    public void ConvertBack_ReturnsValueUnchanged()
    {
      var converter = new RoundedValueConverter { Precision = 3 };

      var result = converter.ConvertBack( 1.23456d, typeof( double ), null, CultureInfo.InvariantCulture );

      Assert.Equal( 1.23456d, result );
    }
  }
}
