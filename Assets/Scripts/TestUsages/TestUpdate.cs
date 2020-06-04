using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebsiteWorkshop.Modules.UpdateModule;

public class TestUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

}
