using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebsiteWorkshop.Frameworks;
namespace WebsiteWorkshop.Modules
{
    public interface ISDKBridge
    {
        void DownloadFiles(string[] urls, string absoluteRoot, long totalBytes = 0);
    }
    public static class SDKBridgeAdapter
    {
        public static ISDKBridge Bridge
        {
            get
            {
                return AndroidBridge.Instance;
            }
        }
        #region 下载接口
        public static void DownloadFiles(string[] urls, string absoluteRoot, int totalBytes = 0)
        {
            Bridge.DownloadFiles(urls, absoluteRoot, totalBytes);
        }
        
        #endregion
    }

}

