using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class PerformanceProfile
{
    public string name;
    [HideInInspector]
    public string thumbItem_fileName;
    public Texture2D thumbItem;
    [HideInInspector]
    public string thumbBackground_fileName;

    public Texture2D thumbBackground;
    public PERF_PROFILE_MINERIUM profile_lv_minerium;
    public string description;
    public ProfileSpecs recomendedSpecs;
    public ProfileSpecs minimumSpecs;


    public void ImportImages(string instanceFolder)
    {
        //importing thumbItem
        {
            string _fullPath = Path.Combine(instanceFolder, thumbItem_fileName);
            if (File.Exists(_fullPath))
            {
                ImageConversion.LoadImage(thumbItem, File.ReadAllBytes(_fullPath));

            }
        }

        //importing thumbBackground
        {
            string _fullPath = Path.Combine(instanceFolder, thumbBackground_fileName);
            if (File.Exists(_fullPath))
            {
                ImageConversion.LoadImage(thumbBackground, File.ReadAllBytes(_fullPath));

            }
        }
    }

    [Serializable]
    public class ProfileSpecs
    {
        public string cpu;
        public string ram;
        public string vRam;
    }
}

public enum PERF_PROFILE_MINERIUM
{
    NONE,
    IRON,
    EMERALD,
    DIAMOND,
    COPER,
    NETHERITE,
    REDSTONE
}
