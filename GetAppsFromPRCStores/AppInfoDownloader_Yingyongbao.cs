using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace ApkDownloader
{
    class AppInfoDownloader_Yingyongbao : AppInfoDownloaderBase
    {
        private string[] url_app_categories =
        {
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=122&pageContext=", //购物
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=102&pageContext=", //阅读
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=110&pageContext=", //新闻
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=103&pageContext=", //视频
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=108&pageContext=", //旅游
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=115&pageContext=", //工具
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=106&pageContext=", //社交
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=101&pageContext=", //音乐
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=119&pageContext=", //美化
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=104&pageContext=", //摄影
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=114&pageContext=", //理财
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=117&pageContext=", //系统
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=107&pageContext=", //生活
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=112&pageContext=", //出行
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=118&pageContext=", //安全
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=111&pageContext=", //教育
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=109&pageContext=", //健康
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=105&pageContext=", //娱乐
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=100&pageContext=", //儿童
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=113&pageContext=", //办公
            "http://sj.qq.com/myapp/category.htm?orgame=1&categoryId=116&pageContext=", //通讯


            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=147&pageContext=", //休闲益智
            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=121&pageContext=", //网络游戏
            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=149&pageContext=", //飞行射击
            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=144&pageContext=", //动作冒险
            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=151&pageContext=", //体育竞速
            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=148&pageContext=", //棋牌中心
            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=153&pageContext=", //经营策略
            "http://sj.qq.com/myapp/category.htm?orgame=2&categoryId=146&pageContext=" //角色扮演
        };

        public AppInfoDownloader_Yingyongbao(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("Yingyongbao downloader receive start command while already running.");
                return;
            }
            mRunning = true;
            WebClient client = getWebClient();
            foreach (string url in url_app_categories)
            {
                bool categoryDone = false;
                try
                {
                    for (int i = 0; ; i++)
                    {
                        string htmlCode = null;
                        if (!mRunning)
                        {
                            Log.info("Yingyongbao downloader stop.");
                            return;
                        }
                        string u = url + i * 30;
                        Log.info("Fetch web page: " + u);
                        htmlCode = client.DownloadString(u);
                        //htmlCode = HtmlContentParser.toUtfString(htmlCode);
                        Log.info("Parse app info from web page: " + u);
                        string[] appInfos = htmlCode.Split(new string[] { "\"app-info clearfix\"" }, StringSplitOptions.None);

                        // if (appInfos.Length <= 1)
                        // {
                        //     Log.info("App parser get empty list, Continue with next category.");
                        //     break;
                        //  }
                        if (appInfos.Length > 1)
                        {
                            foreach (string apkInfo in appInfos)
                            {
                                if (appInfos[0] == apkInfo)
                                {
                                    continue;
                                }
                                AppInfo newAppInfo = new AppInfo();
                                newAppInfo.app_name = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "name ofh", true);
                                newAppInfo.apk_name = generateApkName(newAppInfo.app_name);
                                //newAppInfo.app_version = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "versionName", true);
                                //newAppInfo.category = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "categoryName", true);
                                //newAppInfo.company = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "authorName", true);
                                //int time = Int32.Parse(HtmlContentParser.getValueFromHtmlPiece(apkInfo, "apkPublishTime", true));
                                //newAppInfo.datePublished = (date1970 + TimeSpan.FromSeconds(time)).ToShortDateString();
                                newAppInfo.downloads_store = HtmlContentParser.downloadNumberFromString
                                    (HtmlContentParser.tryParseDownloadString(HtmlContentParser.getValueFromHtmlPiece(apkInfo, "download", true)));
                                newAppInfo.download_url_store = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "ex_url", true);
                                newAppInfo.package_name = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "apk", true);
                                newAppInfo.size = HtmlContentParser.apkSizeFromString(HtmlContentParser.getValueFromHtmlPiece(apkInfo, "size", true));
                                newAppInfo.isSoft = url.Contains("orgame=1");
                                newAppInfo.mStore = AppInfo.Store.Yingyongbao;
                                //newAppInfo.id = appId;
                                newAppInfoFetched(newAppInfo);
                            }
                           
                        }

                        if (categoryDone)
                        {
                            Log.info("App parser come into same app id, Continue with next page.");
                            break;
                        }
                        if (i >= 60)
                        {
                            break;
                        }

                    }// end for i

                }// end try
                catch (Exception e)
                {
                    Log.error("Yingyongbao downloader error, Need fix by code change!");
                    Log.error(e.Message);
                    Log.error(e.StackTrace);
                }// end catch
            }
            done();

        }

        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("Yingyongbao downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }

        public override void downloadBatchAppInfo(object inf)
        {
            mBatchDownloading = true;
            List<AppInfo> list = (List<AppInfo>)inf;
            Log.info("Yingyongbao downloader start batch download " + list.Count + " apps.");
            foreach (AppInfo info in list)
            {
                if (info.downloadSuccess)
                {
                    continue;
                }
                downloadSingleAppInfo(info);
            }
            mBatchDownloading = false;
            return;
        }

        public override bool downloadSingleAppInfo(AppInfo info)
        {
            WebClient client = getWebClient();
            string htmlCode = null;
            try
            {
                htmlCode = client.DownloadString("http://sj.qq.com/myapp/detail.htm?apkName=" + info.package_name);
            }
            catch (WebException)
            {
                //Log.info(info.package_name + " not found in tencent store.");
                return false;
            }
            //htmlCode = HtmlContentParser.toUtfString(htmlCode);

            int index_start = htmlCode.IndexOf("\"appInfo\"");
            int index_end = htmlCode.IndexOf("查看权限");
            if (index_start < 0 || index_end < 0)
            {
                //Log.info(info.package_name + " not found in tencent store.");
                return false;
            }
            htmlCode = htmlCode.Substring(index_start, index_end);
            lock (info)
            {
                if (info.apk_name == null || info.apk_name.Length < 1)
                {
                    info.apk_name = generateApkName(info.app_name);
                }
                info.app_version = HtmlContentParser.getValueFromHtmlPiece(htmlCode, "det-othinfo-data", true);
                info.company = HtmlContentParser.getValueFromHtmlPiece(htmlCode, "det-othinfo-data", true);
                int time = int.Parse(HtmlContentParser.getValueFromHtmlPiece(htmlCode, "data-apkPublishTime", true));
                info.datePublished = (date1970 + TimeSpan.FromSeconds(time)).ToShortDateString();

                // <div class="det-ins-num">8.2亿下载</div>

                string downloadString = HtmlContentParser.getValueFromHtmlPiece(htmlCode, "det-ins-num", true);
                downloadString = downloadString.Substring(0, downloadString.IndexOf("下"));
                info.downloads_store = HtmlContentParser.downloadNumberFromString(downloadString);

                info.download_url_yingyongbao = HtmlContentParser.getValueFromHtmlPiece(htmlCode, "data-apkUrl", true);
                info.download_url_store = info.download_url_yingyongbao;
                string sizeString = HtmlContentParser.getValueFromHtmlPiece(htmlCode, "det-size", true);
                info.size = HtmlContentParser.apkSizeFromString(sizeString);
                info.mStore = AppInfo.Store.Yingyongbao;

                if (info.download_url_store == null || info.download_url_store.Length <= 0)
                {
                    //Log.info(info.package_name + " not found in tencent store.");
                    return false;
                }
            }
            Log.info(info.package_name + " fetched from tencent store.");
            return true;
        }
        public override void downloadTopUsageAppInfo()
        {

            List<AppInfo> topUsage = Config.getTopUsageList();
            foreach (AppInfo info in topUsage)
            {
                if (!mRunning)
                {
                    Log.info("Yingyongbao downloader stoped.");
                    return;
                }
                if (findAppInfoInTotalList(info.package_name) != null)
                {
                    continue;
                }
               
                if(downloadSingleAppInfo(info))
                {
                    newAppInfoFetched(info);
                }
            }
        }
    }
}
