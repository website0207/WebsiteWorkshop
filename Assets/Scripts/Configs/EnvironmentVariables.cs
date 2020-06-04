using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class EnvironmentVariables
{
    private static string _ExportAssetsRoot;
    public static string ExportAssetsRoot
    {
        get
        {
            if (string.IsNullOrEmpty(_ExportAssetsRoot))
            {
                _ExportAssetsRoot = Path.Combine(Application.persistentDataPath, "UpdateAssets");
            }
            return _ExportAssetsRoot;
        }
    }

    private static string _ExportAssetsTemp;
    public static string ExportAssetsTemp
    {
        get
        {
            if (string.IsNullOrEmpty(_ExportAssetsTemp))
            {
                _ExportAssetsTemp = Path.Combine(Application.persistentDataPath, "UpdateAssets", "temp");
            }
            return _ExportAssetsTemp;
        }
    }

    private static string _LocalVersionPath;
    public static string LocalVersionPath
    {
        get
        {
            if (string.IsNullOrEmpty(_LocalVersionPath))
            {
                _LocalVersionPath = Path.Combine(Application.persistentDataPath, "UpdateAssets", "version.json");
            }
            return _LocalVersionPath;
        }
    }

    private static string _LocalCatalogPath;
    public static string LocalCatalogPath
    {
        get
        {
            if (string.IsNullOrEmpty(_LocalCatalogPath))
            {
                _LocalCatalogPath = Path.Combine(Application.persistentDataPath, "UpdateAssets", "catalog.json");
            }
            return _LocalCatalogPath;
        }
    }

    private static string _RemoteCatalogPath;
    public static string RemoteCatalogPath
    {
        get
        {
            if (string.IsNullOrEmpty(_RemoteCatalogPath))
            {
                _RemoteCatalogPath = Path.Combine(Application.persistentDataPath, "UpdateAssets", "temp", "catalog.json");
            }
            return _RemoteCatalogPath;
        }
    }

    private static string _ServerlistPath;
    public static string ServerlistPath
    {
        get
        {
            if (string.IsNullOrEmpty(_ServerlistPath))
            {
                _ServerlistPath = Path.Combine(Application.persistentDataPath, "serverlist.json");
            }
            return _ServerlistPath;
        }
    }

    private static string _AnnouncementPath;
    public static string AnnouncementPath
    {
        get
        {
            if (string.IsNullOrEmpty(_AnnouncementPath))
            {
                _AnnouncementPath = Path.Combine(Application.persistentDataPath, "announcement.json");
            }
            return _AnnouncementPath;
        }
    }
}
