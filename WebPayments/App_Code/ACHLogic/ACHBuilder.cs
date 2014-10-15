using System;


/// <summary>
/// Summary description for ACHBuilder
/// </summary>
public class ACHBuilder
{

  protected string ACHDecimalToString(decimal amount, int length)
  {
    return ACHIntToString(Convert.ToInt32(decimal.Multiply(amount, 100)), length);
  }

  protected string ACHIntToString(int value, int length)
  {
    string format = string.Format("{{0:D{0}}}", length);
    return string.Format(format, value);
  }

  protected string ACHIntToString(ulong value, int length)
  {
    string format = string.Format("{{0:D{0}}}", length);
    return string.Format(format, value);
  }

  protected string ACHDateYYMMDD(DateTime date)
  {
    return string.Format("{0:yyMMdd}", date);
  }

  protected string ACHTimeHHMM(DateTime date)
  { 
    return string.Format("{0:HHmm}", date);
  }

  protected string ACHStandardizeString(string str, int total_width, char right_filler, char left_filler)
  {
    str = str.ToUpper();
    if (str.Length > total_width)
    {
      return str.Substring(0, total_width);
    }
    else if (right_filler == ACHBuilder.EMPTY_CHAR)
    {
      return str.PadLeft(total_width, left_filler);
    }
    else if (left_filler == ACHBuilder.EMPTY_CHAR)
    { 
      return str.PadRight(total_width, right_filler);
    }
    return str;
  }


  public const char EMPTY_CHAR = '\0';
  public const int NO_TRACE_NUMBER_ASSIGNED = -1;
}