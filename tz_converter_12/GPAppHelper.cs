using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace tz_converter_12
{
    public class GPAppHelper
    {
        public GPAppHelper()
        {
        }


        private static GPAppHelper sharedObject = null;

        public static GPAppHelper getShared()
        {
            if (sharedObject == null)
                sharedObject = new GPAppHelper();
            return sharedObject;
        }

        public static string getMonthAbr(int i)
        {
            return "";
        }

        public static void hoursToParts(double aHours, out int h1, out int m1)
        {
            double hours = Math.Abs(aHours);
            h1 = Convert.ToInt32(Math.Floor(hours));
            m1 = Convert.ToInt32((hours - Math.Floor(hours)) * 60);
        }


        public static int ComboMasaToMasa(int nComboMasa)
        {
            return (nComboMasa == 12) ? 12 : ((nComboMasa + 11) % 12);
        }

        public static int MasaToComboMasa(int nMasa)
        {
            return (nMasa == 12) ? 12 : ((nMasa + 1) % 12);
        }

        public static string GetNaksatraChildSylable(int n, int pada)
        {
            int i = (n * 4 + pada) % 108;
            return "";
        }

        public static string GetRasiChildSylable(int n)
        {
            return "";
        }

        public static string GetMahadvadasiName(int i)
        {
            return "";
        }

        public static string GetFastingName(int i)
        {
            return "";
        }


        public static string GetEkadasiName(int nMasa, int nPaksa)
        {
            return "";
        }

        public static string GetDSTSignature(int nDST)
        {
            return "";
        }

        public static string GetParanaReasonText(int eparana_type)
        {
            return string.Empty;
        }

        public static string GetTextLatitude(double d)
        {
            int a0, a1;
            char c0;

            c0 = d < 0.0 ? 'S' : 'N';
            d = d < 0.0 ? -d : d;
            a0 = Convert.ToInt32(d);
            a1 = Convert.ToInt32((d - a0) * 60 + 0.5);

            return string.Format("{0}{1}{2:00}", a0, c0, a1);
        }

        public static string GetTextLongitude(double d)
        {
            int a0, a1;
            char c0;

            c0 = d < 0.0 ? 'W' : 'E';
            a0 = Convert.ToInt32(Math.Abs(d));
            a1 = Convert.ToInt32((Math.Abs(d) - a0) * 60 + 0.5);

            return string.Format("{0}{1}{2:00}", a0, c0, a1);
        }

        public static string GetTextTimeZone(long aad)
        {
            int a4, a5;
            int sig = 1;
            int d = Convert.ToInt32(aad);
            if (d < 0)
            {
                sig = -1;
                d = Math.Abs(d);
            }
            a4 = Convert.ToInt32(d/3600);
            a5 = Convert.ToInt32((d - a4*3600)/60);

            return string.Format("{0}{1}:{2:00}", (sig > 0 ? '+' : '-'), a4, a5);
        }

        public static string GetTextTimeZoneArg(double d)
        {
            int a4, a5;
            int sig;

            if (d < 0.0)
            {
                sig = -1;
                d = -d;
            }
            else
            {
                sig = 1;
            }
            a4 = Convert.ToInt32(d);
            a5 = Convert.ToInt32((d - a4) * 60 + 0.5);

            return string.Format("{0}{1}{2:00}", a4, (sig > 0 ? 'E' : 'W'), a5);
        }


        public static string getShortVersionText()
        {
            return string.Format("{0} {1}", "", GPFileHelper.FileVersion);
        }

        public static string getLongVersionText()
        {
            return string.Format("{0} {1}", "", GPFileHelper.FileVersion);
        }

        public static string CenterString(string s, int width)
        {
            if (s.Length > width)
                return s;
            return s.PadLeft((s.Length + width) / 2).PadRight(width);
        }

        public static string CenterString(string s, int width, char paddingChar)
        {
            if (s.Length > width)
                return s;
            return s.PadLeft((s.Length + width) / 2, paddingChar).PadRight(width, paddingChar);
        }


        public static string MakeFilterString(params FileFormatType[] formats)
        {
            string s = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("|");
            return sb.ToString();
        }

        public static string AnnouncementTitleClass = "AnnTitle";
        public static bool AnnouncementFullWidth = true;

        public static string GenerateAnnouncementHtmlString(string title, string contentHtml, string contentId, bool display)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div style='display:{0};padding:5px;border-color:black;border-width=1px;border-style:solid;' id='{1}Main'>", (display ? "block" : "none"), contentId);
            if (title != null)
                sb.AppendFormat("  <span style='font-size:120%;font-family:Tahoma;font-weight:bold;'>{1}</span><br>&nbsp;", AnnouncementTitleClass,title);
            sb.AppendFormat("<div id='{0}'>", contentId);
            sb.AppendLine(contentHtml);
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<p></p>");

            return sb.ToString();
        }


        public static string GenerateStartupPage()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>Startup Screen</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("<!--");
            //sb.AppendLine(FormaterHtml.CurrentFormattingStyles);
            sb.AppendLine("-->");
            sb.AppendLine("</style>");
            sb.AppendLine("<script>");
            sb.AppendLine("function ShowElement(elemId,disp) { var elem = document.getElementById(elemId); elem.style.display=disp; } ");
            sb.AppendLine("</script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<table align=center><tr><td valign=top width=33%>");

            

            return sb.ToString();
        }

        public static string NextStartupTip()
        {
            return null;
        }

        public static string StringToHtmlString(string s)
        {
            return s.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
        }

    }

    public enum FileFormatType
    {
        PlainText,
        RichText,
        HtmlText,
        HtmlTable,
        Xml,
        Csv,
        Vcal,
        Ical
    }


}
