using System;
using System.Threading;
using System.Windows.Controls;
using Xunit;

namespace Xceed.Wpf.Toolkit.Tests
{
  public class ButtonSpinnerTests
  {
    [Fact]
    public void ButtonSpinnerOrientation_DefaultsToVertical()
    {
      RunOnStaThread( () =>
      {
        var spinner = new ButtonSpinner();

        Assert.Equal( Orientation.Vertical, spinner.ButtonSpinnerOrientation );
      } );
    }

    [Fact]
    public void ButtonSpinnerOrientation_KeepsTheAssignedValue()
    {
      RunOnStaThread( () =>
      {
        var spinner = new ButtonSpinner();

        spinner.ButtonSpinnerOrientation = Orientation.Horizontal;

        Assert.Equal( Orientation.Horizontal, spinner.ButtonSpinnerOrientation );
        Assert.Equal( Orientation.Horizontal, spinner.GetValue( ButtonSpinner.ButtonSpinnerOrientationProperty ) );
      } );
    }

    [Fact]
    public void UpDownButtonSpinnerOrientation_DefaultsToVertical()
    {
      RunOnStaThread( () =>
      {
        var upDown = new DoubleUpDown();

        Assert.Equal( Orientation.Vertical, upDown.ButtonSpinnerOrientation );
      } );
    }

    [Fact]
    public void UpDownButtonSpinnerOrientation_KeepsTheAssignedValue()
    {
      RunOnStaThread( () =>
      {
        var upDown = new DoubleUpDown();

        upDown.ButtonSpinnerOrientation = Orientation.Horizontal;

        Assert.Equal( Orientation.Horizontal, upDown.ButtonSpinnerOrientation );
      } );
    }

    // WPF controls can only be created on an STA thread, and the test runner uses MTA ones.
    private static void RunOnStaThread( Action action )
    {
      Exception failure = null;

      var thread = new Thread( () =>
      {
        try
        {
          action();
        }
        catch( Exception exception )
        {
          failure = exception;
        }
      } );

      thread.SetApartmentState( ApartmentState.STA );
      thread.Start();
      thread.Join();

      if( failure != null )
        throw new InvalidOperationException( "The test body failed on the STA thread.", failure );
    }
  }
}
