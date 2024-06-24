using System.IO;
using UnityEngine;

public static class ApplicationData
{
    static string DEFAULT_INSTANCES_FOLDER=>
            Path.Combine(System.Environment.GetEnvironmentVariable("USERPROFILE"),$"curseforge",$"minecraft",$"Instances") ;

    public static bool HasInitialSettup => PlayerPrefs.HasKey("MMPM_InstancesPath");

    public static string instancesPath
    {
        get
        {
            if (PlayerPrefs.HasKey("MMPM_InstancesPath"))
                return PlayerPrefs.GetString("MMPM_InstancesPath");
            else
                return DEFAULT_INSTANCES_FOLDER;
        }
        set => PlayerPrefs.SetString("MMPM_InstancesPath", value);
    }

    public static string modpackVersion
    {
        get
        {
            if (PlayerPrefs.HasKey("MMPM_ModpackVersion"))
                return PlayerPrefs.GetString("MMPM_ModpackVersion");
            else
                return "1.0.0";
        }
        set => PlayerPrefs.SetString("MMPM_ModpackVersion", value);
    }
    public static string modpackAuthor
    {
        get
        {
            if (PlayerPrefs.HasKey("MMPM_ModpackAuthor"))
                return PlayerPrefs.GetString("MMPM_ModpackAuthor");
            else
                return "";
        }
        set => PlayerPrefs.SetString("MMPM_ModpackAuthor", value);
    }
}