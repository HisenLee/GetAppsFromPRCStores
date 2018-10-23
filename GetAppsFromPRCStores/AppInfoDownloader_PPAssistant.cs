using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace ApkDownloader
{
    class AppInfoDownloader_PPAssistant : AppInfoDownloaderBase
    {
        private string[] url_app_categories =
       { 
            "https://www.25pp.com/android/soft/fenlei/5029/",   //        影音播放
            "https://www.25pp.com/android/soft/fenlei/5018/",   //        系统工具
            "https://www.25pp.com/android/soft/fenlei/5014/",   //        通讯社交
            "https://www.25pp.com/android/soft/fenlei/5024/",   //        手机美化
            "https://www.25pp.com/android/soft/fenlei/5019/",   //        新闻阅读
            "https://www.25pp.com/android/soft/fenlei/5016/",   //        摄影图像
            "https://www.25pp.com/android/soft/fenlei/5026/",   //        考试学习
            "https://www.25pp.com/android/soft/fenlei/5017/",   //        网上购物
            "https://www.25pp.com/android/soft/fenlei/5023/",   //        金融理财
            "https://www.25pp.com/android/soft/fenlei/5020/",   //        生活休闲
            "https://www.25pp.com/android/soft/fenlei/5021/",   //        旅游出行
            "https://www.25pp.com/android/soft/fenlei/5028/",   //        健康运动
            "https://www.25pp.com/android/soft/fenlei/5022/",   //        办公商务
            "https://www.25pp.com/android/soft/fenlei/5027/",   //        育儿亲子

            "https://www.25pp.com/android/game/fenlei/6001/",   //        休闲益智
            "https://www.25pp.com/android/game/fenlei/6003/",   //        跑酷竞速
            "https://www.25pp.com/android/game/fenlei/6008/",   //        扑克棋牌
            "https://www.25pp.com/android/game/fenlei/6004/",   //        动作冒险
            "https://www.25pp.com/android/game/fenlei/6002/",   //        飞行射击
            "https://www.25pp.com/android/game/fenlei/6007/",   //        经营策略
            "https://www.25pp.com/android/game/fenlei/6009/",   //        网络游戏
            "https://www.25pp.com/android/game/fenlei/6005/",   //        体育竞技
            "https://www.25pp.com/android/game/fenlei/6006/",   //        角色扮演
            "https://www.25pp.com/android/game/fenlei/5015/"    //        辅助工具            
        };

        private static string cIdToString(int cid)
        {
            switch (cid)
            {
                case 5029:
                    return "影音播放";
                case 5018:
                    return "系统工具";
                case 5014:
                    return "通讯社交";
                case 5024:
                    return "手机美化";
                case 5019:
                    return "新闻阅读";
                case 5016:
                    return "摄影图像";
                case 5026:
                    return "考试学习";
                case 5017:
                    return "网上购物";
                case 5023:
                    return "金融理财";
                case 5020:
                    return "生活休闲";
                case 5021:
                    return "旅游出行";
                case 5028:
                    return "健康运动";
                case 5022:
                    return "办公商务";
                case 5027:
                    return "育儿亲子";


                case 6001:
                    return "休闲益智";
                case 6003:
                    return "跑酷竞速";
                case 6008:
                    return "扑克棋牌";
                case 6004:
                    return "动作冒险";
                case 6002:
                    return "飞行射击";
                case 6007:
                    return "经营策略";
                case 6009:
                    return "网络游戏";
                case 6005:
                    return "体育竞技";
                case 6006:
                    return "角色扮演";
                case 5015:
                    return "辅助工具";
            }
            throw new Exception("unknown CID.");
        }


        public AppInfoDownloader_PPAssistant(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("PPAssistant receive start command while already running.");
                return;
            }
            mRunning = true;

            WebClient client = getWebClient();
            foreach (string url in url_app_categories)
            {
                string htmlCode = null;
                for (int i = 1; i < 20; i++)
                {
                    if (!mRunning)
                    {
                        Log.info("PPAssistant downloader stop.");
                        return;
                    }
                    try
                    {
                        string u = url + i;
                        Log.info("Fetch web page: " + u);
                        htmlCode = client.DownloadString(u);
                        Log.info("Parse app info from web content: " + u);

                        int temp1 = htmlCode.IndexOf("cate-list-main");
                        int temp2 = htmlCode.IndexOf("page-wrap");
                        string piece = htmlCode.Substring(temp1, temp2 - temp1);


                        string[] appInfos = piece.Split(new string[] { "res_id" }, StringSplitOptions.None);
                        foreach (string apkInfo in appInfos)
                        {
                            if (appInfos[0] == apkInfo)
                            {
                                continue;
                            }


                            AppInfo newAppInfo = new AppInfo();

                            string detailId = apkInfo.Substring(1, apkInfo.IndexOf(";")-1);
                            string detailLink = "https://www.25pp.com/android/detail_"+detailId+"/";  // 详情页的链接


                            int pkgStartIndex = apkInfo.IndexOf("pack_id=")+ "pack_id=".Length;
                            int pkgEndIndex = apkInfo.IndexOf("obj_type") - 1;
                            newAppInfo.package_name = apkInfo.Substring(pkgStartIndex, pkgEndIndex - pkgStartIndex);//包名

                            int tempStartIndex = apkInfo.IndexOf("appname=") + "appname=".Length + 1;
                            int tempEndIndex = apkInfo.IndexOf("closetimer=");
                            string temp = apkInfo.Substring(tempStartIndex, tempEndIndex - tempStartIndex);

                            int nameStart = 0;
                            int nameEnd = temp.IndexOf("\"");                       
                            newAppInfo.app_name = temp.Substring(nameStart, nameEnd - nameStart);        // 应用名
                            newAppInfo.apk_name = generateApkName(newAppInfo.app_name);

                            int downloadStartIndex = temp.LastIndexOf("appdownurl=") + "appdownurl=".Length + 1; 
                            int downloadEndIndex = temp.LastIndexOf("\"");
                            newAppInfo.download_url_store = temp.Substring(downloadStartIndex, downloadEndIndex - downloadStartIndex).TrimEnd();  //下载链接

                            // 版本号；应用大小；安装次数 需要在详情页面解析
                            string info = getDetailInfo(detailLink);
                            string[] Infos = info.Split(new string[] { ":" }, StringSplitOptions.None);
                            newAppInfo.size = HtmlContentParser.apkSizeFromString(Infos[0]); // 大小
                            if (Infos!=null && Infos.Length>1)
                            {
                                newAppInfo.app_version = Infos[1]; // 版本号
                                newAppInfo.downloads_store = HtmlContentParser.downloadNumberFromString(Infos[2]);// 下载次数
                            }                            

                            newAppInfo.isSoft = url.Contains("android/soft");  // app类型  soft/game
                            int cid = Int32.Parse(url.Substring(url.IndexOf("fenlei/") + "fenlei/".Length, 4));
                            newAppInfo.category = cIdToString(cid);     // app分类

                            newAppInfo.mStore = AppInfo.Store.PPAssistant;
                            newAppInfoFetched(newAppInfo);
                        }

                        if (i >= 3)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.error("PPAssistant downloader broken, need fix by code change!");
                        Log.error(ex.Message);
                        Log.error(ex.StackTrace);
                    }
                }
            }
            done();
        }

        /**
         *  版本号；应用大小；安装次数 需要在详情页面解析
         * 
         **/
        public string getDetailInfo(string detailLink)
        {
            string info = "";

            WebClient client = getWebClient();
            string htmlCode = null;
            try
            {
                htmlCode = client.DownloadString(detailLink);

                int tempStart = htmlCode.IndexOf("更新时间") + "更新时间".Length;
                int tempEnd = htmlCode.IndexOf("最低版本");
                string tempVesionAndSize = htmlCode.Substring(tempStart, tempEnd - tempStart);

                int tempSizeStart = tempVesionAndSize.IndexOf("小")+1;
                int tempSizeEnd = tempVesionAndSize.IndexOf("</strong></span></p>");
                string tempSize = tempVesionAndSize.Substring(tempSizeStart, tempSizeEnd - tempSizeStart);
                int tempSizeBegin = tempSize.IndexOf("<strong>")+ "<strong>".Length;
                string size = tempSize.Substring(tempSizeBegin, tempSize.Length- tempSizeBegin);  // app大小

                int tempVersionStart = tempVesionAndSize.IndexOf("本") + 1;
                int tempVersionEnd = tempVesionAndSize.LastIndexOf("</strong>");
                string tempVersion = tempVesionAndSize.Substring(tempVersionStart, tempVersionEnd- tempVersionStart);
                int tempVersionBegin = tempVersion.IndexOf("<strong>")+ "<strong>".Length;
                string version = tempVersion.Substring(tempVersionBegin, tempVersion.Length-tempVersionBegin); // app版本

                int tempInstallStart = htmlCode.IndexOf("app-downs") + "app-downs".Length;
                int tempInstallEnd = htmlCode.IndexOf("app-qrcode");
                string tempInstall = htmlCode.Substring(tempInstallStart, tempInstallEnd-tempInstallStart);
                int tempInstallBegin = tempInstall.IndexOf(">")+1;
                int tempInstallEnd2 = tempInstall.IndexOf("下载");
                string install = tempInstall.Substring(tempInstallBegin, tempInstallEnd2- tempInstallBegin); // app安装量

                int tempLinkBegin = tempInstall.IndexOf("appdownurl=")+ "appdownurl=".Length+1;
                int tempLinkEnd = tempInstall.IndexOf("closetimer=");
                string tempLink = tempInstall.Substring(tempLinkBegin, tempLinkEnd - tempLinkBegin).TrimEnd();
                string link = tempLink.Substring(0, tempLink.Length-1);

                info = size+":"+version+":"+install;
            }
            catch (WebException ex)
            {
                Log.error("PPAssistant detail downloader broken, need fix by code change!");
                Log.error(ex.Message);
                Log.error(ex.StackTrace);
                return info;
            }
            return info;
        }

        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("PPAssistant downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }

        public override void downloadBatchAppInfo(object inf)
        {
            mBatchDownloading = true;
            List<AppInfo> list = (List<AppInfo>)inf;
            Log.info("PPAssistant downloader start batch download " + list.Count + " apps.");
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
            string htmlCodeSearch = null;
            try
            {
                htmlCodeSearch = client.DownloadString("https://www.25pp.com/android/search_app/"+info.app_name+"/");
            }
            catch (WebException)
            {
                //Log.info(info.package_name + " not found in PPAssistant store ");
                return false;
            }
            //htmlCode = HtmlContentParser.toUtfString(htmlCode);
            lock (info)
            {
                if (info.apk_name == null || info.apk_name.Length < 1)
                {
                    info.apk_name = generateApkName(info.app_name);
                }

                string[] appInfos = htmlCodeSearch.Split(new string[] { "res_id" }, StringSplitOptions.None);

                if (appInfos == null || appInfos.Length<2)
                {
                    //Log.info(info.package_name + " not found in PPAssistant store ");
                    return false;
                }
                string apkInfo = appInfos[1];
                string detailId = apkInfo.Substring(1, apkInfo.IndexOf(";") - 1);
                string detailLink = "https://www.25pp.com/android/detail_" + detailId + "/";  // 详情页的链接

                string htmlCode = null;

                try
                {
                    htmlCode = client.DownloadString(detailLink);
                }
                catch (Exception)
                {
                    //Log.info(info.package_name + " not found in PPAssistant store ");
                    return false;
                }

                int tempStart = htmlCode.IndexOf("更新时间") + "更新时间".Length;
                int tempEnd = htmlCode.IndexOf("最低版本");
                if (tempStart>-1 && tempEnd>tempStart) {
                    string tempVesionAndSize = htmlCode.Substring(tempStart, tempEnd - tempStart);

                    int tempSizeStart = tempVesionAndSize.IndexOf("小") + 1;
                    int tempSizeEnd = tempVesionAndSize.IndexOf("</strong></span></p>");
                    string tempSize = tempVesionAndSize.Substring(tempSizeStart, tempSizeEnd - tempSizeStart);
                    int tempSizeBegin = tempSize.IndexOf("<strong>") + "<strong>".Length;
                    string size = tempSize.Substring(tempSizeBegin, tempSize.Length - tempSizeBegin);  // app大小

                    int tempVersionStart = tempVesionAndSize.IndexOf("本") + 1;
                    int tempVersionEnd = tempVesionAndSize.LastIndexOf("</strong>");
                    string tempVersion = tempVesionAndSize.Substring(tempVersionStart, tempVersionEnd - tempVersionStart);
                    int tempVersionBegin = tempVersion.IndexOf("<strong>") + "<strong>".Length;
                    string version = tempVersion.Substring(tempVersionBegin, tempVersion.Length - tempVersionBegin); // app版本

                    int tempInstallStart = htmlCode.IndexOf("app-downs") + "app-downs".Length;
                    int tempInstallEnd = htmlCode.IndexOf("app-qrcode");
                    string tempInstall = htmlCode.Substring(tempInstallStart, tempInstallEnd - tempInstallStart);
                    int tempInstallBegin = tempInstall.IndexOf(">") + 1;
                    int tempInstallEnd2 = tempInstall.IndexOf("下载");
                    string install = tempInstall.Substring(tempInstallBegin, tempInstallEnd2 - tempInstallBegin); // app安装量

                    int tempLinkBegin = tempInstall.IndexOf("appdownurl=") + "appdownurl=".Length + 1;
                    int tempLinkEnd = tempInstall.IndexOf("closetimer=");
                    string tempLink = tempInstall.Substring(tempLinkBegin, tempLinkEnd - tempLinkBegin).TrimEnd();
                    string link = tempLink.Substring(0, tempLink.Length - 1); // 下载链接

                    info.size = HtmlContentParser.apkSizeFromString(size); // 大小
                    info.app_version = version; // 版本号
                    info.downloads_store = HtmlContentParser.downloadNumberFromString(install);// 下载次数

                    info.download_url_ppAssistant = link;
                    info.download_url_store = info.download_url_ppAssistant;
                    info.mStore = AppInfo.Store.PPAssistant;
                }
              
            }
            Log.info(info.package_name + " fetched from PPAssistant store ");
            return true;
        }

        public override void downloadTopUsageAppInfo()
        {
            List<AppInfo> topUsage = Config.getTopUsageList();
            foreach (AppInfo info in topUsage)
            {
                if (!mRunning)
                {
                    Log.info("PPAssistant downloader stoped.");
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
