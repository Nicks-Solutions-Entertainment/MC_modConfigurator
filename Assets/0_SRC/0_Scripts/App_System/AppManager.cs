using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance;

    public static readonly Dictionary<string, CF_RunetimeProfileInfos> RunetimeInstances = new Dictionary<string, CF_RunetimeProfileInfos>();

    private void Awake() => Instance = this;


    #region trigger events
    public delegate void OnStartAppAction();


    public event OnStartAppAction OnStartApp;

    #endregion
    static string ModpackInfosJSON => "modpackInfos.json";

    public bool TryGetModPacksettings(string modpackFolderPath, out ModpackSettings infos)
    {
        string modpackInfosJsonPath = Path.Combine(modpackFolderPath, ModpackInfosJSON);
        infos = new ModpackSettings();
        if (File.Exists(modpackInfosJsonPath))
        {
            string jsonContent = File.ReadAllText(modpackInfosJsonPath);
            infos = JsonUtility.FromJson<ModpackSettings>(jsonContent);
            return true;
        }
        return false;
    }

    public async void StartApp()
    {
        string[] _instancesPaths = CourseForgeConnector.GetInstancesOnFolder(ApplicationData.instancesPath);


        RunetimeInstances.Clear();

        foreach (var _path in _instancesPaths)
        {
            string _relativeInstancePath = Path.GetRelativePath(ApplicationData.instancesPath, _path); ;
            //Debug.Log($"*Loading Instance:'{_relativeInstancePath}' \n path '{_path}'");
            //_relativeInstancePath = Path.GetDirectoryName(_relativeInstancePath);

            Debug.Log($"Loading Instance:'{_relativeInstancePath}' \n path '{_path}'");
            var _runetimeInstance = await CourseForgeConnector.GetRunetimeInstanceInfos(_path);

            RunetimeInstances.Add(_relativeInstancePath, _runetimeInstance);
            Debug.Log($"<color=green>SUCCESS</color> added Instance:{_relativeInstancePath}");

            OnStartAppInternal();
        }
    }

    internal void OnStartAppInternal()
    {
        OnStartApp?.Invoke();

        //here
        /*
             Continuar daqui, iniciando o app por aqui 
            e pegando as informacoes do caminho relativo da instancia para referenciar ao Dicionario acima;

            o mesmo deve ser usado para procurar co-dependentes ao desativar um addon;
        */
    }

    public ModpackSettings CreateAndGetModPackSettings(CF_RunetimeProfileInfos runetimeInfos)
    {
        MODPAK_LOADDER _loadder = MODPAK_LOADDER.Uknown;

        if (runetimeInfos.baseModLoader.name.Contains("forge"))
            _loadder = MODPAK_LOADDER.Forge;
        else if (runetimeInfos.baseModLoader.name.Contains("fabric"))
            _loadder = MODPAK_LOADDER.Fabric;

        string _relativeInstancePath = Path.GetRelativePath(ApplicationData.instancesPath, runetimeInfos.installPath); ;
        _relativeInstancePath = Path.GetDirectoryName(_relativeInstancePath);

        ModpackSettings infos = new ModpackSettings()
        {
            name = runetimeInfos.name,
            richName = runetimeInfos.name,
            author = "authorNickname",
            version = "1.0",
            loadder = _loadder,
            minecraftVersion = runetimeInfos.gameVersion,
            performanceProfiles = new Dictionary<PERF_PROFILE_ORE, PerformanceProfile>(),
            addons = new List<AddonFileInfos>(),
            relativeInstancePath = _relativeInstancePath
        };

        List<string> registeredMods = new List<string>();
        infos.addons = new List<AddonFileInfos>();
        //adicionando addons oficiais
        foreach (var addon in runetimeInfos.installedAddons)
        {
            var addonFileInfos = new AddonFileInfos(addon);
            addonFileInfos.hasDependents = runetimeInfos.TryGetDependentAddons(addon.addonID, out var dependents);

            infos.addons.Add(addonFileInfos);
            string addonFile = Path.GetFileName(addon.fileNameOnDisk);
            Debug.Log($"CreateAndGetModPackSettings :: adding internal mod:{addonFile}");
            registeredMods.Add(Path.GetFileName(addon.fileNameOnDisk));
        }

        //adicionando mods externos
        {
            string modsFolder = Path.Combine(runetimeInfos.installPath, "mods");
            string[] allModPaths = Directory.GetFiles(modsFolder).Where(p => Path.GetExtension(p).CompareTo(".jar") == 0).ToArray();
            for (int i = 0; i < allModPaths.Length; i++)
            {
                string _filePath = allModPaths[i];
                string _addonFilename = Path.GetFileName(_filePath);
                if (!registeredMods.Contains(_addonFilename))
                {
                    Debug.Log($"CreateAndGetModPackSettings :: adding external mod:{_addonFilename}");
                    infos.addons.Add(new AddonFileInfos()
                    {
                        addonName = _addonFilename,
                        fileName = _addonFilename,
                        addonAuthor = "UKNOWN",
                        categoryClassID = CF_RunetimeProfileInfos.AddonCategory.MOD,
                        cf_fileInfos = new CF_ModpackManifest.CF_FileInfos()
                        {
                            fileID = -i,
                            projectID = -i,
                            required = true,
                        },
                        hasDependents = false,
                        required = true,
                    });

                }
            }
        }

        string infosJson = JsonUtility.ToJson(infos, true);
        string pathOfInfosJsonFile = Path.Combine(runetimeInfos.installPath, ModpackInfosJSON);
        //File.WriteAllText(pathOfInfosJsonFile, infosJson);

        return infos;
    }


    //continuar pegando Thumb do perfil,
    //criando funcao que checa dependencias entre addons, e exibe um alerta ao tentar desabilitar fora de ordem,
    // e fazendo habilitar/desabilitar conforme confirmado.
    //
    /// <summary>
    /// ativa addons com duas respecitivas dependencias
    /// <br></br>
    /// ou desativa apos desativar addons que dependem de si.
    /// </summary>
    /// <param name="relativeInstancePath"></param>
    /// <param name="addon"></param>
    public void SetAddonOnInstanceActive(string relativeInstancePath, long addonId, bool state)
    {
        if (!RunetimeInstances.TryGetValue(relativeInstancePath, out CF_RunetimeProfileInfos rInstance)) return;
        string pathOfFile = "";
        if (rInstance.TryGetInstalledAddonByFileId(addonId, out CF_RunetimeProfileInfos.InstalledAddon _installedAddon))
        {
            if (state)
            {

                //checando por dependencias e ativando:
                foreach (var dependency in _installedAddon.installedFile.dependencies)
                {
                    if (rInstance.TryGetInstalledAddonByFileId(dependency.addonId, out CF_RunetimeProfileInfos.InstalledAddon _dependencyAddon))
                    {
                        pathOfFile = Path.Combine(rInstance.installPath, _dependencyAddon.addonFolder, _dependencyAddon.fileNameOnDisk);
                        //SetAddonFileState(pathOfFile, state);
                        //SetAddonFileState(pathOfFile, _installedAddon.fileNameOnDisk, state);
                    }
                }
            }
            else
            {
                //checando dependentes e desativando:
                if (TryGetDependentAddons(relativeInstancePath, addonId, out List<long> dependents))
                {
                    foreach (var dependentAddon in dependents)
                    {
                        if (rInstance.TryGetInstalledAddonByFileId(dependentAddon, out CF_RunetimeProfileInfos.InstalledAddon _dependentAddon))
                        {
                            pathOfFile = Path.Combine(rInstance.installPath, _dependentAddon.addonFolder, _dependentAddon.fileNameOnDisk);
                            //SetAddonFileState(pathOfFile, state);
                        }
                        //dependentAddon.
                    }

                }
            }

            pathOfFile = Path.Combine(rInstance.installPath, _installedAddon.addonFolder);
            SetAddonFileState(pathOfFile, _installedAddon.fileNameOnDisk, state);
        }
    }

    void SetAddonFileState(string addonFolder,string addonFileNameOnDisk, bool state)
    {
        string fileName_noEXT = Path.GetFileNameWithoutExtension(addonFileNameOnDisk);
        string pathToFile = Path.Combine(addonFolder, addonFileNameOnDisk);
        string disabled_pathToFile = Path.Combine(addonFolder, $"{fileName_noEXT}.disabled");
        string enabled_pathToFile = Path.Combine(addonFolder, addonFileNameOnDisk);
        if (state)
        {
            if(File.Exists(disabled_pathToFile))
                Debug.Log($"SetAddonFileState({state}) :: {disabled_pathToFile} \n to {enabled_pathToFile}");
            else if(File.Exists(enabled_pathToFile))
                Debug.Log($"SetAddonFileState({state}) :: already right! \n ({enabled_pathToFile})");
        }
        else
        {
            if (File.Exists(enabled_pathToFile))
                Debug.Log($"SetAddonFileState({state}) :: {enabled_pathToFile} \n to {disabled_pathToFile}");
            else if (File.Exists(disabled_pathToFile))
                Debug.Log($"SetAddonFileState({state}) :: already right! \n ({disabled_pathToFile})");
        }
    }

    public bool TryGetDependentAddons(string relativePath, long addonId, out List<long> dependentAddonsID)
    {
        dependentAddonsID = new List<long>();

        return RunetimeInstances.TryGetValue(relativePath, out CF_RunetimeProfileInfos _rInstance)
                && _rInstance.TryGetDependentAddons(addonId, out dependentAddonsID);
    }

    public void SaveModpackSettings(ModpackSettings modpackSettings, string modpackInsallpath)
    {
        string infosJson = JsonUtility.ToJson(modpackSettings, true);
        string pathOfInfosJsonFile = Path.Combine(modpackInsallpath, ModpackInfosJSON);
        File.WriteAllText(pathOfInfosJsonFile, infosJson);

    }

    public void ApplySelectedProfile(ModpackSettings _settings, PerformanceProfile perfProfile)
    {

        foreach (CF_ModpackManifest.CF_FileInfos addon in perfProfile.addons)
        {
            #region Renaming addon_file
            if (_settings.TryGetAddonFileInfos(addon.fileID, out var addonInfos))
            {
                string enabledFilePath = Path.Combine(addonInfos.addonFolder, addonInfos.enabledFilename);
                string disabledFilePath = Path.Combine(addonInfos.addonFolder, addonInfos.disabledFilename);

                string designedFileName = addonInfos.designedFileName;
                string designedFilePath = Path.Combine(addonInfos.addonFolder, designedFileName);

                if (File.Exists(enabledFilePath) && designedFileName.CompareTo(enabledFilePath) != 0)
                {
                    File.Move(enabledFilePath, designedFileName);
                }
                else if (File.Exists(disabledFilePath) && designedFileName.CompareTo(disabledFilePath) != 0)
                {
                    File.Move(disabledFilePath, designedFileName);
                }
                else
                    Debug.LogWarning($"Addon File {addonInfos.addonName} does not exist or already was renamed.");
            }
            #endregion


        }
    }



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
