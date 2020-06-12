using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebsiteWorkshop.Modules.UpdateModule;

public class TestUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //NewUpdateSchedule();
        //OldUpdateSchedule();
        //AllUpdateSchedule();
        NativeUpdateSchedule();
    }


    public void NewUpdateSchedule()
    {
        UpdateSharedData.Instance.Processor = this;

        NoUpdateFinalProcedure noUpdateFinalProcedure = new NoUpdateFinalProcedure();
        DownloadAssetsProcedure downloadAssetsProcedure = new DownloadAssetsProcedure();
        AnalyzeCatalogProcedure analyzeCatalogProcedure = new AnalyzeCatalogProcedure(downloadAssetsProcedure);
        DownloadCatalogProcedure downloadCatalogProcedure = new DownloadCatalogProcedure(analyzeCatalogProcedure);
        ConfirmDownloadProcedure confirmDownloadProcedure = new ConfirmDownloadProcedure(downloadCatalogProcedure, noUpdateFinalProcedure);
        RemoteProcedure remoteProcedure = new RemoteProcedure(confirmDownloadProcedure);
        LocalProcedure localProcedure = new LocalProcedure(remoteProcedure);

        localProcedure.StartProcedure();
    }

    public void OldUpdateSchedule()
    {
        UpdateSharedData.Instance.Processor = this;

        UnzipProcedure unzipProcedure = new UnzipProcedure();
        DownloadPackageProcedure downloadPackageProcedure = new DownloadPackageProcedure(unzipProcedure);

        downloadPackageProcedure.StartProcedure();
    }

    public void AllUpdateSchedule()
    {
        UpdateSharedData.Instance.Processor = this;

        NoUpdateFinalProcedure noUpdateFinalProcedure = new NoUpdateFinalProcedure();
        DownloadAssetsProcedure downloadAssetsProcedure = new DownloadAssetsProcedure();
        AnalyzeCatalogProcedure analyzeCatalogProcedure = new AnalyzeCatalogProcedure(downloadAssetsProcedure);
        DownloadCatalogProcedure downloadCatalogProcedure = new DownloadCatalogProcedure(analyzeCatalogProcedure);
        ConfirmDownloadProcedure confirmDownloadProcedure = new ConfirmDownloadProcedure(downloadCatalogProcedure, noUpdateFinalProcedure);
        RemoteProcedure remoteProcedure = new RemoteProcedure(confirmDownloadProcedure);
        LocalProcedure localProcedure = new LocalProcedure(remoteProcedure);

        UnzipProcedure unzipProcedure = new UnzipProcedure(localProcedure);
        DownloadPackageProcedure downloadPackageProcedure = new DownloadPackageProcedure(unzipProcedure);

        downloadPackageProcedure.StartProcedure();
    }

    public void NativeUpdateSchedule()
    {
        UpdateSharedData.Instance.Processor = this;

        NoUpdateFinalProcedure noUpdateFinalProcedure = new NoUpdateFinalProcedure();
        NativeDownloadProcedure nativeDownloadProcedure = new NativeDownloadProcedure();
        AnalyzeCatalogProcedure analyzeCatalogProcedure = new AnalyzeCatalogProcedure(nativeDownloadProcedure);
        DownloadCatalogProcedure downloadCatalogProcedure = new DownloadCatalogProcedure(analyzeCatalogProcedure);
        ConfirmDownloadProcedure confirmDownloadProcedure = new ConfirmDownloadProcedure(downloadCatalogProcedure, noUpdateFinalProcedure);
        RemoteProcedure remoteProcedure = new RemoteProcedure(confirmDownloadProcedure);
        LocalProcedure localProcedure = new LocalProcedure(remoteProcedure);

        localProcedure.StartProcedure();
    }

    public void OnDownloadDone()
    {
        Debug.Log("download done");
    }
}
