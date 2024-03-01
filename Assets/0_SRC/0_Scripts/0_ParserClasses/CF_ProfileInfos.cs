using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CF_ExportedProfileInfos
    
{
    public CF_McProfileInfos minecraft;
    public string manifestType;
    public int manifestVersion;
    public string name;
    public string version;
    public string author;
    public List<CF_FileInfos> files;
    public string overrides;

    [Serializable]
    public class CF_FileInfos
    {
        public int projectID;
        public int fileID;
        public bool required;
    }

    [Serializable]
    public class CF_McProfileInfos
    {
        public string version;
        public List<ModLoader> modLoaders;

        [Serializable]
        public class ModLoader
        {
            public string id;
            public bool primary;
        }
    }
}

[Serializable]
public class CF_RunetimeProfileInfos
{
    public string name;
    public Guid guid;
    public int gameTypeID;
    public string gameVersion;
    public int allocatedMemory;

    public ModLoader baseModLoader;
    public Version version;
    public DateTime lastPlayed;
    public string installPath;
    public List<InstalledAddon> installedAddons;

    
    [Serializable]
    public class InstalledAddon
    {
        public string name;
        public string instanceID;
        public int gameID;
        public int categoryClassID;
        public string fileNameOnDisk;
    }

    [Serializable]
    public class ModLoader
    {
        public string forgeVersion;
        public string name;
        public int type;
        public string downloadUrl;
        public string filename;
        public int installMethod;
        public bool latest;
        public bool recommended;
        public string versionJson 
        {
            get => JsonUtility.ToJson(version);
            set => version = JsonUtility.FromJson<Version>(value);
        }
        public Version version;
        public string librariesInstallLocation;
        public string minecraftVersion;


        [Serializable]
        public class Version
        {
            public string id;
            public DateTime time;
            public DateTime releaseTime;
            public string release;
            public string mainClass;
            public string minimumLauncherVersion;
        }

    }
}

