using System;

namespace Morph.Lib
{
  public class StringParser
  {
    public StringParser(string Str)
    {
      _Str = Str;
      _Len = _Str.Length;
    }

    private string _Str;
    private int _Len;
    private int _Pos = 0;

    public int Position
    {
      get { return _Pos; }
      set { _Pos = value; }
    }

    private void Validate()
    {
      if (_Pos < 0)
        throw new StringParserException("Position must not be negative.");
      if (_Pos >= _Len)
        throw new StringParserException("End of string has been reached.");
    }

    public char Current()
    {
      return _Str[_Pos];
    }

    public string Current(int Length)
    {
      Validate();
      return _Str.Substring(_Pos, Length);
    }

    public void Move(int Steps)
    {
      Validate();
      _Pos += Steps;
    }

    public void MoveTo(string SubStr, bool Absorb)
    {
      Validate();
      _Pos = _Str.IndexOf(SubStr, _Pos);
      if (Absorb)
        _Pos += SubStr.Length;
    }

    public string ReadTo(string SubStr, bool Absorb)
    {
      Validate();
      int OldPos = _Pos;
      _Pos = _Str.IndexOf(SubStr, OldPos);
      int SubStrLen = _Pos - OldPos;
      if (Absorb)
        _Pos += SubStr.Length;
      if (SubStrLen > 0)
        return _Str.Substring(OldPos, SubStrLen);
      return null;
    }

    public string ReadTo(char[] chars, bool Absorb)
    {
      Validate();
      int OldPos = _Pos;
      int NewPos = Int32.MaxValue;
      foreach (char c in chars)
      {
        int Pos = _Str.IndexOf(c, OldPos);
        if (NewPos < Pos)
          Pos = NewPos;
      }
      if (NewPos == Int32.MaxValue)
        return null;
      int SubStrLen = _Pos - OldPos;
      if (Absorb)
        _Pos++;
      if (SubStrLen > 0)
        return _Str.Substring(OldPos, SubStrLen);
      return null;
    }

    private bool CharInChars(char c, char[] chars)
    {
      foreach (char e in chars)
        if (c == e)
          return true;
      return false;
    }

    public string ReadChars(char[] chars)
    {
      Validate();
      int OldPos = _Pos;
      while ((_Pos < _Len) && CharInChars(_Str[_Pos], chars))
        _Pos++;
      if (OldPos == _Pos)
        return null;
      return _Str.Substring(OldPos, _Pos - OldPos);
    }

    public bool ReadChar(char Char)
    {
      Validate();
      if (_Str[_Pos] != Char)
        return false;
      _Pos++;
      return true;
    }

    static private char[] Digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    public string ReadDigits()
    {
      return ReadChars(Digits);
    }

    public string ReadToEnd()
    {
      return _Str.Substring(_Pos);
    }

    public bool IsEnded()
    {
      return _Pos >= _Len;
    }
  }

  public class StringParserException : Exception
  {
    public StringParserException(string Message)
      : base(Message)
    {
    }
  }
}