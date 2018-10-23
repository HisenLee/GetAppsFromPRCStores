using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApkDownloader
{
    class HtmlContentParser
    {

        // get web title from web string
        public static string getWebTitle(string webContent)
        {
            int start = webContent.IndexOf("title") + "title".Length;
            int end = webContent.IndexOf("title", start);
            string t1 = webContent.Substring(start, end - start);
            string t2 = t1.Substring(t1.IndexOf('>') + 1, t1.IndexOf('<') - t1.IndexOf('>') - 1);
            return t2.Trim();
        }

        //public static string toUtfString(string orig)
        //{
            //if (Encoding.Default != Encoding.UTF8)
            //{
                //byte[] bytes = Encoding.Default.GetBytes(orig);
                //return Encoding.UTF8.GetString(bytes);
            //}
            //return orig;
        //}

        //e.g.  x.xMB -> bytes x.xGB -> bytes
        public static int apkSizeFromString(string size)
        {
            int sizeInteger = -1;

            try
            {
                if (size.EndsWith("M") || size.EndsWith("K") || size.EndsWith("G"))
                {
                    size = size + "B";
                }
                if (size.Contains("MB"))
                {
                    if (size.Contains("."))
                    {
                        int intgr = int.Parse(size.Substring(0, size.IndexOf(".")).Replace(",", ""));
                        int dt = int.Parse(size.Substring(size.IndexOf(".") + 1, size.IndexOf("MB") - size.IndexOf(".") - 1));
                        sizeInteger = (intgr + dt / 100) * 1024 * 1024;
                    }
                    else
                    {
                        sizeInteger = int.Parse(size.Substring(0, size.IndexOf("MB")).Replace(",", "")) * 1024 * 1024;
                    }
                }
                else if (size.Contains("KB"))
                {
                    if (size.Contains("."))
                    {
                        int intgr = int.Parse(size.Substring(0, size.IndexOf(".")).Replace(",", ""));
                        int dt = int.Parse(size.Substring(size.IndexOf(".") + 1, size.IndexOf("KB") - size.IndexOf(".") - 1));
                        sizeInteger = (intgr + dt / 100) * 1024;
                    }
                    else
                    {
                        sizeInteger = int.Parse(size.Substring(0, size.IndexOf("KB")).Replace(",", "")) * 1024;
                    }
                }
                else if (size.Contains("GB"))
                {
                    if (size.Contains("."))
                    {
                        int intgr = int.Parse(size.Substring(0, size.IndexOf(".")).Replace(",", ""));
                        int dt = int.Parse(size.Substring(size.IndexOf(".") + 1, size.IndexOf("GB") - size.IndexOf(".") - 1));
                        sizeInteger = (intgr + dt / 100) * 1024 * 1024 * 1024;
                    }
                    else
                    {
                        sizeInteger = int.Parse(size.Substring(0, size.IndexOf("GB")).Replace(",", "")) * 1024 * 1024 * 1024;
                    }
                }
                else
                {
                    sizeInteger = int.Parse(size);
                }
            } // end try
            catch (Exception ex)
            {
                sizeInteger = -1;
                Log.info("HtmlContentParser.apkSizeFromString parse error, Need fix by code change!");
                Log.info(ex.Message);
                Log.info(ex.StackTrace);
            }            
            return sizeInteger;
        }

        // 下载1423万次
        public static string tryParseDownloadString(string download)
        {
            int index1 = download.IndexOf("载");
            int index2 = download.IndexOf("次");
            string result = download.Substring(index1 + 1, index2 - index1 -1);
            return result;
        }

        // x.x万/亿 to long
        public static long downloadNumberFromString(string downloads)
        {
            if (downloads.Contains("万"))
            {
                if (downloads.Contains("."))
                {
                    long intgr = long.Parse(downloads.Substring(0, downloads.IndexOf(".")));
                    long dt = long.Parse(downloads.Substring(downloads.IndexOf(".") + 1, downloads.IndexOf("万") - downloads.IndexOf(".") - 1));
                    return intgr * 10000 + dt * 1000;
                }
                return long.Parse(downloads.Substring(0, downloads.IndexOf("万"))) * 10000;
            }
            if (downloads.Contains("亿"))
            {
                if (downloads.Contains("."))
                {
                    long intgr = long.Parse(downloads.Substring(0, downloads.IndexOf(".")));
                    long dt = long.Parse(downloads.Substring(downloads.IndexOf(".") + 1, downloads.IndexOf("亿") - downloads.IndexOf(".") - 1));
                    return intgr * 10000 * 10000 + dt * 10000 * 1000;
                }
                else
                {
                    return long.Parse(downloads.Substring(0, downloads.IndexOf("亿"))) * 10000 * 10000;
                }
            }
            if (downloads.Contains("千"))
            {
                return long.Parse(downloads.Substring(0, downloads.IndexOf("千"))) * 1000;
            }
            return long.Parse(downloads);
        }

        // try every possiblity to get a target value by key from html piece;
        //data-install="1456 万" data-like="135"
        public static string getValueFromHtmlPiece(string htmlPiece, string keyWord, bool throwException)
        {
            string result = null;

            //e.g. <span class="install-count">7270 人安装</span>
            string target = keyWord + "\">";
            int start = htmlPiece.IndexOf(target);
            if (start >= 0)
            {
                start = start + target.Length;
                int end = htmlPiece.IndexOf("</", start);
                result = htmlPiece.Substring(start, end - start);
                return result;
            }

            //e.g. class="name"
            target = keyWord + "=\"";
            start = htmlPiece.IndexOf(target);
            if (start >= 0)
            {
                start = start + target.Length;
                int end = htmlPiece.IndexOf('\"', start);
                result = htmlPiece.Substring(start, end - start);
                return result;
            }

            // e.g. "appDownCount":28237562
            target = "\"" + keyWord + "\":";
            start = htmlPiece.IndexOf(target);
            if (start >= 0)
            {
                start = start + target.Length;
                int end = htmlPiece.IndexOf(',', start);
                result = htmlPiece.Substring(start, end - start);
                if (result.Contains('"'))
                {
                    result = result.Substring(1, result.Length - 2);
                }
                return result;
            }

            if (result == null && throwException)
            {
                //Log.info("getValueFromHtmlPiece return null.");
                //Log.info("keyword: " + keyWord);
                //Log.writeError("html piece: " + htmlPiece);
                throw new KeyNotFoundException();
            }
            return result;
        }
    }
}
