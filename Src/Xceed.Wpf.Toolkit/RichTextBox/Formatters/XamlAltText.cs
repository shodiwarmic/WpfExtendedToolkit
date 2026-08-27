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
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Xceed.Wpf.Toolkit
{
  /// <summary>
  /// Reads and writes the RichTextBoxImage.AltText attached property on the pictures of a Xaml
  /// document. The property is written explicitly because the Xaml conversion of a TextRange
  /// only preserves the properties it knows about.
  /// </summary>
  internal static class XamlAltText
  {
    #region Static Members

    private static readonly XName AltTextAttributeName = XName.Get(
      "RichTextBoxImage.AltText",
      "clr-namespace:Xceed.Wpf.Toolkit;assembly=" + typeof( RichTextBoxImage ).Assembly.GetName().Name );

    #endregion //Static Members

    #region Internal Methods

    /// <summary>
    /// Returns the alt text of every picture of the Xaml content, in document order. An entry
    /// is null when the corresponding picture has no alt text.
    /// </summary>
    internal static IList<string> Extract( string xaml )
    {
      var altTexts = new List<string>();

      //Parsing the whole content is only worth it when an alt text is present.
      if( string.IsNullOrEmpty( xaml ) || ( xaml.IndexOf( XamlAltText.AltTextAttributeName.LocalName, StringComparison.Ordinal ) < 0 ) )
        return altTexts;

      XDocument document = XamlAltText.Parse( xaml );
      if( document == null )
        return altTexts;

      foreach( XElement picture in XamlAltText.GetPictures( document ) )
      {
        XAttribute altText = picture.Attribute( XamlAltText.AltTextAttributeName );
        altTexts.Add( ( altText != null ) ? altText.Value : null );
      }

      return altTexts;
    }

    /// <summary>
    /// Adds the provided alt texts to the pictures of the Xaml content. Alt texts are matched to
    /// pictures by position, so the content is returned unmodified when the counts do not match.
    /// </summary>
    internal static string Insert( string xaml, IList<string> altTexts )
    {
      if( string.IsNullOrEmpty( xaml ) || !XamlAltText.HasAltText( altTexts ) )
        return xaml;

      XDocument document = XamlAltText.Parse( xaml );
      if( document == null )
        return xaml;

      IList<XElement> pictures = XamlAltText.GetPictures( document );
      if( pictures.Count != altTexts.Count )
        return xaml;

      bool modified = false;
      for( int i = 0; i < pictures.Count; i++ )
      {
        if( string.IsNullOrEmpty( altTexts[ i ] ) )
          continue;

        pictures[ i ].SetAttributeValue( XamlAltText.AltTextAttributeName, altTexts[ i ] );
        modified = true;
      }

      return modified ? document.ToString( SaveOptions.DisableFormatting ) : xaml;
    }

    #endregion //Internal Methods

    #region Private Methods

    private static bool HasAltText( IList<string> altTexts )
    {
      if( altTexts == null )
        return false;

      foreach( string altText in altTexts )
      {
        if( !string.IsNullOrEmpty( altText ) )
          return true;
      }

      return false;
    }

    private static XDocument Parse( string xaml )
    {
      if( string.IsNullOrEmpty( xaml ) )
        return null;

      try
      {
        return XDocument.Parse( xaml, LoadOptions.PreserveWhitespace );
      }
      catch( XmlException )
      {
        return null;
      }
    }

    /// <summary>
    /// Returns, in document order, the element carrying the alt text of every embedded object of
    /// the Xaml content: the child of the container when there is one, the container otherwise.
    /// </summary>
    private static IList<XElement> GetPictures( XDocument document )
    {
      var pictures = new List<XElement>();

      foreach( XElement element in document.Descendants() )
      {
        string localName = element.Name.LocalName;
        if( ( localName != "InlineUIContainer" ) && ( localName != "BlockUIContainer" ) )
          continue;

        pictures.Add( XamlAltText.GetContent( element ) ?? element );
      }

      return pictures;
    }

    private static XElement GetContent( XElement container )
    {
      XElement content = XamlAltText.GetFirstObjectElement( container );
      if( content != null )
        return content;

      //The content may have been written using the property element syntax.
      XElement childProperty = container.Elements().FirstOrDefault( element => element.Name.LocalName.EndsWith( ".Child", StringComparison.Ordinal ) );
      return ( childProperty != null ) ? XamlAltText.GetFirstObjectElement( childProperty ) : null;
    }

    private static XElement GetFirstObjectElement( XElement parent )
    {
      return parent.Elements().FirstOrDefault( element => element.Name.LocalName.IndexOf( '.' ) < 0 );
    }

    #endregion //Private Methods
  }
}
