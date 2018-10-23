using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ApkDownloader
{
    class AppInfoDownloader_Baidu : AppInfoDownloaderBase
    {
        private string[] url_app_categories =
        {
            "http://shouji.baidu.com/software/503/", //社交通讯
            "http://shouji.baidu.com/software/501/", //系统工具
            "http://shouji.baidu.com/software/510/", //理财购物
            "http://shouji.baidu.com/software/502/", //主题壁纸
            "http://shouji.baidu.com/software/509/", //旅游出行
            "http://shouji.baidu.com/software/506/", //影音播放
            "http://shouji.baidu.com/software/508/", //拍摄美化
            "http://shouji.baidu.com/software/504/", //生活实用
            "http://shouji.baidu.com/software/505/", //资讯阅读
            "http://shouji.baidu.com/software/507/", //办公学习

            "http://shouji.baidu.com/game/401/", //休闲益智
            "http://shouji.baidu.com/game/406/", //赛车竞速
            "http://shouji.baidu.com/game/407/", //棋牌桌游
            "http://shouji.baidu.com/game/402/", //脚色扮演
            "http://shouji.baidu.com/game/404/", //模拟辅助
            "http://shouji.baidu.com/game/403/", //动作射击
            "http://shouji.baidu.com/game/board_102_200/", //网络游戏
            "http://shouji.baidu.com/game/408/", //经营养成
            "http://shouji.baidu.com/game/405/" //体育竞技
        };

        public AppInfoDownloader_Baidu(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("Baidu receive start command while already running.");
                return;
            }
            mRunning = true;

            WebClient client = getWebClient();
            foreach (string url in url_app_categories)
            {
                string htmlCode = null;
                for (int pageIndex = 1; pageIndex <= 4; pageIndex++)
                {
                    if (!mRunning)
                    {
                        Log.info("Baidu downloader stop.");
                        return;
                    }
                    try
                    {
                        string pageUrl = url + "list_" + pageIndex + ".html";
                        Log.info("Fetch web page: " + pageUrl);
                        htmlCode = client.DownloadString(pageUrl);
                        //htmlCode = HtmlContentParser.toUtfString(htmlCode);
                        Log.info("Parse app info from web content: " + pageUrl);
                        string[] appInfos = htmlCode.Split(new string[] { "app-box" }, StringSplitOptions.None);
                        foreach (string splitItem in appInfos)
                        {
                            if (appInfos[0] == splitItem)
                            {
                                continue;
                            }
                            try
                            {
                                // get category
                                string category = getCategoryFromBaidu(url);
                                int splitBegin = splitItem.IndexOf("class=\"name\">") + ("class=\"name\">").Length;
                                int splitEnd = splitItem.IndexOf("<em>装进手机</em>");
                                string temp = splitItem.Substring(splitBegin, splitEnd - splitBegin).Trim();
                                // getApkName
                                int nameEndIndex = temp.IndexOf("</p>");
                                string apkName = temp.Substring(0, nameEndIndex).Trim();
                                if (apkName.Contains(':')) // ? * / \ < > : " |
                                {
                                    apkName = apkName.Replace(':', '_');
                                }
                                if (apkName.Contains('?'))
                                {
                                    apkName = apkName.Replace('?', '_');
                                }
                                if (apkName.Contains('*'))
                                {
                                    apkName = apkName.Replace('*', '_');
                                }
                                if (apkName.Contains('|'))
                                {
                                    apkName = apkName.Replace('|', '_');
                                }
                                if (apkName.Contains('/'))
                                {
                                    apkName = apkName.Replace('/', '_');
                                }
                                if (apkName.Contains('\\'))
                                {
                                    apkName = apkName.Replace('\\', '_');
                                }
                                if (apkName.Contains('<'))
                                {
                                    apkName = apkName.Replace('<', '_');
                                }
                                if (apkName.Contains('>'))
                                {
                                    apkName = apkName.Replace('>', '_');
                                }
                                if (apkName.Contains('"'))
                                {
                                    apkName = apkName.Replace('"', '_');
                                }

                                // getInstallCount
                                int installCountBegin = temp.IndexOf("down\">") + ("down\">").Length;
                                int installCountEnd = temp.IndexOf("下载</span>");
                                string installCountStr = temp.Substring(installCountBegin, installCountEnd - installCountBegin);
                                long installCount = HtmlContentParser.downloadNumberFromString(installCountStr);
                                // getApkSize
                                int tempSizeBegin = temp.IndexOf("class=\"size\">") + ("class=\"size\">").Length;
                                int tempSizeEnd = temp.IndexOf("class=\"down-btn\">");
                                string tempSizeStr = temp.Substring(tempSizeBegin, tempSizeEnd - tempSizeBegin);
                                int tempSizeEnd2 = tempSizeStr.IndexOf("</span>");
                                string apkSize = tempSizeStr.Substring(0, tempSizeEnd2);
                                // getDownloadUrl
                                int downloadUrlBegin = temp.IndexOf("data_url=\"") + ("data_url=\"").Length;
                                int downloadUrlEnd = temp.IndexOf("data_name=");
                                string downloadUrlTmp = temp.Substring(downloadUrlBegin, downloadUrlEnd - downloadUrlBegin).Trim();
                                string downloadUrl = downloadUrlTmp.Substring(0, downloadUrlTmp.Length - 1);
                                // getpkgName
                                int pkgNameBegin = temp.IndexOf("data_package=\"") + ("data_package=\"").Length;
                                int pkgNameEnd = temp.IndexOf("data_versionname=");
                                string pkgNameTmp = temp.Substring(pkgNameBegin, pkgNameEnd - pkgNameBegin).Trim();
                                string pkgName = pkgNameTmp.Substring(0, pkgNameTmp.Length - 1);
                                // getVersion
                                int versionBegin = temp.IndexOf("data_versionname=\"") + ("data_versionname=\"").Length;
                                int versionEnd = temp.IndexOf("data_icon=");
                                string versionTmp = temp.Substring(versionBegin, versionEnd - versionBegin).Trim();
                                string version = versionTmp.Substring(0, versionTmp.Length - 1);


                                AppInfo newAppInfo = new AppInfo();
                                newAppInfo.app_name = apkName;
                                newAppInfo.apk_name = generateApkName(newAppInfo.app_name);
                                newAppInfo.app_version = version;
                                newAppInfo.category = category;
                                newAppInfo.datePublished = "";
                                newAppInfo.downloads_store = installCount;
                                newAppInfo.download_url_store = downloadUrl;
                                newAppInfo.package_name = pkgName;
                                newAppInfo.size = HtmlContentParser.apkSizeFromString(apkSize);
                                newAppInfo.isSoft = url.Contains("soft");
                                newAppInfo.mStore = AppInfo.Store.Baidu;
                                newAppInfoFetched(newAppInfo);
                            }
                            catch (Exception ex)
                            {
                                Log.info(ex.Message);
                                Log.info(ex.StackTrace);
                            }
                            
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.error("Baidu downloader broken, need fix by code change!");
                        Log.error(ex.Message);
                        Log.error(ex.StackTrace);
                    }
                }
            }
            done();
        }

        public string getCategoryFromBaidu(string categoryUrl)
        {
            int categoryId = int.Parse(categoryUrl.Substring(categoryUrl.Length - 4, 3));
            switch (categoryId)
            {
                case 503:
                    return "社交通讯";
                case 501:
                    return "系统工具";
                case 510:
                    return "理财购物";
                case 502:
                    return "主题壁纸";
                case 509:
                    return "旅游出行";
                case 506:
                    return "影音播放";
                case 508:
                    return "拍摄美化";
                case 504:
                    return "生活实用";
                case 505:
                    return "资讯阅读";
                case 507:
                    return "办公学习";

                case 401:
                    return "休闲益智";
                case 406:
                    return "赛车竞速";
                case 407:
                    return "棋牌桌游";
                case 402:
                    return "角色扮演";
                case 404:
                    return "模拟辅助";
                case 403:
                    return "动作射击";
                case 200:
                    return "网络游戏";
                case 408:
                    return "经营养成";
                case 405:
                    return "体育竞技";

            }
            throw new Exception("Baidu unknown categoryID.");
        }


        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("Baidu downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }

        public override void downloadBatchAppInfo(object inf)
        {
            mBatchDownloading = true;
            List<AppInfo> list = (List<AppInfo>)inf;
            Log.info("Baidu downloader start batch download "+list.Count + " apps.");
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
                htmlCode = client.DownloadString("http://shouji.baidu.com/s?wd=" + info.app_name);
            }
            catch (WebException)
            {
                //Log.info(info.package_name + " not found in Baidu store ");
                return false;
            }
            //htmlCode = HtmlContentParser.toUtfString(htmlCode);

            int start = htmlCode.IndexOf("app-list");
            int end = htmlCode.IndexOf("装进手机");
            if (start < 0 || end < 0)
            {
                //Log.info(info.package_name + " not found in Baidu store ");
                return false;
            }
            string piece = htmlCode.Substring(start, end - start);
            lock (info)
            {
                if (info.apk_name == null || info.apk_name.Length < 1)
                {
                    info.apk_name = generateApkName(info.app_name);
                }
                string package = HtmlContentParser.getValueFromHtmlPiece(piece, "data_package", true);
                if (!package.Equals(info.package_name))
                {
                    //Log.info(info.package_name + " not found in Baidu store ");
                    return false;
                }
                info.app_version = HtmlContentParser.getValueFromHtmlPiece(piece, "data_versionname", true);
                info.category = HtmlContentParser.getValueFromHtmlPiece(piece, "data_detail_type", true);
                info.datePublished = "";

                //<em><span class="download-num">8428万</span>次下载</em>
                string downloadString = HtmlContentParser.getValueFromHtmlPiece(piece, "download-num", true);
                info.downloads_store = HtmlContentParser.downloadNumberFromString(downloadString);
                info.download_url_baidu = HtmlContentParser.getValueFromHtmlPiece(piece, "data_url", true);
                info.download_url_store = info.download_url_baidu;
                info.size = int.Parse(HtmlContentParser.getValueFromHtmlPiece(piece, "data_size", true));
                info.isSoft = HtmlContentParser.getValueFromHtmlPiece(piece, "data_detail_type", true).Contains("soft");
                info.mStore = AppInfo.Store.Baidu;
            }
            Log.info(info.package_name + " fetched from Baidu store ");
            return true;
        }

        public override void downloadTopUsageAppInfo()
        {
            List<AppInfo> topUsage = Config.getTopUsageList();
            foreach (AppInfo info in topUsage)
            {
                if (!mRunning)
                {
                    Log.info("Baidu downloader stoped.");
                    return;
                }
                if (findAppInfoInTotalList(info.package_name) != null)
                {
                    continue;
                }
                if (downloadSingleAppInfo(info))
                {
                    newAppInfoFetched(info);
                }
            }
        }
    }
}
