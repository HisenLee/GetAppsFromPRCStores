using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApkDownloader
{
    class AppInfoDownloader_Qihoo : AppInfoDownloaderBase
    {
        private string[] url_app_categories =
           {
            "http://zhushou.360.cn/list/index/cid/11/order/download/?page=", //系统安全
            "http://zhushou.360.cn/list/index/cid/12/order/download/?page=", //通讯社交
            "http://zhushou.360.cn/list/index/cid/14/order/download/?page=", //影音视听
            "http://zhushou.360.cn/list/index/cid/15/order/download/?page=", //新闻阅读
            "http://zhushou.360.cn/list/index/cid/16/order/download/?page=", //生活休闲
            "http://zhushou.360.cn/list/index/cid/18/order/download/?page=", //主题壁纸
            "http://zhushou.360.cn/list/index/cid/17/order/download/?page=", //办公商务
            "http://zhushou.360.cn/list/index/cid/102228/order/download/?page=", //摄影摄像
            "http://zhushou.360.cn/list/index/cid/102230/order/download/?page=", //购物优惠
            "http://zhushou.360.cn/list/index/cid/102231/order/download/?page=", //地图旅游
            "http://zhushou.360.cn/list/index/cid/102232/order/download/?page=", //教育学习
            "http://zhushou.360.cn/list/index/cid/102139/order/download/?page=", //金融理财
            "http://zhushou.360.cn/list/index/cid/102233/order/download/?page=", //健康医疗

            "http://zhushou.360.cn/list/index/cid/101587/order/download/?page=", //角色扮演
            "http://zhushou.360.cn/list/index/cid/19/order/download/?page=", //休闲益智
            "http://zhushou.360.cn/list/index/cid/20/order/download/?page=", //动作冒险
            "http://zhushou.360.cn/list/index/cid/100451/order/download/?page=", //网络游戏
            "http://zhushou.360.cn/list/index/cid/51/order/download/?page=", //体育竞速
            "http://zhushou.360.cn/list/index/cid/52/order/download/?page=", //飞行射击
            "http://zhushou.360.cn/list/index/cid/53/order/download/?page=", //经营策略
            "http://zhushou.360.cn/list/index/cid/54/order/download/?page=", //棋牌天地
            "http://zhushou.360.cn/list/index/cid/102238/order/download/?page=" //儿童游戏
        };

        private static bool isSoft(int cid)
        {
            switch (cid)
            {
                case 11:
                case 12:
                case 14:
                case 15:
                case 16:
                case 18:
                case 17:
                case 102228:
                case 102230:
                case 102231:
                case 102232:
                case 102139:
                case 102233:
                    return true;

                case 101587:
                case 19:
                case 20:
                case 100451:
                case 51:
                case 52:
                case 53:
                case 54:
                case 102238:
                    return false;
            }
            throw new Exception("unknown CID.");
        }
        private static string cIdToString(int cid)
        {
            switch (cid)
            {
                case 11:
                    return "系统安全";
                case 12:
                    return "通讯社交";
                case 14:
                    return "影音视听";
                case 15:
                    return "新闻阅读";
                case 16:
                    return "生活休闲";
                case 18:
                    return "主题壁纸";
                case 17:
                    return "办公商务";
                case 102228:
                    return "摄影摄像";
                case 102230:
                    return "购物优惠";
                case 102231:
                    return "地图旅游";
                case 102232:
                    return "教育学习";
                case 102139:
                    return "金融理财";
                case 102233:
                    return "健康医疗";
                case 101587:
                    return "角色扮演";
                case 19:
                    return "休闲益智";
                case 20:
                    return "动作冒险";
                case 100451:
                    return "网络游戏";
                case 51:
                    return "体育竞速";
                case 52:
                    return "飞行射击";
                case 53:
                    return "经营策略";
                case 54:
                    return "棋牌天地";
                case 102238:
                    return "儿童游戏";
            }
            throw new Exception("unknown CID.");
        }

        public AppInfoDownloader_Qihoo(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("Qihoo downloader receive start command while already running.");
                return;
            }
            mRunning = true;
            WebClient client = getWebClient();
            foreach (string url in url_app_categories)
            {
                string htmlCode = null;
                int pageCount = 0;
                for (int i = 1; i < 30; i++)
                {
                    if (!mRunning)
                    {
                        Log.info("Qihoo downloader stop.");
                        return;
                    }
                    try
                    {
                        string u = url + i;
                        Log.info("Fetch web page: " + u);
                        htmlCode = client.DownloadString(u);
                        //htmlCode = HtmlContentParser.toUtfString(htmlCode);

                        // pg.pageCount = 18.693877551;
                        if (i == 1)
                        {
                            int start_index = htmlCode.IndexOf("pg.pageCount = ") + 15;
                            int end_index = htmlCode.IndexOf(";", start_index);
                            string count = htmlCode.Substring(start_index, end_index - start_index);
                            if (count.Contains("."))
                            {
                                count = count.Substring(0, count.IndexOf("."));
                            }
                            pageCount = int.Parse(count);
                        }
                        else
                        {
                            if (i > pageCount)
                            {
                                Log.info("All pages fetched, Jump tp next category.");
                                break;
                            }
                        }

                        Log.info("Parse app info from web content: " + u);
                        string[] appInfos = htmlCode.Split(new string[] { "/detail/index/soft_id/" }, StringSplitOptions.None);
                        int index = 0;
                        foreach (string apkInfo in appInfos)
                        {
                            if (index++ == 0)
                            {
                                continue;
                            }
                            if (index % 2 == 0)
                            {
                                continue;
                            }
                            AppInfo newAppInfo = new AppInfo();

                            // /detail/index/soft_id/1625930">开心消消乐-春节砸金蛋</a></h3>
                            int index_start = apkInfo.IndexOf("\">") + 2;
                            int index_end = apkInfo.IndexOf("</");
                            newAppInfo.app_name = apkInfo.Substring(index_start, index_end - index_start);
                            newAppInfo.apk_name = generateApkName(newAppInfo.app_name); 

                            // /cid/11/order/
                            index_start = url.IndexOf("/cid/") + 5;
                            index_end = url.IndexOf("/order/");
                            int cid = Int32.Parse(url.Substring(index_start, index_end - index_start));
                            newAppInfo.category = cIdToString(cid);

                            // <span>38万次下载</span>
                            index_start = apkInfo.IndexOf("<span>") + 6;
                            index_end = apkInfo.IndexOf("次下载");
                            string downloadString = apkInfo.Substring(index_start, index_end - index_start);
                            newAppInfo.downloads_store = HtmlContentParser.downloadNumberFromString(downloadString);

                            // &url=http://shouji.360tpcdn.com/161215/xxx.apk"
                            // &url=pdown://http://shouji.360tpcdn.com/170120/7761218e200ab38fbba8e1f86bbc22a4/com.qidian.QDReader_257.apk|p2=4180403c83b65b468ec13ffcb36fd7e8e172c4dd|b2=12479947|b5=????|b9=1|p4=3600"
                            index_start = apkInfo.IndexOf("&url=") + 5;
                            index_end = apkInfo.IndexOf(".apk\"", index_start);
                            if (index_end < 0)
                            {
                                index_end = apkInfo.IndexOf(".apk", index_start);
                            }
                            string url_store = apkInfo.Substring(index_start, index_end - index_start);
                            if (!url_store.StartsWith("http"))
                            {
                                url_store = url_store.Substring(url_store.IndexOf("http"));
                            }
                            newAppInfo.download_url_store = url_store + ".apk";

                            index_start = newAppInfo.download_url_store.LastIndexOf("/") + 1;
                            index_end = newAppInfo.download_url_store.IndexOf("_", index_start);
                            if (index_end < 0)
                            {
                                index_end = newAppInfo.download_url_store.Length;
                            }
                            newAppInfo.package_name = newAppInfo.download_url_store.Substring(index_start, index_end - index_start);

                            newAppInfo.isSoft = isSoft(cid);
                            newAppInfo.mStore = AppInfo.Store.Qihoo;
                            newAppInfoFetched(newAppInfo);
                        }

                        if (appInfos.Length != 99)
                        {
                            break;
                        }
                        if (i >= 4)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.error("Qihoo downloader broken, need fix by code change!");
                        Log.error(ex.Message);
                        Log.error(ex.StackTrace);
                    }
                }
            }
            done();
        }

        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("Qihoo downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }

        public override void downloadBatchAppInfo(object inf)
        {
            mBatchDownloading = true;
            List<AppInfo> list = (List<AppInfo>)inf;
            Log.info("Qihoo downloader start batch download " + list.Count + " apps.");
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
                htmlCode = client.DownloadString("http://zhushou.360.cn/search/index/?kw=" + info.app_name);
            }
            catch (WebException)
            {
                //Log.info(info.package_name + " not found in Qihoo store ");
                return false;
            }
            //htmlCode = HtmlContentParser.toUtfString(htmlCode);
            string piece = null;
            try
            {
                piece = htmlCode.Split(new string[] { "/detail/index/soft_id/" }, StringSplitOptions.None)[2];
            }
            catch (Exception)
            {
                //Log.info(info.package_name + " not found in Qihoo store ");
                return false;
            }

            // http://m.shouji.360tpcdn.com
            int index_start = piece.IndexOf("http://");
            if (index_start < 0)
            {
                //Log.info(info.package_name + " not found in Qihoo store ");
                return false;
            }
            int index_end = piece.IndexOf(".apk\"", index_start);
            if (index_end < 0)
            {
                //Log.info(info.package_name + " not found in Qihoo store ");
                return false;
            }
            lock (info)
            {
                info.download_url_qihoo = piece.Substring(index_start, index_end - index_start);
                if (!info.download_url_qihoo.Contains(info.package_name))
                {
                    info.download_url_qihoo = "";
                    //Log.info(info.package_name + " not found in Qihoo store ");
                    return false;
                }
                info.download_url_store = info.download_url_qihoo;
                if (info.apk_name == null || info.apk_name.Length < 1)
                {
                    info.apk_name = generateApkName(info.app_name);
                }

                // downNum
                string downloadString = HtmlContentParser.getValueFromHtmlPiece(piece, "downNum", true);
                index_end = downloadString.IndexOf("次下载");
                downloadString = downloadString.Substring(0, index_end);
                info.downloads_store = HtmlContentParser.downloadNumberFromString(downloadString);

                info.mStore = AppInfo.Store.Qihoo;
            }
            Log.info(info.package_name + " info fetched from Qihoo store ");
            return true;
        }
        public override void downloadTopUsageAppInfo()
        {
            
            List<AppInfo> topUsage = Config.getTopUsageList();
            foreach (AppInfo info in topUsage)
            {
                if (!mRunning)
                {
                    Log.info("Qihoo downloader stoped.");
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
