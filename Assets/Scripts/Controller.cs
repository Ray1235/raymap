﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using LibR3;

public class Controller : MonoBehaviour {
    public R3Loader.Mode mode = R3Loader.Mode.Rayman3PC;
    public string gameDataBinFolder;
    public string lvlName;

    public Material baseMaterial;
    public Material baseLightMaterial;
    public Material baseTransparentMaterial;
    public Material baseBlendMaterial;
    public Material baseBlendTransparentMaterial;
    public Material negativeLightProjectorMaterial;
    public SectorManager sectorManager;
    public LightManager lightManager;
    public bool allowDeadPointers = false;
    public bool forceDisplayBackfaces = false;
    R3Loader loader = null;

    // Use this for initialization
    void Start() {
        // Read command line arguments
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++) {
            switch (args[i]) {
                case "--lvl":
                case "-l":
                    lvlName = args[i + 1];
                    i++;
                    break;
                case "--folder":
                case "--directory":
                case "-d":
                case "-f":
                    gameDataBinFolder = args[i + 1];
                    i++;
                    break;
                case "--mode":
                case "-m":
                    switch (args[i + 1]) {
                        case "r3_gc":
                            mode = R3Loader.Mode.Rayman3GC; break;
                        case "ra_gc":
                            mode = R3Loader.Mode.RaymanArenaGC; break;
                        case "r3_pc":
                            mode = R3Loader.Mode.Rayman3PC; break;
                        case "ra_pc":
                            mode = R3Loader.Mode.RaymanArenaPC; break;
                    }
                    break;
            }
        }

        loader = R3Loader.Loader;
        loader.mode = mode;
        loader.gameDataBinFolder = gameDataBinFolder;
        loader.lvlName = lvlName;
        loader.allowDeadPointers = allowDeadPointers;
        loader.forceDisplayBackfaces = forceDisplayBackfaces;
        loader.baseMaterial = baseMaterial;
        loader.baseTransparentMaterial = baseTransparentMaterial;
        loader.baseBlendMaterial = baseBlendMaterial;
        loader.baseBlendTransparentMaterial = baseBlendTransparentMaterial;
        loader.negativeLightProjectorMaterial = negativeLightProjectorMaterial;
        loader.baseLightMaterial = baseLightMaterial;

        loader.Load();
        sectorManager.Init();
        lightManager.Init();
    }
	// Update is called once per frame
	void Update () {
	}

    public void SaveChanges() {
        if(loader != null) loader.Save();
    }
}