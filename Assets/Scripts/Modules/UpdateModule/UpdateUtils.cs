using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebsiteWorkshop.Modules.UpdateModule
{
    public static class UpdateUtils
    {
        /// <summary>
        /// 比较版本号字符串新旧，版本号格式为"{ver1}.{ver2}.{ver3}"。eg:6.0.111
        /// </summary>
        /// <param name="localVersion">本地版本号</param>
        /// <param name="RemoteVersion">远端版本号</param>
        /// <returns>如果新版本，返回true</returns>
        public static bool IsNewerVersion(string localVersion, string RemoteVersion)
        {
            string[] localVers = localVersion.Split('.');
            string[] remoteVers = RemoteVersion.Split('.');

            for (int i = 0; i < localVers.Length; i++)
            {
                if (Int32.Parse(localVers[i]) == Int32.Parse(remoteVers[i]))
                {
                    continue;
                }
                return Int32.Parse(localVers[i]) < Int32.Parse(remoteVers[i]);
            }
            return false;
        }
        /// <summary>
        /// 计算数据的MD5值
        /// </summary>
        /// <param name="input">数据流</param>
        /// <returns>计算出的值，以十六进制格式显示</returns>
        public static string MD5Hash(Stream input)
        {
            byte[] data = MD5.Create().ComputeHash(input);
            input.Close();
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}

