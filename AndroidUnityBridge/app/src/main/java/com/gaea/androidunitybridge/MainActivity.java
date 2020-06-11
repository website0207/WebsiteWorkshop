package com.gaea.androidunitybridge;

import android.os.Bundle;

import com.arialyy.aria.core.Aria;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Aria.download(this).register();
    }

    public void SetDownloaderArgs()
    {
        Aria.get(this).getDownloadConfig().setMaxSpeed(1);
        Aria.get(this).getDownloadConfig().setMaxTaskNum(1);
        Aria.get(this).getDownloadConfig().setThreadNum(1);
        Aria.get(this).getDownloadConfig().setUseBlock(false);
    }

    
}
