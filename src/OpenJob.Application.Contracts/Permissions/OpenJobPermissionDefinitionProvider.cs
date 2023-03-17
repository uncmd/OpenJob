using OpenJob.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace OpenJob.Permissions;

public class OpenJobPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var openJobGroup = context.AddGroup(OpenJobPermissions.GroupName, L("Permission:OpenJob"));

        var appsPermission = openJobGroup.AddPermission(OpenJobPermissions.Apps.Default, L("Permission:AppManagement"));
        appsPermission.AddChild(OpenJobPermissions.Apps.Create, L("Permission:Create"));
        appsPermission.AddChild(OpenJobPermissions.Apps.Update, L("Permission:Edit"));
        appsPermission.AddChild(OpenJobPermissions.Apps.Delete, L("Permission:Delete"));

        var jobsPermission = openJobGroup.AddPermission(OpenJobPermissions.Jobs.Default, L("Permission:JobManagement"));
        jobsPermission.AddChild(OpenJobPermissions.Jobs.Create, L("Permission:Create"));
        jobsPermission.AddChild(OpenJobPermissions.Jobs.Update, L("Permission:Edit"));
        jobsPermission.AddChild(OpenJobPermissions.Jobs.Delete, L("Permission:Delete"));

        var tasksPermission = openJobGroup.AddPermission(OpenJobPermissions.Tasks.Default, L("Permission:TaskManagement"));
        tasksPermission.AddChild(OpenJobPermissions.Tasks.Create, L("Permission:Create"));
        tasksPermission.AddChild(OpenJobPermissions.Tasks.Update, L("Permission:Edit"));
        tasksPermission.AddChild(OpenJobPermissions.Tasks.Delete, L("Permission:Delete"));

        var workersPermission = openJobGroup.AddPermission(OpenJobPermissions.Workers.Default, L("Permission:WorkerManagement"));
        workersPermission.AddChild(OpenJobPermissions.Workers.Create, L("Permission:Create"));
        workersPermission.AddChild(OpenJobPermissions.Workers.Update, L("Permission:Edit"));
        workersPermission.AddChild(OpenJobPermissions.Workers.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OpenJobResource>(name);
    }
}
