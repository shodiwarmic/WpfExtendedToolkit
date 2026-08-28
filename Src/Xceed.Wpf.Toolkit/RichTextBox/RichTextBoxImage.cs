/*************************************************************************************
   
   Toolkit for WPF

   Copyright (C) 2007-2019 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md

   For more features, controls, and fast professional support,
   pick up the Plus Edition at https://xceed.com/xceed-toolkit-plus-for-wpf/

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Documents;

namespace Xceed.Wpf.Toolkit
{
  /// <summary>
  /// Exposes the AltText attached property, which holds the alternative text ( the
  /// description ) of a picture embedded in a RichTextBox document. The property is
  /// preserved by the RtfFormatter and the XamlFormatter when the document is converted
  /// to and from the RichTextBox.Text property.
  /// </summary>
  public static class RichTextBoxImage
  {
    #region AltText

    public static readonly DependencyProperty AltTextProperty = DependencyProperty.RegisterAttached( "AltText", typeof( string ), typeof( RichTextBoxImage ), new FrameworkPropertyMetadata( null, OnAltTextChanged ) );

    public static string GetAltText( DependencyObject element )
    {
      if( element == null )
        throw new ArgumentNullException( "element" );

      return ( string )element.GetValue( RichTextBoxImage.AltTextProperty );
    }

    public static void SetAltText( DependencyObject element, string value )
    {
      if( element == null )
        throw new ArgumentNullException( "element" );

      element.SetValue( RichTextBoxImage.AltTextProperty, value );
    }

    private static void OnAltTextChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      var altText = e.NewValue as string;
      var oldAltText = e.OldValue as string;

      //Report the alt text to accessibility clients, unless the element carries a name of its own.
      object localName = d.ReadLocalValue( AutomationProperties.NameProperty );
      if( object.Equals( localName, DependencyProperty.UnsetValue ) || object.Equals( localName, oldAltText ) )
      {
        d.SetValue( AutomationProperties.NameProperty, altText ?? string.Empty );
      }
    }

    #endregion //AltText

    #region Internal Methods

    /// <summary>
    /// Returns the alt text of every picture of the document, in document order. An entry
    /// is null when the corresponding picture has no alt text.
    /// </summary>
    internal static IList<string> GetAltTexts( FlowDocument document )
    {
      IList<DependencyObject> pictures = RichTextBoxImage.GetPictures( document );
      var altTexts = new List<string>( pictures.Count );

      foreach( DependencyObject picture in pictures )
      {
        altTexts.Add( RichTextBoxImage.GetEffectiveAltText( picture ) );
      }

      return altTexts;
    }

    /// <summary>
    /// Assigns the provided alt texts to the pictures of the document. Alt texts are matched
    /// to pictures by position, so nothing is applied when the counts do not match.
    /// </summary>
    internal static void ApplyAltTexts( FlowDocument document, IList<string> altTexts )
    {
      if( ( altTexts == null ) || ( altTexts.Count == 0 ) )
        return;

      IList<DependencyObject> pictures = RichTextBoxImage.GetPictures( document );
      if( pictures.Count != altTexts.Count )
        return;

      for( int i = 0; i < pictures.Count; i++ )
      {
        if( string.IsNullOrEmpty( altTexts[ i ] ) )
          continue;

        RichTextBoxImage.SetAltText( pictures[ i ], altTexts[ i ] );
      }
    }

    #endregion //Internal Methods

    #region Private Methods

    private static string GetEffectiveAltText( DependencyObject element )
    {
      if( element == null )
        return null;

      var altText = element.GetValue( RichTextBoxImage.AltTextProperty ) as string;
      if( !string.IsNullOrEmpty( altText ) )
        return altText;

      //An alt text may also have been provided through the standard accessibility properties.
      altText = AutomationProperties.GetName( element );
      if( !string.IsNullOrEmpty( altText ) )
        return altText;

      altText = AutomationProperties.GetHelpText( element );
      return string.IsNullOrEmpty( altText ) ? null : altText;
    }

    /// <summary>
    /// Returns, in document order, the element carrying the alt text of every embedded object
    /// of the document. Every embedded object is returned—not only the images—because each of
    /// them is written as a picture when the document is saved.
    /// </summary>
    private static IList<DependencyObject> GetPictures( FlowDocument document )
    {
      var pictures = new List<DependencyObject>();

      if( document != null )
      {
        RichTextBoxImage.CollectPictures( document.Blocks, pictures );
      }

      return pictures;
    }

    private static void CollectPictures( IEnumerable<Block> blocks, List<DependencyObject> pictures )
    {
      foreach( Block block in blocks )
      {
        var blockContainer = block as BlockUIContainer;
        if( blockContainer != null )
        {
          pictures.Add( ( DependencyObject )blockContainer.Child ?? blockContainer );
          continue;
        }

        var paragraph = block as Paragraph;
        if( paragraph != null )
        {
          RichTextBoxImage.CollectPictures( paragraph.Inlines, pictures );
          continue;
        }

        var section = block as Section;
        if( section != null )
        {
          RichTextBoxImage.CollectPictures( section.Blocks, pictures );
          continue;
        }

        var list = block as List;
        if( list != null )
        {
          foreach( ListItem listItem in list.ListItems )
          {
            RichTextBoxImage.CollectPictures( listItem.Blocks, pictures );
          }
          continue;
        }

        var table = block as Table;
        if( table != null )
        {
          foreach( TableRowGroup rowGroup in table.RowGroups )
          {
            foreach( TableRow row in rowGroup.Rows )
            {
              foreach( TableCell cell in row.Cells )
              {
                RichTextBoxImage.CollectPictures( cell.Blocks, pictures );
              }
            }
          }
        }
      }
    }

    private static void CollectPictures( IEnumerable<Inline> inlines, List<DependencyObject> pictures )
    {
      foreach( Inline inline in inlines )
      {
        var inlineContainer = inline as InlineUIContainer;
        if( inlineContainer != null )
        {
          pictures.Add( ( DependencyObject )inlineContainer.Child ?? inlineContainer );
          continue;
        }

        var span = inline as Span;
        if( span != null )
        {
          RichTextBoxImage.CollectPictures( span.Inlines, pictures );
          continue;
        }

        var figure = inline as Figure;
        if( figure != null )
        {
          RichTextBoxImage.CollectPictures( figure.Blocks, pictures );
          continue;
        }

        var floater = inline as Floater;
        if( floater != null )
        {
          RichTextBoxImage.CollectPictures( floater.Blocks, pictures );
        }
      }
    }

    #endregion //Private Methods
  }
}
