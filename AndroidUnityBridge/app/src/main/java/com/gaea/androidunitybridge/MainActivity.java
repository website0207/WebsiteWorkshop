package com.gaea.androidunitybridge;

import android.os.Bundle;

import com.arialyy.annotations.Download;
import com.arialyy.annotations.DownloadGroup;
import com.arialyy.aria.core.Aria;
import com.arialyy.aria.core.task.DownloadGroupTask;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import java.util.ArrayList;
import java.util.Collections;

public class MainActivity extends UnityPlayerActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Aria.download(this).register();
    }

    public void downloadFiles(String[] urls, String absoluteRoot, int totalBytes)
    {
        ArrayList<String> tempList = new ArrayList<String>(urls.length);
        Collections.addAll(tempList, urls);
        if (totalBytes == 0)
            Aria.download(this)
                    .loadGroup(tempList)
                    .setDirPath(absoluteRoot)
                    .unknownSize()
                    .create();
        else
            Aria.download(this)
                    .loadGroup(tempList)
                    .setDirPath(absoluteRoot)
                    .setFileSize(totalBytes)
                    .create();
    }

    @DownloadGroup.onTaskComplete()
    protected void taskComplete(DownloadGroupTask task)
    {
        UnityPlayer.UnitySendMessage("GameObject", "OnDownloadDone", "");
    }
}
