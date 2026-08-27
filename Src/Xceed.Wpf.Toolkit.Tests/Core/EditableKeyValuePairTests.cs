using System.ComponentModel;
using System.Linq;
using Xceed.Wpf.Toolkit.Core;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests.Core
{
  public class EditableKeyValuePairTests
  {
    [Fact]
    public void Constructor_AssignsKeyAndValue()
    {
      var pair = new EditableKeyValuePair<string, int>( "answer", 42 );

      Assert.Equal( "answer", pair.Key );
      Assert.Equal( 42, pair.Value );
    }

    [Fact]
    public void ToString_UsesBracketedPairFormat()
    {
      var pair = new EditableKeyValuePair<string, int>( "answer", 42 );

      Assert.Equal( "[answer,42]", pair.ToString() );
    }

    [Fact]
    public void GetProperties_ExposesKeyAndValueForTheCollectionEditor()
    {
      var pair = new EditableKeyValuePair<string, int>( "answer", 42 );

      var propertyNames = pair.GetProperties().Cast<PropertyDescriptor>().Select( p => p.Name ).ToList();

      Assert.Equal( new[] { "Key", "Value" }, propertyNames );
    }

    [Fact]
    public void GetProperties_WithAttributeFilter_ReturnsTheSameProperties()
    {
      var pair = new EditableKeyValuePair<string, int>( "answer", 42 );

      Assert.Equal( pair.GetProperties().Count, pair.GetProperties( new System.Attribute[ 0 ] ).Count );
    }

    [Fact]
    public void GetPropertyOwner_ReturnsThePairItself()
    {
      var pair = new EditableKeyValuePair<string, int>( "answer", 42 );

      Assert.Same( pair, pair.GetPropertyOwner( null ) );
    }

    [Fact]
    public void ParameterlessConstructor_IsSupportedForNewItems()
    {
      var pair = new EditableKeyValuePair<string, int>();

      Assert.Null( pair.Key );
      Assert.Equal( 0, pair.Value );
    }
  }
}
