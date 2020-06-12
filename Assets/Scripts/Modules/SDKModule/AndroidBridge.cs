using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebsiteWorkshop.Frameworks;

namespace WebsiteWorkshop.Modules
{
    public class AndroidBridge : SingletonTemplate<AndroidBridge>, ISDKBridge
    {
        /// <summary>
        /// 获取unity游戏的主活动
        /// </summary>
        public AndroidJavaObject MainActivity
        {
            get
            {
                if (_mainActivity == null)
                {
                    _mainActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _mainActivity;
            }
        }
        private AndroidJavaObject _mainActivity;

        public void DownloadFiles(string[] urls, string absoluteRoot, long totalBytes = 0)
        {
            MainActivity.Call("downloadFiles", urls, absoluteRoot, totalBytes);
        }
    }
}

