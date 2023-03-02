using OpenJob.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace OpenJob.Permissions;

public class OpenJobPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(OpenJobPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(OpenJobPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OpenJobResource>(name);
    }
}
