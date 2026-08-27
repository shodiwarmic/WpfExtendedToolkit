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
using System.Globalization;
using System.Text;

namespace Xceed.Wpf.Toolkit
{
  /// <summary>
  /// Reads and writes the alt text of the pictures of an RTF document. The alt text is stored,
  /// like Word does, in the shape property named "wzDescription" of the picture:
  /// {\pict{\*\picprop{\sp{\sn wzDescription}{\sv My alt text}}}...}
  /// The RTF conversion of WPF ignores those properties, so they are extracted before a
  /// document is loaded and injected back after a document is saved.
  /// </summary>
  internal static class RtfAltText
  {
    #region Constants

    private const string DescriptionPropertyName = "wzDescription";
    private const int NoParameter = int.MinValue;

    #endregion //Constants

    #region Internal Methods

    /// <summary>
    /// Returns the alt text of every picture of the RTF content, in document order. An entry
    /// is null when the corresponding picture has no alt text.
    /// </summary>
    internal static IList<string> Extract( string rtf )
    {
      var altTexts = new List<string>();

      //Scanning the whole content, pictures included, is only worth it when an alt text is present.
      if( string.IsNullOrEmpty( rtf ) || ( rtf.IndexOf( RtfAltText.DescriptionPropertyName, StringComparison.Ordinal ) < 0 ) )
        return altTexts;

      foreach( PictureGroup pictureGroup in RtfAltText.FindPictureGroups( rtf ) )
      {
        altTexts.Add( RtfAltText.ReadAltText( rtf, pictureGroup.ContentStart, pictureGroup.End ) );
      }

      return altTexts;
    }

    /// <summary>
    /// Adds the provided alt texts to the pictures of the RTF content. Alt texts are matched to
    /// pictures by position, so the content is returned unmodified when the counts do not match.
    /// </summary>
    internal static string Insert( string rtf, IList<string> altTexts )
    {
      if( string.IsNullOrEmpty( rtf ) || !RtfAltText.HasAltText( altTexts ) )
        return rtf;

      IList<PictureGroup> pictureGroups = RtfAltText.FindPictureGroups( rtf );
      if( pictureGroups.Count != altTexts.Count )
        return rtf;

      StringBuilder result = null;
      int position = 0;

      for( int i = 0; i < pictureGroups.Count; i++ )
      {
        if( string.IsNullOrEmpty( altTexts[ i ] ) )
          continue;

        if( result == null )
        {
          result = new StringBuilder( rtf.Length + 64 );
        }

        int contentStart = pictureGroups[ i ].ContentStart;
        result.Append( rtf, position, contentStart - position );
        result.Append( @"{\*\picprop{\sp{\sn " )
              .Append( RtfAltText.DescriptionPropertyName )
              .Append( @"}{\sv " )
              .Append( RtfAltText.Escape( altTexts[ i ] ) )
              .Append( "}}}" );
        position = contentStart;
      }

      if( result == null )
        return rtf;

      result.Append( rtf, position, rtf.Length - position );
      return result.ToString();
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

    private static IList<PictureGroup> FindPictureGroups( string rtf )
    {
      var pictureGroups = new List<PictureGroup>();
      int index = 0;

      while( index < rtf.Length )
      {
        if( rtf[ index ] != '\\' )
        {
          index++;
          continue;
        }

        string controlWord;
        int parameter;
        int next = RtfAltText.ReadControlWord( rtf, index, out controlWord, out parameter );

        if( controlWord == "pict" )
        {
          int end = RtfAltText.FindGroupEnd( rtf, next );
          pictureGroups.Add( new PictureGroup( next, end ) );
          index = end;
        }
        else if( controlWord == "nonshppict" )
        {
          //Duplicate, in the older format, of the picture written by the preceding \shppict group.
          index = RtfAltText.FindGroupEnd( rtf, next );
        }
        else if( ( controlWord == "bin" ) && ( parameter > 0 ) )
        {
          index = Math.Min( rtf.Length, next + parameter );
        }
        else
        {
          index = next;
        }
      }

      return pictureGroups;
    }

    private static string ReadAltText( string rtf, int start, int end )
    {
      int nameIndex = rtf.IndexOf( RtfAltText.DescriptionPropertyName, start, end - start, StringComparison.Ordinal );
      if( nameIndex < 0 )
        return null;

      int valueIndex = rtf.IndexOf( @"{\sv", nameIndex, end - nameIndex, StringComparison.Ordinal );
      if( valueIndex < 0 )
        return null;

      int valueStart = valueIndex + 4;
      if( ( valueStart < end ) && RtfAltText.IsAsciiLetter( rtf[ valueStart ] ) )
        return null;

      //The space following a control word is its delimiter and is not part of the value.
      if( ( valueStart < end ) && ( rtf[ valueStart ] == ' ' ) )
      {
        valueStart++;
      }

      int valueEnd = RtfAltText.FindGroupEnd( rtf, valueStart ) - 1;
      if( ( valueEnd <= valueStart ) || ( valueEnd > end ) )
        return null;

      return RtfAltText.Unescape( rtf.Substring( valueStart, valueEnd - valueStart ) );
    }

    /// <summary>
    /// Returns the index following the closing brace of the group started before the provided index.
    /// </summary>
    private static int FindGroupEnd( string rtf, int index )
    {
      int depth = 1;

      while( index < rtf.Length )
      {
        char character = rtf[ index ];

        if( character == '\\' )
        {
          string controlWord;
          int parameter;
          int next = RtfAltText.ReadControlWord( rtf, index, out controlWord, out parameter );
          index = ( ( controlWord == "bin" ) && ( parameter > 0 ) ) ? Math.Min( rtf.Length, next + parameter ) : next;
          continue;
        }

        if( character == '{' )
        {
          depth++;
        }
        else if( character == '}' )
        {
          depth--;
          if( depth == 0 )
            return index + 1;
        }

        index++;
      }

      return rtf.Length;
    }

    /// <summary>
    /// Reads the control word starting at the provided index and returns the index of the first
    /// character following it. The control word is null when the backslash introduces an escaped
    /// character rather than a control word.
    /// </summary>
    private static int ReadControlWord( string rtf, int index, out string controlWord, out int parameter )
    {
      controlWord = null;
      parameter = RtfAltText.NoParameter;

      int position = index + 1;
      if( ( position >= rtf.Length ) || !RtfAltText.IsAsciiLetter( rtf[ position ] ) )
        return Math.Min( rtf.Length, index + 2 );

      int wordStart = position;
      while( ( position < rtf.Length ) && RtfAltText.IsAsciiLetter( rtf[ position ] ) )
      {
        position++;
      }
      controlWord = rtf.Substring( wordStart, position - wordStart );

      int parameterStart = position;
      if( ( position < rtf.Length ) && ( rtf[ position ] == '-' ) )
      {
        position++;
      }
      while( ( position < rtf.Length ) && ( rtf[ position ] >= '0' ) && ( rtf[ position ] <= '9' ) )
      {
        position++;
      }

      if( position > parameterStart )
      {
        int value;
        if( int.TryParse( rtf.Substring( parameterStart, position - parameterStart ), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value ) )
        {
          parameter = value;
        }
      }

      if( ( position < rtf.Length ) && ( rtf[ position ] == ' ' ) )
      {
        position++;
      }

      return position;
    }

    private static string Escape( string value )
    {
      var result = new StringBuilder( value.Length + 8 );

      foreach( char character in value )
      {
        if( ( character == '\\' ) || ( character == '{' ) || ( character == '}' ) )
        {
          result.Append( '\\' ).Append( character );
        }
        else if( character == '\r' )
        {
          continue;
        }
        else if( character == '\n' )
        {
          result.Append( @"\line " );
        }
        else if( character == '\t' )
        {
          result.Append( @"\tab " );
        }
        else if( ( character >= ' ' ) && ( character <= '~' ) )
        {
          result.Append( character );
        }
        else
        {
          //RTF stores a Unicode character as a signed 16 bit value followed by an ANSI fallback character.
          result.Append( @"\u" ).Append( ( ( short )character ).ToString( CultureInfo.InvariantCulture ) ).Append( " ?" );
        }
      }

      return result.ToString();
    }

    private static string Unescape( string value )
    {
      var result = new StringBuilder( value.Length );
      int index = 0;

      while( index < value.Length )
      {
        char character = value[ index ];

        if( ( character == '\r' ) || ( character == '\n' ) )
        {
          //Line breaks of the RTF file itself are not part of the text.
          index++;
          continue;
        }

        if( character != '\\' )
        {
          result.Append( character );
          index++;
          continue;
        }

        if( index + 1 >= value.Length )
          break;

        char next = value[ index + 1 ];
        if( ( next == '\\' ) || ( next == '{' ) || ( next == '}' ) )
        {
          result.Append( next );
          index += 2;
          continue;
        }

        if( next == '\'' )
        {
          int code;
          if( ( index + 3 < value.Length )
            && int.TryParse( value.Substring( index + 2, 2 ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code ) )
          {
            result.Append( ( char )code );
            index += 4;
          }
          else
          {
            index += 2;
          }
          continue;
        }

        string controlWord;
        int parameter;
        int position = RtfAltText.ReadControlWord( value, index, out controlWord, out parameter );

        if( controlWord == "u" )
        {
          if( parameter != RtfAltText.NoParameter )
          {
            result.Append( ( char )( ushort )parameter );
          }
          index = RtfAltText.SkipUnicodeFallback( value, position );
          continue;
        }

        if( ( controlWord == "line" ) || ( controlWord == "par" ) )
        {
          result.Append( '\n' );
        }
        else if( controlWord == "tab" )
        {
          result.Append( '\t' );
        }

        index = position;
      }

      return result.ToString();
    }

    private static int SkipUnicodeFallback( string value, int index )
    {
      if( index >= value.Length )
        return index;

      if( ( value[ index ] == '\\' ) && ( index + 3 < value.Length ) && ( value[ index + 1 ] == '\'' ) )
        return index + 4;

      if( ( value[ index ] != '\\' ) && ( value[ index ] != '{' ) && ( value[ index ] != '}' ) )
        return index + 1;

      return index;
    }

    private static bool IsAsciiLetter( char character )
    {
      return ( ( character >= 'a' ) && ( character <= 'z' ) ) || ( ( character >= 'A' ) && ( character <= 'Z' ) );
    }

    #endregion //Private Methods

    #region PictureGroup Private Class

    /// <summary>
    /// Location of the content of a {\pict} group: ContentStart follows the \pict control word
    /// and End follows the closing brace of the group.
    /// </summary>
    private class PictureGroup
    {
      internal PictureGroup( int contentStart, int end )
      {
        this.ContentStart = contentStart;
        this.End = end;
      }

      internal int ContentStart
      {
        get;
        private set;
      }

      internal int End
      {
        get;
        private set;
      }
    }

    #endregion //PictureGroup Private Class
  }
}
