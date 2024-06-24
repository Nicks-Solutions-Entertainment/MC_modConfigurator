using System.Collections.Generic;
using System.Linq;

public static class ApplicationExtensions
{
    public static bool HasDependentsOnInstance(this CF_ModpackManifest.CF_FileInfos file, CF_RunetimeProfileInfos runetimeProfile )
    {
        return runetimeProfile.installedAddons.Where(_addon => 
            _addon.installedFile.projectId != file.projectID
            && _addon.installedFile.DependsOf(file.projectID))
            .Count() > 0;
    }
}