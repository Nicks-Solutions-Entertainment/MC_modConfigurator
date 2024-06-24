using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CF_ModpackManifest
    
{
    public CF_McProfileInfos minecraft;
    public string manifestType = "minecraftModpack";
    public int manifestVersion;
    public string name;
    public string version;
    public string author;
    public string description;
    public long projectID;
    public List<CF_FileInfos> files;
    public string overrides = "overrides";

    [Serializable]
    public class CF_FileInfos
    {
        public long projectID;//addonId
        public long fileID;//installed.id
        public bool required;

        
    }

    [Serializable]
    public class CF_McProfileInfos
    {
        public string version;
        public string additionalJavaArgs = null;
        public List<ModLoader> modLoaders;
        public string libraries = null;
        [Serializable]
        public class ModLoader
        {
            public string id;
            public bool primary;
        
            public ModLoader(string id, bool primary)
            {
                this.id = id;
                this.primary = primary;
            }
            public ModLoader()
            {
                
            }
        }
    }

    public bool HasAuthorRegistered()
    {
        return this!=null && !string.IsNullOrEmpty(this.author);
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
    public CF_ModpackManifest manifest;
    public DateTime lastPlayed;
    public int playedCount;
    public string installPath;
    public List<InstalledAddon> installedAddons;

    public bool TryGetInstalledAddonByFileId(long fileId, out InstalledAddon addon)
    {
        Debug.Log($"TryGetInstalledAddon({fileId})");
        addon = installedAddons.Where(item => item.installedFile.id == fileId ).FirstOrDefault();
        return addon.addonID!=0 && addon.installedFile.id == fileId;
    }

    public bool TryGetDependentAddons(long addonId, out List<long> dependentAddonsID)
    {
        dependentAddonsID = new List<long>();

        {
            dependentAddonsID = installedAddons.Where(_addon =>
            _addon.installedFile.projectId != addonId
            && _addon.installedFile.DependsOf(addonId)).Select(_addon => _addon.addonID).ToList();
        }

        return dependentAddonsID.Count > 0;
    }

    [Serializable]
    public class InstalledAddon
    {
        public string name;
        public string instanceID;
        public long addonID;
        public long gameID;
        public AddonCategory categoryClassID;
        public string fileNameOnDisk;
        public string primaryAuthor;
        public int primaryCategoryId;
        public string webSiteURL;
        public string thumbnailUrl;
        public InstalledFileBasicInfos installedFile;
        public bool enabled;
        public string addonFolder
        {
            get
            {
                switch (categoryClassID)
                {
                    case AddonCategory.MOD:
                        {
                            return "mods";
                        }
                    case AddonCategory.RESOURCEPACK:
                        {
                            return "resourcepacks";

                        }
                    case AddonCategory.SHADERPACK:
                        {
                            return "shaderpacks";
                        }
                    default:
                        return "mods";
                }
            }
        }

        [Serializable]
        public class InstalledFileBasicInfos
        {
            public long id;
            public long projectId;
            public string fileName;
            public string fileNameOnDisk;
            public long fileLenght;
            public string downloadUrl;
            public List<AddonFileDependency> dependencies = new List<AddonFileDependency>();
            public DateTime fileDate;

            /// <summary>
            /// retorna se tal addon esta listado entre as dependencias obrigatorias.
            /// </summary>
            /// <param name="addonId"></param>
            /// <returns></returns>
            public bool DependsOf(long addonId)
            {
                return dependencies.Select(dep=> dep.addonId).Contains(addonId);
            }
        }
        

    }
    [Serializable]
    public class AddonFileDependency
    {
        public long addonId;
        public AddonDependencyRelactionType relactionType;

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

    public enum AddonCategory
    {
        MOD = 6,
        RESOURCEPACK = 12,
        SHADERPACK = 6552
    }

}

