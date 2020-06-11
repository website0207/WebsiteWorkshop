using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using System;
using System.Linq;
using WebsiteWorkshop.Frameworks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace WebsiteWorkshop.Modules.UpdateModule
{
    public class UpdateSharedData : SharedProcedureData<UpdateSharedData>
    {
        /// <summary>
        /// 处理下载的类
        /// </summary>
        public MonoBehaviour Processor { get; set; }
        /// <summary>
        /// 位于本地的version数据
        /// </summary>
        public JsonData LocalVersion { get; set; }
        /// <summary>
        /// 位于本地的catalog数据
        /// </summary>
        public JsonData LocalCatalog { get; set; }
        /// <summary>
        /// 位于远端的version数据
        /// </summary>
        public JsonData RemoteVersion { get; set; }
        /// <summary>
        /// 位于远端的catalog数据
        /// </summary>
        public JsonData RemoteCatalog { get; set; }
        /// <summary>
        /// 下载所需信息队列
        /// </summary>
        public List<AssetDownloadData> DownloadList
        {
            get
            {
                if (_downloadList == null)
                {
                    _downloadList = new List<AssetDownloadData>();
                }
                return _downloadList;
            }
        }
        private List<AssetDownloadData> _downloadList;
        /// <summary>
        /// 下载压缩包的信息
        /// </summary>
        public JsonData RemotePackage { get; set; }

        public AndroidJavaObject Activity
        {
            get
            {
                if (_activity == null)
                {
                    _activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _activity;
            }
        }
        private AndroidJavaObject _activity;
    }

    public class LocalProcedure : BaseProcedure
    {
        public LocalProcedure(BaseProcedure next) : base(next) { }
        protected override bool CheckDependency()
        {
            return true;
        }
        protected override void HandleProcedure()
        {
            try
            {
                // 初次安装之后，建立更新根目录
                if (!Directory.Exists(EnvironmentVariables.ExportAssetsRoot))
                {
                    Directory.CreateDirectory(EnvironmentVariables.ExportAssetsRoot);
                }
                LoadLocalVersion();
                LoadLocalCatalog();
                PostProcedure();
            }
            catch (Exception e)
            {
                Debug.LogError("Stop @LocalProcedure");
            }
        }
        private void LoadLocalVersion()
        {
            try
            {
                // 检查版本信息文件，如果本地存在下载的文件
                if (File.Exists(EnvironmentVariables.LocalVersionPath))
                {
                    string versionStr = Encoding.UTF8.GetString(File.ReadAllBytes(EnvironmentVariables.LocalVersionPath));
                    UpdateSharedData.Instance.LocalVersion = JsonMapper.ToObject(versionStr);
                }
                // 否则读取默认路径的文件
                else
                {
                    string versionStr = (Resources.Load("Config/version") as TextAsset).text;
                    UpdateSharedData.Instance.LocalVersion = JsonMapper.ToObject(versionStr);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("load local version fail:{0}", e);
                throw;
            }
        }
        private void LoadLocalCatalog()
        {
            try
            {
                // 检查catalog信息文件，如果本地存在清单文件
                if (File.Exists(EnvironmentVariables.LocalCatalogPath))
                {
                    string catalogStr = Encoding.UTF8.GetString(File.ReadAllBytes(EnvironmentVariables.LocalCatalogPath));
                    UpdateSharedData.Instance.LocalCatalog = JsonMapper.ToObject(catalogStr);
                }
                // 否则读取默认路径的清单，一般是用于首包
                else
                {
                    string catalogStr = (Resources.Load("Config/catalog") as TextAsset).text;
                    UpdateSharedData.Instance.LocalCatalog = JsonMapper.ToObject(catalogStr);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("load local catalog fail:{0}", e);
                throw;
            }
        }
    }

    public class RemoteProcedure : BaseProcedure
    {
        public override event Action OnError;

        public RemoteProcedure(BaseProcedure next) : base(next) { }

        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.Processor != null 
                && UpdateSharedData.Instance.LocalVersion != null;
        }
        protected override void HandleProcedure()
        {
            UpdateSharedData.Instance.Processor.StartCoroutine(GetRemoteFiles());
        }

        private IEnumerator GetRemoteFiles()
        {
            // 远程拿到version文件
            using (UnityWebRequest uwr = UnityWebRequest.Get(((string)UpdateSharedData.Instance.LocalVersion["versionurl"])))
            {
                uwr.certificateHandler = new UpdateCertificateHandler();
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    Debug.LogErrorFormat("load remote version fail:{0}", uwr.error);
                    Debug.LogError("Stop @RemoteProcedure");
                    OnError?.Invoke();
                    yield break;
                }
                else
                {
                    Debug.Log(uwr.downloadHandler.text);
                    UpdateSharedData.Instance.RemoteVersion = JsonMapper.ToObject(uwr.downloadHandler.text);
                }
            }
            // 远程拿到serverlist
            using (UnityWebRequest uwr = UnityWebRequest.Get((string)UpdateSharedData.Instance.RemoteVersion["serverlist"]))
            {
                uwr.certificateHandler = new UpdateCertificateHandler();
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    Debug.LogErrorFormat("load serverlist fail:{0}", uwr.error);
                    Debug.LogError("Stop @RemoteProcedure");
                    OnError?.Invoke();
                    yield break;
                }
                else
                {
                    Debug.Log(uwr.downloadHandler.text);
                    try
                    {
                        File.WriteAllText(EnvironmentVariables.ServerlistPath, uwr.downloadHandler.text);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogErrorFormat("write serverlist fail:{0}", uwr.error);
                        Debug.LogError("Stop @RemoteProcedure");
                        OnError?.Invoke();
                        yield break;
                    }
                }
            }
            // 远程拿到announcement
            using (UnityWebRequest uwr = UnityWebRequest.Get((string)UpdateSharedData.Instance.RemoteVersion["announcement"]))
            {
                uwr.certificateHandler = new UpdateCertificateHandler();
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    Debug.LogErrorFormat("load announcement fail:{0}", uwr.error);
                    Debug.LogError("Stop @RemoteProcedure");
                    OnError?.Invoke();
                    yield break;
                }
                else
                {
                    Debug.Log(uwr.downloadHandler.text);
                    try
                    {
                        File.WriteAllText(EnvironmentVariables.AnnouncementPath, uwr.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogErrorFormat("write announcement fail:{0}", uwr.error);
                        Debug.LogError("Stop @RemoteProcedure");
                        OnError?.Invoke();
                        yield break;
                    }
                }
            }

            PostProcedure();
        }
    }

    public class ConfirmDownloadProcedure : BaseProcedure
    {
        public ConfirmDownloadProcedure(BaseProcedure needDownload, BaseProcedure notDownload) : base(needDownload, notDownload) { }
        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.LocalVersion != null 
                && UpdateSharedData.Instance.RemoteVersion != null;
        }
        protected override void HandleProcedure()
        {
            if (UpdateUtils.IsNewerVersion((string)UpdateSharedData.Instance.LocalVersion["version"], (string)UpdateSharedData.Instance.RemoteVersion["version"]))
            {
                Next = Siblings[0];
            }
            else
            {
                Next = Siblings[1];
            }

            PostProcedure();
        }
    }

    public class DownloadCatalogProcedure : BaseProcedure
    {
        public override event Action OnError;

        public DownloadCatalogProcedure(BaseProcedure next) : base(next) { }
        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.RemoteVersion != null 
                && UpdateSharedData.Instance.Processor != null;
        }
        protected override void HandleProcedure()
        {
            UpdateSharedData.Instance.Processor.StartCoroutine(LoadRemoteCatalog());
        }

        private IEnumerator LoadRemoteCatalog()
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get((string)UpdateSharedData.Instance.RemoteVersion["catalogurl"]))
            {
                uwr.certificateHandler = new UpdateCertificateHandler();
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    Debug.LogErrorFormat("load remote catalog fail:{0}", uwr.error);
                    Debug.LogError("Stop @DownloadCatalogProcedure");
                    OnError?.Invoke();
                    yield break;
                }
                else
                {
                    if (!Directory.Exists(Path.GetDirectoryName(EnvironmentVariables.RemoteCatalogPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(EnvironmentVariables.RemoteCatalogPath));
                    }
                    try
                    {
                        File.WriteAllText(EnvironmentVariables.RemoteCatalogPath, uwr.downloadHandler.text);
                        UpdateSharedData.Instance.RemoteCatalog = JsonMapper.ToObject(uwr.downloadHandler.text);

                        PostProcedure();
                    }
                    catch (Exception e)
                    {
                        Debug.LogErrorFormat("write remote catalog fail:{0}", e);
                        Debug.LogError("Stop @DownloadCatalogProcedure");
                        OnError?.Invoke();
                        yield break;
                    }
                }
            }
        }
    }

    public class AnalyzeCatalogProcedure : BaseProcedure
    {
        public override event Action OnError;
        public float RequiredDownloadSize { get; set; }
        public AnalyzeCatalogProcedure(BaseProcedure next) : base(next) { }
        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.LocalCatalog != null 
                && UpdateSharedData.Instance.RemoteCatalog != null
                && UpdateSharedData.Instance.RemoteVersion != null;
        }
        protected override void HandleProcedure()
        {
            AnalyzeDownloadList(UpdateSharedData.Instance.RemoteCatalog["assets"].Keys, UpdateSharedData.Instance.LocalCatalog["assets"].Keys);
        }
        private void AnalyzeDownloadList(IEnumerable<string> remoteKeys, IEnumerable<string> localKeys)
        {
            RequiredDownloadSize = 0f;
            UpdateSharedData.Instance.DownloadList.Clear();
            bool[] completeFlags = new bool[3] { false, false, false };
            Action onComplete = () =>
            {
                // 需要判断是完成而退出线程而不是因为异常而退出，因此不能用Thread.isAlive
                bool allComplete = true;
                foreach (var f in completeFlags)
                {
                    allComplete = allComplete && f;
                }
                if (allComplete)
                {
                    Debug.Log("Check Done!");
                    Loom.DispatchToMainThread(() => PostProcedure());
                }
            };
            Debug.Log("Start remote version assets check");
            try
            {
                // for update
                Loom.StartSingleThread(action =>
                {
                    IEnumerable<string> updateKeys = remoteKeys.Intersect(localKeys);
                    AddToDownloadList(updateKeys);
                    completeFlags[0] = true;
                    (action as Action)?.Invoke();
                }, onComplete);

                // for add
                Loom.StartSingleThread(action =>
                {
                    IEnumerable<string> addKeys = remoteKeys.Except(localKeys);
                    AddToDownloadList(addKeys);
                    completeFlags[1] = true;
                    (action as Action)?.Invoke();
                }, onComplete);

                // for delete
                Loom.StartSingleThread(action =>
                {
                    IEnumerable<string> deleteKeys = localKeys.Except(remoteKeys);
                    foreach (var item in deleteKeys)
                    {
                        string deletePath = Path.Combine(EnvironmentVariables.ExportAssetsRoot, item);
                        if (File.Exists(deletePath))
                        {
                            File.Delete(deletePath);
                        }
                    }
                    completeFlags[2] = true;
                    (action as Action)?.Invoke();
                }, onComplete);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("analyze catalog fail:{0}", e);
                Debug.LogError("Stop @AnalyzeCatalogProcedure");
                OnError?.Invoke();
            }
        }

        private void AddToDownloadList(IEnumerable<string> assetKeys)
        {
            foreach (var item in assetKeys)
            {
                // update catalog 中的MD5值
                string assetRemoteMD5 = (string)UpdateSharedData.Instance.RemoteCatalog["assets"][item]["md5"];
                // current catalog 中的MD5值
                string assetLocalMD5 = UpdateSharedData.Instance.LocalCatalog["assets"].Keys.Contains(item) ? (string)UpdateSharedData.Instance.LocalCatalog["assets"][item]["md5"] : null;

                // 添加进更新数组中的文件，1个条件：
                // 1.本地文件的MD5值与update catalog列表中不同。因为MD5相同的话，可能的情况是该文件已经下载但是整个更新流程并未完成。因此不用重复下载
                // 取消ver比较的原因在于，可能出现某个在中间某个版本删除，然后原文件又被加回来的情况。这样就会导致ver号升高但是MD5不变，等同于下载完成但是更新未完成，因此ver号失去校验意义
                string fileAbsPath = Path.Combine(EnvironmentVariables.ExportAssetsRoot, item);
                if (assetRemoteMD5.Equals(assetLocalMD5))      // 不需要更新
                    continue;
                else if ((File.Exists(fileAbsPath)                // 文件存在
                        && UpdateUtils.MD5Hash(File.OpenRead(fileAbsPath)).Equals(assetRemoteMD5)))    //已下载完成
                {
                    Debug.Log("the asset has been downloaded@" + fileAbsPath);
                    continue;
                }
                else
                {
                    AssetDownloadData data = new AssetDownloadData();
                    data.localPath = item;
                    Uri result;
                    if (Uri.TryCreate(new Uri((string)UpdateSharedData.Instance.RemoteVersion["cdn1"]), item, out result))
                    {
                        data.uri1 = result.ToString();
                    }
                    if (Uri.TryCreate(new Uri((string)UpdateSharedData.Instance.RemoteVersion["cdn2"]), item, out result))
                    {
                        data.uri2 = result.ToString();
                    }
                    data.size = ((int)UpdateSharedData.Instance.RemoteCatalog["assets"][item]["size"]) / 1024l;
                    RequiredDownloadSize += ((int)UpdateSharedData.Instance.RemoteCatalog["assets"][item]["size"]) / 1024f;
                    UpdateSharedData.Instance.DownloadList.Add(data);
                }
            }
        }
    }

    public class DownloadAssetsProcedure : BaseProcedure
    {
        private CoroutineQueue queue { get; set; }

        public override event Action OnError;

        public List<AssetDownloadData> ErrorList {
            get
            {
                if (_errorList == null)
                {
                    _errorList = new List<AssetDownloadData>();
                }
                return _errorList;
            }
        }
        private List<AssetDownloadData> _errorList;

        public DownloadAssetsProcedure(BaseProcedure next = null) : base(next) { }

        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.DownloadList != null;
        }
        protected override void HandleProcedure()
        {
            GetRemoteAssets();
        }

        private bool IsAllDownloadCached()
        {
            return UpdateSharedData.Instance.DownloadList.Count == 0;
        }

        private void HandleAfterDownloadSuccess()
        {
            try
            {
                File.WriteAllText(EnvironmentVariables.LocalVersionPath, UpdateSharedData.Instance.RemoteVersion.ToJson());
                File.WriteAllText(EnvironmentVariables.LocalCatalogPath, UpdateSharedData.Instance.RemoteCatalog.ToJson());
                if (File.Exists(EnvironmentVariables.RemoteCatalogPath))
                {
                    File.Delete(EnvironmentVariables.RemoteCatalogPath);
                }
                Debug.Log("Download asset success");
                PostProcedure();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("write remote version/catalog fail:{0}", e);
                Debug.LogError("Stop @DownloadAssetsProcedure");
                OnError?.Invoke();
            }
        }

        private void GetRemoteAssets()
        {
            if (IsAllDownloadCached())
            {
                HandleAfterDownloadSuccess();
            }
            ErrorList.Clear();
            // 协程请求数限制在100以下，因为：Curl error limit reached: 100 consecutive messages printed，由此推断UnityWebRequest的同时请求应该不超过这个值
            queue = new CoroutineQueue(100, UpdateSharedData.Instance.Processor.StartCoroutine);
            queue.OnBatchComplete += () => {
                bool isSuccess = true;
                // 检验下载完整性
                foreach (var item in UpdateSharedData.Instance.DownloadList)
                {
                    if (!(item as AssetDownloadData).isSuccess)
                    {
                        isSuccess = false;
                        break;
                    }
                }
                if (isSuccess)
                {
                    HandleAfterDownloadSuccess();
                }
                else
                {
                    Debug.LogErrorFormat("have asset(s) download fail:{0}\n", ErrorList.Aggregate("", (retStr, data) => retStr += string.Format("{0},\n", data.localPath)));
                    Debug.LogError("Stop @DownloadAssetsProcedure");
                    OnError?.Invoke();
                }
            };
            queue.RunBatch(UpdateSharedData.Instance.DownloadList.Select(asset => GetUpdateFileAndSave(asset)));
        }

        private IEnumerator GetUpdateFileAndSave(AssetDownloadData asset)
        {
            DownloadHandlerFile downloadHandler = new DownloadHandlerFile(Path.Combine(EnvironmentVariables.ExportAssetsRoot, asset.localPath));
            using (UnityWebRequest uwr1 = UnityWebRequest.Get(asset.uri1))
            {
                uwr1.certificateHandler = new UpdateCertificateHandler();
                uwr1.downloadHandler = downloadHandler;
                uwr1.SendWebRequest();
                while (!uwr1.isDone)
                {
                    asset.progress = uwr1.downloadProgress;
                    yield return new WaitForEndOfFrame();
                }

                if (uwr1.isNetworkError)
                {
                    ErrorList.Add(asset);
                    yield break;
                }
                else if (uwr1.isHttpError)
                {
                    using (UnityWebRequest uwr2 = UnityWebRequest.Get(asset.uri2))
                    {
                        uwr2.certificateHandler = new UpdateCertificateHandler();
                        uwr2.downloadHandler = downloadHandler;
                        uwr2.SendWebRequest();
                        while (!uwr2.isDone)
                        {
                            asset.progress = uwr2.downloadProgress;
                            yield return new WaitForEndOfFrame();
                        }

                        if (uwr2.isNetworkError || uwr2.isHttpError)
                        {
                            ErrorList.Add(asset);
                            yield break;
                        }
                    }
                }
                else
                {
                    asset.isSuccess = true;
                }
            }
        }
    }

    public class NoUpdateFinalProcedure : BaseProcedure
    {
        public NoUpdateFinalProcedure(BaseProcedure next = null) : base(next) { }
        protected override bool CheckDependency()
        {
            return true;
        }
        protected override void HandleProcedure()
        {
            // 添加相关触发
            Debug.Log("no update done");
            PostProcedure();
        }
    }

    public class UpdateCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    public class AssetDownloadData
    {
        /// <summary>
        /// 远端uri地址
        /// </summary>
        public string uri1 { get; set; }
        /// <summary>
        /// 备用uri地址
        /// </summary>
        public string uri2 { get; set; }
        /// <summary>
        /// 下载文件大小，单位kb
        /// </summary>
        public long size { get; set; }
        /// <summary>
        /// 文件下载进度，单位百分比
        /// </summary>
        public float progress { get; set; }
        /// <summary>
        /// 需要保存的本地地址（相对于需要保存的根目录）
        /// </summary>
        public string localPath { get; set; }
        /// <summary>
        /// 资源下载并写入成功标记位
        /// </summary>
        public bool isSuccess { get; set; }

        public AssetDownloadData()
        {
            this.uri1 = string.Empty;
            this.uri2 = string.Empty;
            this.size = 0l;
            this.progress = 0f;
            isSuccess = false;
        }
    }

    public class DownloadPackageProcedure : BaseProcedure
    {
        private CoroutineQueue queue { get; set; }
        public DownloadPackageProcedure(BaseProcedure next) : base(next) { }
        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.Processor != null;
        }
        protected override void HandleProcedure()
        {
            string packageStr = (Resources.Load("Config/package") as TextAsset).text;
            UpdateSharedData.Instance.RemotePackage = JsonMapper.ToObject(packageStr);
            queue = new CoroutineQueue(100, UpdateSharedData.Instance.Processor.StartCoroutine);
            queue.OnBatchComplete += () =>
            {
                Debug.Log("Download Package Done!");
                PostProcedure();
            };
            List<string> temp = new List<string>();
            foreach (var item in UpdateSharedData.Instance.RemotePackage["packages"])
            {
                Debug.Log(item);
                temp.Add(item.ToString());
            }
            if (!Directory.Exists(EnvironmentVariables.UnzipAssetsRoot))
            {
                Directory.CreateDirectory(EnvironmentVariables.UnzipAssetsRoot);
            }
            if (!Directory.Exists(EnvironmentVariables.UnzipAssetsTemp))
            {
                Directory.CreateDirectory(EnvironmentVariables.UnzipAssetsTemp);
            }
            queue.RunBatch(temp.Select(packageUrl => GetPackageAndSave(packageUrl)));
        }
        private IEnumerator GetPackageAndSave(string packageUrl)
        {
            DownloadHandlerFile downloadHandler = new DownloadHandlerFile(Path.Combine(EnvironmentVariables.UnzipAssetsTemp, Path.GetFileName(new Uri(packageUrl).LocalPath)));
            Debug.Log(Path.Combine(EnvironmentVariables.UnzipAssetsTemp, Path.GetFileName(new Uri(packageUrl).LocalPath)));
            using (UnityWebRequest uwr = UnityWebRequest.Get(packageUrl))
            {
                uwr.certificateHandler = new UpdateCertificateHandler();
                uwr.downloadHandler = downloadHandler;
                uwr.SendWebRequest();
                while (!uwr.isDone)
                {
                    Debug.LogFormat("{0} processing @{1}", packageUrl, uwr.downloadProgress);
                    yield return new WaitForSeconds(1);
                }
                Debug.Log(packageUrl + "DONE!!!!!!!!!!!!!!!!!");
            }
        }

        
    }

    public class UnzipProcedure : BaseProcedure
    {
        public UnzipProcedure(BaseProcedure next = null) : base(next) { }

        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.RemotePackage != null;
        }

        protected override void HandleProcedure()
        {
            DirectoryInfo di = new DirectoryInfo(EnvironmentVariables.UnzipAssetsTemp);
            foreach (FileInfo zipedFile in di.GetFiles("*.zip"))
            {
                ExtractZipFile(zipedFile.FullName, EnvironmentVariables.UnzipAssetsRoot);
                File.Delete(zipedFile.FullName);
            }
            Debug.Log("unzip complete!!!!!!!!!!");
            PostProcedure();
        }

        private void ExtractZipFile(string archiveFilenameIn, string outFolder, string password = null)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!string.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    string entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    string fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }
    }

    public class DownloadNativeProcedure : BaseProcedure
    {
        public DownloadNativeProcedure(BaseProcedure next) : base(next) { }

        protected override bool CheckDependency()
        {
            return UpdateSharedData.Instance.DownloadList != null
                && UpdateSharedData.Instance.Activity != null;
        }

        protected override void HandleProcedure()
        {
            
        }
    }
}
