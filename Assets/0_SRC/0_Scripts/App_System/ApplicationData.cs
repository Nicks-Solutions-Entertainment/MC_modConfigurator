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
            // DEFAULT uses %USERPROFILE%...
            //System.Environment.GetEnvironmentVariable("USERPROFILE") <- try it
        }
        set => PlayerPrefs.SetString("MMPM_InstancesPath", value);
    }
}