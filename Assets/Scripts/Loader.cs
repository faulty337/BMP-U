﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using JLChnToZ.Toolset.Singleton;

public class Loader : SingletonBehaviour<Loader> {
    public static string songPath;
    public static bool autoMode = false;
    public static float speed = 1;

    public int sceneIndex;

    public BMS.BMSManager bmsManager;
    public BMS.NoteDetector noteDetector;

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    
	void Start () {
        switch(sceneIndex) {
            case 0: // Menu Scene
                break;
            case 1: // Game Scene
                StartCoroutine(LoadBMSCoroutine());
                break;
        }
	}

    IEnumerator LoadBMSCoroutine() {
        string bmsContent;
        var fileInfo = new FileInfo(songPath);
        using(var fs = fileInfo.OpenRead())
        using(var fsRead = new StreamReader(fs, SongInfoLoader.CurrentEncoding))
            bmsContent = fsRead.ReadToEnd();
        bmsManager.PreEventOffset = TimeSpan.FromSeconds(2 - speed);
        bmsManager.LoadBMS(bmsContent, fileInfo.Directory.FullName);
        while(!bmsManager.BMSLoaded) yield return null;

        bmsManager.ReloadBMS(BMS.BMSReloadOperation.Body);
        while(!bmsManager.BMSLoaded) yield return null;
        bmsManager.ReloadBMS(BMS.BMSReloadOperation.Resources);
        while(bmsManager.IsLoadingResources) yield return null;
        bmsManager.InitializeNoteScore();

        noteDetector.autoMode = autoMode;
        bmsManager.IsStarted = true;
    }
}