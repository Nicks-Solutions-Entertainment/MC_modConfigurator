using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ModpackSettings
{
    public string name;
    public string richName;
    public string author;
    public string minecraftVersion;
    public string version;
    public MODPAK_LOADDER loadder;
    public string thumbImageName;
    public Texture2D thumbTexture;
    public string relativeInstancePath;

    public List<AddonFileInfos> addons;
    public Dictionary<PERF_PROFILE_ORE,PerformanceProfile> performanceProfiles;

    public bool TryGetAddonFileInfos(long fileId,out AddonFileInfos addonInfos)
    {
        addonInfos = addons.Where(item=> item.cf_fileInfos.fileID == fileId).FirstOrDefault();

        return addonInfos.cf_fileInfos.fileID == fileId;
    }

    public void ImportThumb(string instanceFolder)
    {
        //importing thumbItem
        {
            string _fullPath = Path.Combine(instanceFolder, thumbImageName);
            if (File.Exists(_fullPath))
            {
                ImageConversion.LoadImage(thumbTexture, File.ReadAllBytes(_fullPath));

            }
        }
    }

    public void ApplyRichName(string _richName)
    {
        string _name = Regex.Replace(_richName,"<.*?>", "");
        name = _name;
        richName = _richName;
    }

    public void SelectPerformanceProfile(PERF_PROFILE_ORE profile_ore)
    {
        if (performanceProfiles.TryGetValue(profile_ore, out PerformanceProfile profile))
        {
            profile.selected = true;
            AppManager.Instance.ApplySelectedProfile(this,profile);
        }
    }

    internal void OnAddonStateChanged(long addonId, bool state)
    {
        AppManager.Instance.SetAddonOnInstanceActive(relativeInstancePath, addonId, state);
    }
}

[Serializable]
public class AddonFileInfos
{
    public string addonName;
    public string fileName;
    public string addonAuthor;
    public CF_ModpackManifest.CF_FileInfos cf_fileInfos;
    public bool required;
    public CF_RunetimeProfileInfos.AddonCategory categoryClassID;
    public List<CF_RunetimeProfileInfos.AddonFileDependency> dependencies = new List<CF_RunetimeProfileInfos.AddonFileDependency>();
    public bool hasDependents = false;
    public string addonFolder
    {
        get
        {
            switch (categoryClassID)
            {
                case CF_RunetimeProfileInfos.AddonCategory.MOD:
                    {
                        return "mods";
                    }
                case CF_RunetimeProfileInfos.AddonCategory.RESOURCEPACK:
                    {
                        return "resourcepacks";

                    }
                case CF_RunetimeProfileInfos.AddonCategory.SHADERPACK:
                    {
                        return "shaderpacks";
                    }
                default:
                    return "mods";
            }
        }
    }

    public string enabledFilename => $"{fileName}.jar";
    public string disabledFilename => $"{fileName}.disabled";
    public string designedFileName
    {
        get
        {
            return enabled ? enabledFilename : disabledFilename;
        }
    }


    public bool externalFile 
    {
        get => cf_fileInfos.fileID<=0 && cf_fileInfos.projectID<=0; 
    }

    public bool enabled
    {
        get=> cf_fileInfos.required;
        set=> cf_fileInfos.required = value;
    }

    public AddonFileInfos()
    {
    }
    public AddonFileInfos(CF_RunetimeProfileInfos.InstalledAddon _installedAddon)
    {
        long _fileId = _installedAddon.installedFile.id;
        long _projectId = _installedAddon.installedFile.projectId;
        addonName = _installedAddon.name;
        addonAuthor = _installedAddon.primaryAuthor;
        fileName = Path.GetFileNameWithoutExtension(_installedAddon.fileNameOnDisk);
        cf_fileInfos = new CF_ModpackManifest.CF_FileInfos()
        {
            fileID = _fileId, projectID = _projectId,required = true
        };
        dependencies = _installedAddon.installedFile.dependencies.ToList();
        categoryClassID = _installedAddon.categoryClassID;
        required = true;
    }
}
public enum MODPAK_LOADDER
{
    Uknown,
    Forge,
    Fabric
}