using System;
using System.Globalization;
using System.Windows;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests.Converters
{
  public class WizardPageButtonVisibilityConverterTests
  {
    private readonly WizardPageButtonVisibilityConverter _converter = new WizardPageButtonVisibilityConverter();

    private object Convert( object wizardVisibility, object pageVisibility )
    {
      return _converter.Convert( new[] { wizardVisibility, pageVisibility }, typeof( Visibility ), null, CultureInfo.InvariantCulture );
    }

    [Theory]
    [InlineData( Visibility.Visible )]
    [InlineData( Visibility.Hidden )]
    [InlineData( Visibility.Collapsed )]
    public void Convert_Inherit_UsesWizardVisibility( Visibility wizardVisibility )
    {
      Assert.Equal( wizardVisibility, Convert( wizardVisibility, WizardPageButtonVisibility.Inherit ) );
    }

    [Theory]
    [InlineData( WizardPageButtonVisibility.Collapsed, Visibility.Collapsed )]
    [InlineData( WizardPageButtonVisibility.Hidden, Visibility.Hidden )]
    [InlineData( WizardPageButtonVisibility.Visible, Visibility.Visible )]
    public void Convert_ExplicitPageVisibility_OverridesWizardVisibility( WizardPageButtonVisibility pageVisibility, Visibility expected )
    {
      Assert.Equal( expected, Convert( Visibility.Visible, pageVisibility ) );
    }

    [Fact]
    public void Convert_NullWizardVisibility_FallsBackToHidden()
    {
      Assert.Equal( Visibility.Hidden, Convert( null, WizardPageButtonVisibility.Inherit ) );
    }

    [Fact]
    public void Convert_UnsetWizardVisibility_FallsBackToHidden()
    {
      Assert.Equal( Visibility.Hidden, Convert( DependencyProperty.UnsetValue, WizardPageButtonVisibility.Inherit ) );
    }

    [Fact]
    public void Convert_UnsetPageVisibility_FallsBackToHidden()
    {
      Assert.Equal( Visibility.Hidden, Convert( Visibility.Visible, DependencyProperty.UnsetValue ) );
    }

    [Fact]
    public void Convert_WrongNumberOfValues_Throws()
    {
      Assert.Throws<ArgumentException>(
        () => { _converter.Convert( new object[] { Visibility.Visible }, typeof( Visibility ), null, CultureInfo.InvariantCulture ); } );
    }

    [Fact]
    public void Convert_NullValues_Throws()
    {
      Assert.Throws<ArgumentException>(
        () => { _converter.Convert( null, typeof( Visibility ), null, CultureInfo.InvariantCulture ); } );
    }
  }

  public class ObjectTypeToNameConverterTests
  {
    private readonly ObjectTypeToNameConverter _converter = new ObjectTypeToNameConverter();

    private object Convert( object value )
    {
      return _converter.Convert( value, typeof( string ), null, CultureInfo.InvariantCulture );
    }

    [Fact]
    public void Convert_Type_ReturnsTypeName()
    {
      Assert.Equal( "String", Convert( typeof( string ) ) );
    }

    [Fact]
    public void Convert_TypeWithDisplayName_ReturnsDisplayName()
    {
      Assert.Equal( "Named Widget", Convert( typeof( NamedWidget ) ) );
    }

    [Fact]
    public void Convert_InstanceWithoutToStringOverride_ReturnsTypeName()
    {
      Assert.Equal( "PlainWidget", Convert( new PlainWidget() ) );
    }

    [Fact]
    public void Convert_InstanceWithDisplayNameAndNoToStringOverride_ReturnsDisplayName()
    {
      Assert.Equal( "Named Widget", Convert( new NamedWidget() ) );
    }

    [Fact]
    public void Convert_InstanceWithToStringOverride_ReturnsValueItself()
    {
      var widget = new DescribedWidget();

      Assert.Same( widget, Convert( widget ) );
    }

    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
      Assert.Null( Convert( null ) );
    }

    private class PlainWidget
    {
    }

    [System.ComponentModel.DisplayName( "Named Widget" )]
    private class NamedWidget
    {
    }

    private class DescribedWidget
    {
      public override string ToString()
      {
        return "a described widget";
      }
    }
  }
}
