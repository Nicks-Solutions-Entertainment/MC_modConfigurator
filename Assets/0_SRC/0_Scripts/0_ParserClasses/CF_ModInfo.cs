using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class CF_Content
{
    public string name;
    public long id;
    public string slug;
}

[Serializable]
public class CF_GameInfo : CF_Content
{
    public DateTime dateModified;
    public CF_GameAssetsInfos assets;
    public int status;
    public int apiStatus;
}

[Serializable]
public class CF_GameContent : CF_Content
{
    public long gameId;
    public long classId;
}

[Serializable]
public class CF_ModInfo : CF_GameContent
{
    public bool enabled;
    public ModLinks links;
    public string summary;
    public MOD_STATUS status;
    public int downloadCount;
    public bool isFeatured;
    public int primaryCategoryId;
    public List<ModCategory> categories;
    public List<GameContentAuthor> authors;
    public int mainFileId;
    

    [Serializable]
    public class ModLinks
    {
        public string websiteUrl;
        public string wikiUrl;
        public string issuesUrl;
        public string sourceUrl;
    }
    [Serializable]
    public class ModCategory : CF_GameContent
    {
        public string url;
        public DateTime dateModified;
        public bool isClass;
        public long parentCategoryId;
    }
    


    public enum MOD_STATUS
    {
        UNDEFINED = 0,
        New,

        ChangesRequired,

        UnderSoftReview,

        Approved,

        Rejected,

        ChangesMade,

        Inactive,

        Abandoned,

        Deleted,

        UnderReview
    }
}

[Serializable]
public class CF_ModFileInfo_Basic
{
    public int id;
    public int gameId;
    public int modId;
    public bool isAvailable;
    public string displayName;
    public string downloadUrl;
    public int downloadCount;
    public List<CF_ModFileInfo.CF_ModFileDependence> dependencies;
    public bool isServerPack;

    public CF_ModFileInfo_Basic(int _id, int _gameId, int _modId, bool _isAvailable, string _displayName, string _downloadUrl, int _downloadCount, List<CF_ModFileInfo.CF_ModFileDependence> _dependencies, bool _isServerPack)
    {
        id = _id;
        gameId = _gameId;
        modId = _modId;
        isAvailable = _isAvailable;
        displayName = _displayName;
        downloadUrl = _downloadUrl;
        downloadCount = _downloadCount;
        dependencies = _dependencies;
        isServerPack = _isServerPack;
    }
}
[Serializable]
public class CF_ModFileInfo
{
    public int id;
    public int gameId;
    public int modId;
    public string displayName;
    public bool isAvailable;
    public string fileName;
    public int releaseType;
    public MODFILE_STATUS fileStatus;
    public List<CF_FileHash> hashes;
    public DateTime fileDate;
    public int fileLenght;
    public int downloadCount;
    public int fileSizeOnDisk;
    public string downloadUrl;
    public List<string> gameVersions;
    public List<CF_ModFileDependence> dependencies;
    public bool exposeAsAlternative;
    public int parentProjectFileId;
    public int alternateFileId;
    public bool isServerPack;
    public int serverPackFileId;
    public bool isEarlyAccessContent;
    public DateTime earlyAccessEndDate;
    public int fileFingerprint;
    public List<CF_ModFileModule> modules;

    public CF_ModFileInfo_Basic basicInfos => new CF_ModFileInfo_Basic
        (
        id,gameId,modId,isAvailable,downloadUrl,downloadUrl,downloadCount,dependencies.ToList(),isServerPack
        );

    [Serializable]
    public class CF_FileHash
    {
        public string value;
        public int algo;

    }

    [Serializable]
    public class CF_ModFileSortableGameVersion
    {
        public string gameVersionName;
        public string gameVersionPadded;
        public string gameVersion;
        public DateTime gameVersionReleaseDate;
        public int gameVersionTypeId;
    }

    [Serializable]
    public class CF_ModFileDependence
    {
        public long modId;
        public AddonDependencyRelactionType relactionType;

    }

    [Serializable]
    public class CF_ModFileModule
    {
        public int name;
        public int fingerprint;
    }

    public enum MODFILE_STATUS
    {
        UNDEFINED = 0,
        Processing,
        ChangesRequired,
        UnderReview,
        Approved,
        Rejected,
        MalwareDetected,
        Deleted,
        Archived,
        Testing,
        Released,
        ReadyForReview,
        Deprecated,
        Baking,
        AwaitingPublishing,
        FailedPublishing
    }
}

public enum AddonDependencyRelactionType
{
    EmbeddedLibrary = 1,
    OptionalDependency,
    RequiredDependency,
    Tool,
    Incompatible,
    Include
}


[Serializable]
public class GameContentAuthor
{
    public long id;
    public string name;
    public string url;
}

[Serializable]
public class CF_GameAssetsInfos
{
    public string iconUrl;
    public string tileUrl;
    public string coverUrl;
}

[Serializable]
public class CF_GamesListResponse
{
    public List<CF_GameInfo> data;

    [Serializable]
    public class Pagination
    {
        public long index;
        public int pageSize;
        public int resultCount;
        public int totalCount;
    }
}

