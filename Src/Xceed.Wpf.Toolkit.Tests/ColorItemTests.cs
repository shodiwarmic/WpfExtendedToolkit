using System.Windows.Media;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests
{
  public class ColorItemTests
  {
    [Fact]
    public void Constructor_AssignsColorAndName()
    {
      var item = new ColorItem( Colors.Red, "Red" );

      Assert.Equal( Colors.Red, item.Color );
      Assert.Equal( "Red", item.Name );
    }

    [Fact]
    public void Equals_SameColorAndName_IsTrue()
    {
      var first = new ColorItem( Colors.Red, "Red" );
      var second = new ColorItem( Colors.Red, "Red" );

      Assert.True( first.Equals( second ) );
      Assert.Equal( first.GetHashCode(), second.GetHashCode() );
    }

    [Fact]
    public void Equals_DifferentColor_IsFalse()
    {
      var first = new ColorItem( Colors.Red, "Red" );
      var second = new ColorItem( Colors.Blue, "Red" );

      Assert.False( first.Equals( second ) );
    }

    [Fact]
    public void Equals_DifferentName_IsFalse()
    {
      var first = new ColorItem( Colors.Red, "Red" );
      var second = new ColorItem( Colors.Red, "Crimson" );

      Assert.False( first.Equals( second ) );
    }

    [Fact]
    public void Equals_NullColorOnBothItems_IsTrue()
    {
      var first = new ColorItem( null, "None" );
      var second = new ColorItem( null, "None" );

      Assert.True( first.Equals( second ) );
    }

    [Fact]
    public void Equals_OtherType_IsFalse()
    {
      var item = new ColorItem( Colors.Red, "Red" );

      Assert.False( item.Equals( "Red" ) );
      Assert.False( item.Equals( null ) );
    }
  }
}
