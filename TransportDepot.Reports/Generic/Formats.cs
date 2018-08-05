using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;

namespace TransportDepot.Reports.Generic
{
  static class Formats
  {
    
    public static class Landscape
    {
      public static string HeaderLeftWidth { get { return "5cm"; } }
      public static string HeaderCenterWidth { get { return "17cm"; } }
      public static string HeaderRightWidth { get { return "5cm"; } }
      public static string FooterLeftWidth { get { return "5cm"; } }
      public static string FooterCenterWidth { get { return "17cm"; } }
      public static string FooterRightWidth { get { return "5cm"; } }
    }

    public static class Portrait
    {
      public static string HeaderLeftWidth { get { return "3cm"; } }
      public static string HeaderCenterWidth { get { return "12.5cm"; } }
      public static string HeaderRightWidth { get { return "3.6cm"; } }
      public static string FooterLeftWidth { get { return "3cm"; } }
      public static string FooterCenterWidth { get { return "12.5cm"; } }
      public static string FooterRightWidth { get { return "3.6cm"; } }
    }

    public static string GetHeaderLeftWidth(Orientation docOrientation)
    {
      return docOrientation.Equals(Orientation.Landscape) 
        ? Landscape.HeaderLeftWidth 
        : Portrait.HeaderLeftWidth;
    }

    public static string GetFooterLeftWidth(Orientation docOrientation)
    {
      return docOrientation.Equals(Orientation.Landscape)
        ? Landscape.FooterLeftWidth
        : Portrait.FooterLeftWidth;
    }

    public static string GetHeaderCenterWidth(Orientation docOrientation)
    {
      return docOrientation.Equals(Orientation.Landscape)
        ? Landscape.HeaderCenterWidth
        : Portrait.HeaderCenterWidth;
    }

    public static string GetFooterCenterWidth(Orientation docOrientation)
    {
      return docOrientation.Equals(Orientation.Landscape)
        ? Landscape.FooterCenterWidth
        : Portrait.FooterCenterWidth;
    }

    public static string GetHeaderRightWidth(Orientation docOrientation)
    {
      return docOrientation.Equals(Orientation.Landscape)
        ? Landscape.HeaderRightWidth
        : Portrait.HeaderRightWidth;
    }

    public static string GeFooterRightWidth(Orientation docOrientation)
    {
      return docOrientation.Equals(Orientation.Landscape)
        ? Landscape.FooterRightWidth
        : Portrait.FooterRightWidth;
    }
  }
}
