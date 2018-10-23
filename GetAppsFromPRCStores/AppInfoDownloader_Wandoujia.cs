using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ApkDownloader
{
    class AppInfoDownloader_Wandoujia : AppInfoDownloaderBase
    {
        private string[] url_app_categories =
        {
            "http://www.wandoujia.com/category/5029", //影音播放
            "http://www.wandoujia.com/category/5014", //通讯社交
            "http://www.wandoujia.com/category/5019", //新闻阅读
            "http://www.wandoujia.com/category/5026", //考试学习
            "http://www.wandoujia.com/category/5023", //金融理财
            "http://www.wandoujia.com/category/5021", //旅游出行
            "http://www.wandoujia.com/category/5022", //办公商务
            "http://www.wandoujia.com/category/5018", //系统工具
            "http://www.wandoujia.com/category/5024", //手机美化
            "http://www.wandoujia.com/category/5016", //摄影图像
            "http://www.wandoujia.com/category/5017", //网上购物
            "http://www.wandoujia.com/category/5020", //生活休闲
            "http://www.wandoujia.com/category/5028", //健康运动
            "http://www.wandoujia.com/category/5027", //育儿亲子



            "http://www.wandoujia.com/category/6001", //休闲益智
            "http://www.wandoujia.com/category/6008", //扑克棋牌
            "http://www.wandoujia.com/category/6002", //飞行射击
            "http://www.wandoujia.com/category/6009", //网络游戏
            "http://www.wandoujia.com/category/6006", //角色扮演
            "http://www.wandoujia.com/category/6003", //跑酷竞速
            "http://www.wandoujia.com/category/6004", //动作冒险
            "http://www.wandoujia.com/category/6007", //经营策略
            "http://www.wandoujia.com/category/6005", //体育竞技
            "http://www.wandoujia.com/category/5015"  //辅助工具
          
        };

        public AppInfoDownloader_Wandoujia(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("Wandoujia downloader receive start command while already running.");
                return;
            }
            mRunning = true;
            WebClient client = getWebClient();
            foreach (string url in url_app_categories)
            {
                string htmlCode = null;
                for (int pageIndex = 1; pageIndex <= 8; pageIndex++)
                {
                    if (!mRunning)
                    {
                        Log.info("Wandoujia downloader stop.");
                        return;
                    }
                    try
                    {
                        string pageUrl = url + "/" + pageIndex;
                        Log.info("Fetch web page: " + pageUrl);
                        try
                        {
                            htmlCode = client.DownloadString(pageUrl);
                        }
                        catch (Exception e)
                        {
                            if (e.Message != null && e.Message.Contains("404"))
                            {
                                Log.info("Page not found, Continue with next category.");
                                break;
                            }
                            else
                            {
                                throw e;
                            }
                        }
                        Log.info("Parse app info from web content: " + pageUrl);

                        string[] appInfos = htmlCode.Split(new string[] { "app-title-h2" }, StringSplitOptions.None);
                        foreach (string splitItem in appInfos)
                        {
                            if (appInfos[0] == splitItem)
                            {
                                continue;
                            }
                            try
                            {
                                // get apkDetailUrl
                                int detailBegin = splitItem.IndexOf("href=\"") + ("href=\"").Length;
                                int detailEnd = splitItem.IndexOf("title=\"");
                                string detailUrlSplit = splitItem.Substring(detailBegin, detailEnd - detailBegin).Trim();
                                string detailUrl = detailUrlSplit.Substring(0, detailUrlSplit.Length - 1).Trim();

                                // get pkgName
                                int pkgNameBegin = detailUrl.LastIndexOf("/") + ("/").Length;
                                string pkgName = detailUrl.Substring(pkgNameBegin, detailUrl.Length - pkgNameBegin);
                                if (pkgName.StartsWith("apps-"))
                                {
                                    int begin = pkgName.IndexOf("apps-") + ("apps-").Length;
                                    pkgName = pkgName.Substring(begin, pkgName.Length - begin);
                                }

                                // get downloadUrl
                                string downloadUrl = "http://www.wandoujia.com/apps/" + pkgName + "/download";

                                // apkDetailHtml
                                htmlCode = client.DownloadString(detailUrl);

                                // get category
                                string category = getCategoryFromWandoujia(url);

                                // get apkName
                                string apkName = "";
                                int apkNameSplitBegin = htmlCode.IndexOf("app-info") + ("app-info").Length;
                                int apkNameSplitEnd = htmlCode.IndexOf("class=\"download-wp");
                                if (apkNameSplitEnd != -1)
                                {
                                    string apkNameSplit = htmlCode.Substring(apkNameSplitBegin, apkNameSplitEnd - apkNameSplitBegin).Trim();
                                    int apkNameBegin = apkNameSplit.IndexOf("itemprop=\"name\">") + ("itemprop=\"name\">").Length;
                                    int apkNameEnd = apkNameSplit.IndexOf("</span>");
                                    apkName = apkNameSplit.Substring(apkNameBegin, apkNameEnd - apkNameBegin).Trim();
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

                                }
                                else
                                {
                                    int apkNameSplitEnd2 = htmlCode.IndexOf("offline-info");
                                    // 豌豆荚有可能会搜出下线的app，此时需要特殊处理
                                    if (apkNameSplitEnd2 != -1)
                                    {
                                        break;
                                    }

                                }

                                // get InstallCount
                                int installCountSplitBegin = htmlCode.IndexOf("UserDownloads:") + ("UserDownloads:").Length;
                                int installCountSplitEnd = htmlCode.IndexOf("</i><b>次下载</b>");
                                string installCountSplit = htmlCode.Substring(installCountSplitBegin, installCountSplitEnd - installCountSplitBegin).Trim();
                                int installCountBegin = installCountSplit.IndexOf("\">") + ("\">").Length;
                                int installCountEnd = installCountSplit.Length;
                                string installCountStr = installCountSplit.Substring(installCountBegin, installCountEnd - installCountBegin).Trim();
                                long installCount = HtmlContentParser.downloadNumberFromString(installCountStr);

                                // get apkSize
                                int apkSizeSplitBegin = htmlCode.IndexOf("fileSize") + ("fileSize").Length;
                                int apkSizeSplitEnd = htmlCode.IndexOf("<dt>分类</dt>");
                                string apkSizeSplit = htmlCode.Substring(apkSizeSplitBegin, apkSizeSplitEnd - apkSizeSplitBegin).Trim();
                                int apkSizeBegin = apkSizeSplit.IndexOf("content=\"") + ("content=\"").Length;
                                int apkSizeEnd = apkSizeSplit.IndexOf("\"></dd>");
                                string apkSize = apkSizeSplit.Substring(apkSizeBegin, apkSizeEnd - apkSizeBegin).Trim();

                                // get version
                                string version = "";

                                int versionSplitBegin = htmlCode.IndexOf("<dt>版本</dt>") + ("<dt>版本</dt>").Length;
                                // 有的app界面没有要求这一栏
                                int hasNeedFlag = htmlCode.IndexOf("<dt>要求</dt>");
                                if (hasNeedFlag != -1)
                                {
                                    int versionSplitEnd = hasNeedFlag;
                                    string versionSplit = htmlCode.Substring(versionSplitBegin, versionSplitEnd - versionSplitBegin).Trim();

                                    int versionTmpFlag = versionSplit.IndexOf("&nbsp;");
                                    if (versionTmpFlag != -1)
                                    {
                                        int versionBegin = versionSplit.IndexOf("<dd>&nbsp;") + ("<dd>&nbsp;").Length;
                                        int versionEnd = versionSplit.IndexOf("</dd>");
                                        version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                                    }
                                    else
                                    {
                                        int versionBegin = versionSplit.IndexOf("<dd>") + ("<dd>").Length;
                                        int versionEnd = versionSplit.IndexOf("</dd>");
                                        version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                                    }
                                }
                                else
                                {
                                    // 没有要求这一栏(但有来自这一栏)
                                    int versionSplitEnd = htmlCode.IndexOf("<dt>来自</dt>");
                                    if (versionSplitEnd != -1)
                                    {
                                        string versionSplit = htmlCode.Substring(versionSplitBegin, versionSplitEnd - versionSplitBegin).Trim();

                                        int versionTmpFlag = versionSplit.IndexOf("&nbsp;");
                                        if (versionTmpFlag != -1)
                                        {
                                            int versionBegin = versionSplit.IndexOf("<dd>&nbsp;") + ("<dd>&nbsp;").Length;
                                            int versionEnd = versionSplit.IndexOf("</dd>");
                                            version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                                        }
                                        else
                                        {
                                            int versionBegin = versionSplit.IndexOf("<dd>") + ("<dd>").Length;
                                            int versionEnd = versionSplit.IndexOf("</dd>");
                                            version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                                        }
                                    }
                                    else
                                    {
                                        // 没有要求这一栏, 也没有来自这一栏
                                        versionSplitEnd = htmlCode.IndexOf("infos relative-rec log-param-f");
                                        string versionSplit = htmlCode.Substring(versionSplitBegin, versionSplitEnd - versionSplitBegin).Trim();

                                        int versionTmpFlag = versionSplit.IndexOf("&nbsp;");
                                        if (versionTmpFlag != -1)
                                        {
                                            int versionBegin = versionSplit.IndexOf("<dd>&nbsp;") + ("<dd>&nbsp;").Length;
                                            int versionEnd = versionSplit.IndexOf("</dd>");
                                            version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                                        }
                                        else
                                        {
                                            int versionBegin = versionSplit.IndexOf("<dd>") + ("<dd>").Length;
                                            int versionEnd = versionSplit.IndexOf("</dd>");
                                            version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                                        }
                                    }

                                }

                                // get company
                                string company = "";
                                int companySplitBegin = htmlCode.IndexOf("<dt>来自</dt>") + ("<dt>来自</dt>").Length;
                                int companySplitEnd = htmlCode.IndexOf("</dl>");
                                if (companySplitBegin != -1 && companySplitEnd != -1 && companySplitEnd > companySplitBegin)
                                {
                                    string companySplit = htmlCode.Substring(companySplitBegin, companySplitEnd - companySplitBegin).Trim();
                                    int companyBegin = companySplit.IndexOf("itemprop=\"name\">") + ("itemprop=\"name\">").Length;
                                    int companyEnd = companySplit.IndexOf("</span>");
                                    if (companyBegin != -1 && companyEnd != -1 && companyEnd > companyBegin)
                                    {
                                        company = companySplit.Substring(companyBegin, companyEnd - companyBegin).Trim();
                                    }

                                }
                                

                                AppInfo newAppInfo = new AppInfo();
                                newAppInfo.app_name = apkName;
                                newAppInfo.apk_name = generateApkName(newAppInfo.app_name);
                                newAppInfo.package_name = pkgName;
                                newAppInfo.app_version = version;
                                newAppInfo.category = category;
                                newAppInfo.company = company;
                                newAppInfo.size = HtmlContentParser.apkSizeFromString(apkSize);
                                newAppInfo.downloads_store = installCount;
                                newAppInfo.download_url_store = downloadUrl;
                                newAppInfo.isSoft = (int.Parse(url.Substring(url.LastIndexOf("/") + 1)) < 6000 && int.Parse(url.Substring(url.LastIndexOf("/") + 1)) != 5015);
                                newAppInfo.mStore = AppInfo.Store.Wandoujia;
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
                        Log.error("Wandoujia downloader error, Need fix by code change!");
                        Log.error(ex.Message);
                        Log.error(ex.StackTrace);
                    }
                }
            }
            done();
        }

        public string getCategoryFromWandoujia(string categoryUrl)
        {
            int index_start = categoryUrl.LastIndexOf("/") + 1;
            int index_end = categoryUrl.Length;
            int categoryId = Int32.Parse(categoryUrl.Substring(index_start, index_end - index_start));

            switch (categoryId)
            {
                case 5014:
                    return "通讯社交";
                case 5029:
                    return "影音播放";
                case 5019:
                    return "新闻阅读";
                case 5026:
                    return "考试学习";
                case 5023:
                    return "金融理财";
                case 5021:
                    return "旅游出行";
                case 5018:
                    return "系统工具";
                case 5022:
                    return "办公商务";
                case 5024:
                    return "手机美化";
                case 5016:
                    return "摄影图像";
                case 5017:
                    return "网上购物";
                case 5020:
                    return "生活休闲";
                case 5028:
                    return "健康运动";
                case 5027:
                    return "育儿亲子";


                case 6001:
                    return "休闲益智";
                case 6008:
                    return "扑克棋牌";
                case 6002:
                    return "飞行射击";
                case 6009:
                    return "网络游戏";
                case 6006:
                    return "角色扮演";
                case 6003:
                    return "跑酷竞速";
                case 6004:
                    return "动作冒险";
                case 6007:
                    return "经营策略";
                case 6005:
                    return "体育竞技";
                case 5015:
                    return "辅助工具";

            }
            throw new Exception("Wandoujia unknown categoryID.");
        }


        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("Wandoujia downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }

        public override void downloadBatchAppInfo(object inf)
        {
            mBatchDownloading = true;
            List<AppInfo> list = (List<AppInfo>)inf;
            Log.info("Wandoujia downloader start batch download " + list.Count + " apps.");
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
                htmlCode = client.DownloadString("http://www.wandoujia.com/apps/" + info.package_name);
            }
            catch (WebException)
            {
                //Log.info(info.package_name + " not found in Wandoujia store ");
                return false;
            }
            //htmlCode = HtmlContentParser.toUtfString(htmlCode);
            lock (info)
            {
                if (info.apk_name == null || info.apk_name.Length < 1)
                {
                    info.apk_name = generateApkName(info.app_name);
                }
                info.download_url_wandoujia = "http://www.wandoujia.com/apps/" + info.package_name + "/download";
                info.download_url_store = info.download_url_wandoujia;
                info.mStore = AppInfo.Store.Wandoujia;

                // data-install="2.3 亿"
                try
                {
                    // get InstallCount
                    int installCountSplitBegin = htmlCode.IndexOf("UserDownloads:") + ("UserDownloads:").Length;
                    int installCountSplitEnd = htmlCode.IndexOf("</i><b>次下载</b>");
                    string installCountSplit = htmlCode.Substring(installCountSplitBegin, installCountSplitEnd - installCountSplitBegin).Trim();
                    int installCountBegin = installCountSplit.IndexOf("\">") + ("\">").Length;
                    int installCountEnd = installCountSplit.Length;
                    string installCountStr = installCountSplit.Substring(installCountBegin, installCountEnd - installCountBegin).Trim();
                    long installCount = HtmlContentParser.downloadNumberFromString(installCountStr);

                    // get apkSize
                    int apkSizeSplitBegin = htmlCode.IndexOf("fileSize") + ("fileSize").Length;
                    int apkSizeSplitEnd = htmlCode.IndexOf("<dt>分类</dt>");
                    string apkSizeSplit = htmlCode.Substring(apkSizeSplitBegin, apkSizeSplitEnd - apkSizeSplitBegin).Trim();
                    int apkSizeBegin = apkSizeSplit.IndexOf("content=\"") + ("content=\"").Length;
                    int apkSizeEnd = apkSizeSplit.IndexOf("\"></dd>");
                    string apkSize = apkSizeSplit.Substring(apkSizeBegin, apkSizeEnd - apkSizeBegin).Trim();

                    // get version
                    string version = "";

                    int versionSplitBegin = htmlCode.IndexOf("<dt>版本</dt>") + ("<dt>版本</dt>").Length;
                    // 有的app界面没有要求这一栏
                    int hasNeedFlag = htmlCode.IndexOf("<dt>要求</dt>");
                    if (hasNeedFlag != -1)
                    {
                        int versionSplitEnd = hasNeedFlag;
                        string versionSplit = htmlCode.Substring(versionSplitBegin, versionSplitEnd - versionSplitBegin).Trim();

                        int versionTmpFlag = versionSplit.IndexOf("&nbsp;");
                        if (versionTmpFlag != -1)
                        {
                            int versionBegin = versionSplit.IndexOf("<dd>&nbsp;") + ("<dd>&nbsp;").Length;
                            int versionEnd = versionSplit.IndexOf("</dd>");
                            version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                        }
                        else
                        {
                            int versionBegin = versionSplit.IndexOf("<dd>") + ("<dd>").Length;
                            int versionEnd = versionSplit.IndexOf("</dd>");
                            version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                        }
                    }
                    else
                    {
                        // 没有要求这一栏(但有来自这一栏)
                        int versionSplitEnd = htmlCode.IndexOf("<dt>来自</dt>");
                        if (versionSplitEnd != -1)
                        {
                            string versionSplit = htmlCode.Substring(versionSplitBegin, versionSplitEnd - versionSplitBegin).Trim();

                            int versionTmpFlag = versionSplit.IndexOf("&nbsp;");
                            if (versionTmpFlag != -1)
                            {
                                int versionBegin = versionSplit.IndexOf("<dd>&nbsp;") + ("<dd>&nbsp;").Length;
                                int versionEnd = versionSplit.IndexOf("</dd>");
                                version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                            }
                            else
                            {
                                int versionBegin = versionSplit.IndexOf("<dd>") + ("<dd>").Length;
                                int versionEnd = versionSplit.IndexOf("</dd>");
                                version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                            }
                        }
                        else
                        {
                            // 没有要求这一栏, 也没有来自这一栏
                            versionSplitEnd = htmlCode.IndexOf("infos relative-rec log-param-f");
                            string versionSplit = htmlCode.Substring(versionSplitBegin, versionSplitEnd - versionSplitBegin).Trim();

                            int versionTmpFlag = versionSplit.IndexOf("&nbsp;");
                            if (versionTmpFlag != -1)
                            {
                                int versionBegin = versionSplit.IndexOf("<dd>&nbsp;") + ("<dd>&nbsp;").Length;
                                int versionEnd = versionSplit.IndexOf("</dd>");
                                version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                            }
                            else
                            {
                                int versionBegin = versionSplit.IndexOf("<dd>") + ("<dd>").Length;
                                int versionEnd = versionSplit.IndexOf("</dd>");
                                version = versionSplit.Substring(versionBegin, versionEnd - versionBegin).Trim();
                            }
                        }

                    }

                    // get company
                    string company = "";
                    int companySplitBegin = htmlCode.IndexOf("<dt>来自</dt>") + ("<dt>来自</dt>").Length;
                    int companySplitEnd = htmlCode.IndexOf("</dl>");
                    if (companySplitBegin != -1 && companySplitEnd != -1 && companySplitEnd > companySplitBegin)
                    {
                        string companySplit = htmlCode.Substring(companySplitBegin, companySplitEnd - companySplitBegin).Trim();
                        int companyBegin = companySplit.IndexOf("itemprop=\"name\">") + ("itemprop=\"name\">").Length;
                        int companyEnd = companySplit.IndexOf("</span>");
                        if (companyBegin != -1 && companyEnd != -1 && companyEnd > companyBegin)
                        {
                            company = companySplit.Substring(companyBegin, companyEnd - companyBegin).Trim();
                        }

                    }
                   
                    info.app_version = version;
                    info.downloads_store = installCount;
                    info.company = company;
                    info.size = HtmlContentParser.apkSizeFromString(apkSize);
                }
                catch (Exception)
                {
                    Log.info(info.package_name + " not found in Wandoujia store ");
                    return false;
                }

                
            }
            Log.info(info.package_name + " fetched from Wandoujia store ");
            return true;
        }

        public override void downloadTopUsageAppInfo()
        {
            List<AppInfo> topUsage = Config.getTopUsageList();
            foreach (AppInfo info in topUsage)
            {
                if (!mRunning)
                {
                    Log.info("Wandoujia downloader stoped.");
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
