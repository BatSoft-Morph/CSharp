using System;
using System.Text;

namespace Morph.Lib
{
  public static class Conversion
  {
    #region Date/Time

    //  DateTime encoded according to http://www.w3.org/TR/xmlschema-2/#dateTime

    static private void AppendFullNumber(StringBuilder Builder, int Number, int Length)
    {
      string Str = Number.ToString();
      for (int i = Length - Str.Length; i > 0; i--)
        Builder.Append('0');
      Builder.Append(Number);
    }

    static public string DateTimeToStr(DateTime When)
    {
      StringBuilder builder = new StringBuilder(21);
      builder.Append(When.Year);
      builder.Append('-');      
      AppendFullNumber(builder, When.Month, 2);
      builder.Append('-');
      AppendFullNumber(builder, When.Day, 2);
      builder.Append('T');
      AppendFullNumber(builder, When.Hour, 2);
      builder.Append(':');
      AppendFullNumber(builder, When.Minute, 2);
      builder.Append(':');
      AppendFullNumber(builder, When.Second, 2);
      if (When.Kind == DateTimeKind.Local)
        builder.Append('Z');
      return builder.ToString();
    }

    static public DateTime StrToDateTime(string When)
    {
      StringParser Parser = new StringParser(When);
      //  Year (could be negative)
      int Year;
      string YearStr = Parser.ReadTo("-", true);
      if (YearStr == null)
        Year = -Int32.Parse(Parser.ReadTo("-", true));
      else
        Year = Int32.Parse(YearStr);
      //  Read the rest of the date/time
      int Month = Int32.Parse(Parser.ReadTo("-", true));
      int Day = Int32.Parse(Parser.ReadTo("T", true));
      int Hour = Int32.Parse(Parser.ReadTo(":", true));
      int Minute = Int32.Parse(Parser.ReadTo(":", true));
      int Second = Int32.Parse(Parser.ReadDigits());
      //  Milliseconds
      int MS = 0;
      if (!Parser.IsEnded())
        if (Parser.Current() == '.')
        {
          Parser.Move(1);
          MS = Int32.Parse(Parser.ReadDigits());
        }
      //  Time zone
      DateTimeKind Kind;
      if (Parser.IsEnded())
        Kind = DateTimeKind.Utc;
      else if (Parser.Current() == 'Z')
        Kind = DateTimeKind.Local;
      else
      {
        DateTime Result = new DateTime(Year, Month, Day, Hour, Minute, Second, MS, new System.Globalization.GregorianCalendar(), DateTimeKind.Utc);
        int TZHour = Int32.Parse(Parser.ReadTo(":", true));
        int TZMinute = Int32.Parse(Parser.ReadToEnd());
        Result.AddHours(-TZHour);
        Result.AddMinutes(-TZMinute);
        return Result;
      }
      return new DateTime(Year, Month, Day, Hour, Minute, Second, MS, new System.Globalization.GregorianCalendar(), Kind);
    }

    #endregion
  }
}