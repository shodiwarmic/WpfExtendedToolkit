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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace Xceed.Wpf.Toolkit
{
  /// <summary>
  /// Formats the RichTextBox text as RTF
  /// </summary>
  public class RtfFormatter : ITextFormatter
  {
    public string GetText( FlowDocument document )
    {
      TextRange tr = new TextRange( document.ContentStart, document.ContentEnd );
      using( MemoryStream ms = new MemoryStream() )
      {
        tr.Save( ms, DataFormats.Rtf );
        var rtf = ASCIIEncoding.Default.GetString( ms.ToArray() );
        //the RTF conversion of WPF does not write the alt text of the pictures: add it back.
        return RtfAltText.Insert( rtf, RichTextBoxImage.GetAltTexts( document ) );
      }
    }

    public void SetText( FlowDocument document, string text )
    {
      //the RTF conversion of WPF drops the alt text of the pictures: read it before loading
      //the document so that it can be restored once the pictures have been created.
      IList<string> altTexts = RtfAltText.Extract( text );

      try
      {
        //if the text is null/empty clear the contents of the RTB. If you were to pass a null/empty string
        //to the TextRange.Load method an exception would occur.
        if( String.IsNullOrEmpty( text ) )
        {
          document.Blocks.Clear();
        }
        else
        {
          TextRange tr = new TextRange( document.ContentStart, document.ContentEnd );
          using( MemoryStream ms = new MemoryStream( Encoding.ASCII.GetBytes( text ) ) )
          {
            tr.Load( ms, DataFormats.Rtf );
          }
        }
      }
      catch
      {
        throw new InvalidDataException( "Data provided is not in the correct RTF format." );
      }

      RichTextBoxImage.ApplyAltTexts( document, altTexts );
    }
  }
}
