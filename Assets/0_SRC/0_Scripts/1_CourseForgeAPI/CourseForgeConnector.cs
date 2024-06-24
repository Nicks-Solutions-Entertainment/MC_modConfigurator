using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


public class CourseForgeConnector : MonoBehaviour
{
    //$2a$10$xX2i07NqAmP9JIw/VpNO3uRXnSkwpRJKnWrFXsucwp4hbwGGCSkNW
    [SerializeField] string filePath = "";
    [SerializeField,TextArea(1,4)] string runetimeInstanceJson = "";
    [SerializeField] string m_apiKey = "";
    string m_endPoint_url = "";
    [SerializeField] long m_modUID = 0;
    [SerializeField] CF_RunetimeProfileInfos runetimeProfile = new CF_RunetimeProfileInfos();
    [SerializeField] CF_GamesListResponse courseForgegames = new CF_GamesListResponse();
    [SerializeField] List<CF_ModInfo> modListFromManifest = new List<CF_ModInfo>();
    [SerializeField] string m_modSearched = "";
    [SerializeField] string m_modListPath = "";


    public string ApiKey
    {
        get => m_apiKey;set =>m_apiKey = value;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    [ContextMenu("TestUserFolder")]
    void TestUserFolder()
    {
        Debug.Log($"USERPROFILE: {System.Environment.GetEnvironmentVariable("USERPROFILE")}");
    }
    [ContextMenu("RequestGameList")]
    public void RequestGameList()
    {
        StartCoroutine(_RequestGameList());

    }

    [ContextMenu("RequestModInfo")]
    void RequestModInfo()
    {
        StartCoroutine(_RequestModInfo(m_modUID));
    }
    public void RequestModInfo(long modUID) => _RequestModInfo(m_modUID);

    [ContextMenu("Get MODs from Profile")]
    void ReadModpackFromCourseforgeInstance() => ReadModpackFromCourseforgeInstanceFile(filePath);
    public async void ReadModpackFromCourseforgeInstanceFile(string instancePath)
    {
        string _manifestContent = "";
        string _pathToExtract = Path.Combine(
                Application.temporaryCachePath,
                "manifest.json"
            );
        if (File.Exists(instancePath))
        {
            using (ZipArchive zip = ZipFile.OpenRead(instancePath))
                foreach (ZipArchiveEntry entry in zip.Entries)
                    if (entry.Name == "manifest.json")
                    {
                        //if (!File.Exists(_pathToExtract))
                        //    File.Create(_pathToExtract);
                        //entry.ExtractToFile("myfile");
                        entry.ExtractToFile(_pathToExtract,true);
                        _manifestContent = await File.ReadAllTextAsync(_pathToExtract);
                        File.Delete(_pathToExtract);
                        break;
                    }
            //--------
            CF_ModpackManifest _profile = JsonUtility.FromJson<CF_ModpackManifest>(_manifestContent);

            StartCoroutine(_RequestModListFromCourseForgeProfile(_profile));
        }
        else
            Debug.LogError($"Arquivo nao existe no caminho especificado ({instancePath}).");
    }

    [ContextMenu("ReadRunetimeInstance")]
    void ReadRunetimeInstanceJson() => ReadRunetimeInstanceJson(runetimeInstanceJson);
    public async void ReadRunetimeInstanceJson(string runetimeTinstancePath)
    {
        string _profileContent = await File.ReadAllTextAsync(runetimeTinstancePath);

        runetimeProfile = JsonUtility.FromJson<CF_RunetimeProfileInfos>(_profileContent);
        if(runetimeProfile.manifest!=null)
            StartCoroutine(_RequestModListFromCourseForgeProfile(runetimeProfile.manifest));

        //runetimeProfile.SetVersion();
    }

    public static CF_ModpackManifest GenerateModpackManifestFromInstanceData(CF_RunetimeProfileInfos instanceData)
    {
        CF_ModpackManifest.CF_McProfileInfos _minecraftInfos = new CF_ModpackManifest.CF_McProfileInfos()
        {
            version = instanceData.baseModLoader.minecraftVersion,
            modLoaders = new List<CF_ModpackManifest.CF_McProfileInfos.ModLoader>()
            {
                new CF_ModpackManifest.CF_McProfileInfos.ModLoader(instanceData.baseModLoader.version.id,true)
            },
            
        };

        CF_ModpackManifest manifest = new CF_ModpackManifest()
        {
            name = instanceData.name,
            author = ApplicationData.modpackAuthor,
            minecraft = _minecraftInfos,
            version = ApplicationData.modpackVersion,
            manifestType = "minecraftModpack",
            files = new List<CF_ModpackManifest.CF_FileInfos>(),
            overrides = "overrides",
        };

        foreach (var mod in instanceData.installedAddons)
        {
            //var _modInfo = 
            manifest.files.Add(new CF_ModpackManifest.CF_FileInfos()
            {
                fileID = mod.installedFile.id,
                projectID = mod.installedFile.projectId
            });
        }
        return manifest;
    }

    public static async Task<CF_RunetimeProfileInfos> GetRunetimeInstanceInfos(string runetimeTinstanceFolder)
    {
        string _pathOfInstanceJson = Path.Combine(runetimeTinstanceFolder, "minecraftinstance.json");
        //Debug.Log($"_pathOfInstanceJson:{_pathOfInstanceJson}");
        string _profileContent = await File.ReadAllTextAsync(_pathOfInstanceJson);

        return JsonUtility.FromJson<CF_RunetimeProfileInfos>(_profileContent);
    }

    public static string[] GetInstancesOnFolder(string instancesFolder)
    {
        if (!string.IsNullOrEmpty(instancesFolder) && Directory.Exists(instancesFolder))
            return Directory.GetDirectories(instancesFolder);
        else
            return new string[0];
    }

    public CF_ModpackManifest CreateManifestFromInstanceInfos(CF_RunetimeProfileInfos runetimeInstanceInfos)
    {
        CF_ModpackManifest _modpackManifest = new CF_ModpackManifest()
        {
            minecraft = new CF_ModpackManifest.CF_McProfileInfos()
            {
                version = runetimeInstanceInfos.gameVersion,
                modLoaders = new List<CF_ModpackManifest.CF_McProfileInfos.ModLoader>()
                {
                    new CF_ModpackManifest.CF_McProfileInfos.ModLoader
                    (runetimeInstanceInfos.baseModLoader.name,true)
                }
            },
            version = "1.0",
            manifestVersion = 1,
            name = runetimeInstanceInfos.name,
            author = "TEST",
            description = "",
            files = new List<CF_ModpackManifest.CF_FileInfos>(),
        };

        foreach (var addon in runetimeInstanceInfos.installedAddons)
        {
            bool _required = true;
            string _addonFileName = Path.GetFileNameWithoutExtension(addon.installedFile.fileName);
            string _addonFilePath_enabled = Path.Combine(runetimeInstanceInfos.installPath, addon.addonFolder, addon.installedFile.fileName);
            string _addonFilePath_disabled = Path.Combine(runetimeInstanceInfos.installPath, addon.addonFolder, $"{_addonFileName}.disabled");

            if (!File.Exists(_addonFilePath_enabled) && File.Exists(_addonFilePath_disabled))
                _required = false;

            _modpackManifest.files.Add(new CF_ModpackManifest.CF_FileInfos()
            {
                projectID = addon.addonID,
                fileID = addon.installedFile.id,
                required = _required
            });
        }
        return _modpackManifest;
        //string _manifestJson = JsonUtility.ToJson()
    }
    IEnumerator _RequestModListFromCourseForgeProfile(CF_ModpackManifest profile)
    {
        string _requestUrlBase = $"https://api.curseforge.com/v1/mods/";
        modListFromManifest.Clear();

        foreach (var _file in profile.files)
        {
            
            yield return  _RequestModInfo(_file.projectID,_file.required);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator _RequestModInfo(long _uid,bool isEnabled = true)
    {
        //long gameId = 0;
        //x-api-key
        string _modSlug = m_modSearched.Replace(' ', '-');
        string _slug = $"&slug={_modSlug}";

        //HtmlDocument document = new HtmlDocument();
        //document.Load(filePath);

        Debug.Log($"{_slug}");
        UnityWebRequest _webRequest = UnityWebRequest.Get($"https://api.curseforge.com/v1/mods/{_uid}");
        _webRequest.SetRequestHeader("x-api-key", m_apiKey);
        yield return _webRequest.SendWebRequest();

        switch (_webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                string _resultText = _webRequest.downloadHandler.text;
                ModInfoResponse _modInfo = JsonUtility.FromJson<ModInfoResponse>(_resultText);
                _modInfo.data.enabled = isEnabled;
                modListFromManifest.Add(_modInfo.data);
                Debug.Log($"MOD {_modInfo.data.id}({_modInfo.data.slug}) obtido.");
                break;
            default:
                Debug.Log($"result :: {_webRequest.result} \n {_webRequest.downloadHandler.text}");
                break;
        }
        yield return null;
    }

    async Task<ModInfoResponse> GetModInfo(long uid)
    {
        ModInfoResponse _response = null;
        string _modSlug = m_modSearched.Replace(' ', '-');
        string _slug = $"&slug={_modSlug}";

        Debug.Log($"{_slug}");
        UnityWebRequest _webRequest = UnityWebRequest.Get($"https://api.curseforge.com/v1/mods/{uid}");
        _webRequest.SetRequestHeader("x-api-key", m_apiKey);
         _webRequest.SendWebRequest();

        while (!_webRequest.isDone)
            await Task.Delay(1);

        switch (_webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                string _resultText = _webRequest.downloadHandler.text;
                _response = JsonUtility.FromJson<ModInfoResponse>(_resultText);
                //_modInfo.data.enabled = isEnabled;
                modListFromManifest.Add(_response.data);
                Debug.Log($"MOD {_response.data.id}({_response.data.slug}) obtido.");
                break;
            default:
                Debug.Log($"result :: {_webRequest.result} \n {_webRequest.downloadHandler.text}");
                break;
        }
        
        return _response;
    }

    IEnumerator _RequestGameList()
    {

        UnityWebRequest _webRequest = UnityWebRequest.Get($"https://api.curseforge.com/v1/games/");
        _webRequest.SetRequestHeader("x-api-key", m_apiKey);

        yield return _webRequest.SendWebRequest();
        switch (_webRequest.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.Success:
                string _resultText = _webRequest.downloadHandler.text;
                courseForgegames = JsonUtility.FromJson<CF_GamesListResponse>(_resultText);
                Debug.Log($"{_resultText}");
                break;
            case UnityWebRequest.Result.ConnectionError:
                break;
            case UnityWebRequest.Result.ProtocolError:
                break;
            case UnityWebRequest.Result.DataProcessingError:
                break;
            default:
                break;
        }
        yield return null;
    }


    
}
