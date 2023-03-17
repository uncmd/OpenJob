using OpenJob.Localization;
using OpenJob.MultiTenancy;
using OpenJob.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace OpenJob.Blazor.Menus;

public class OpenJobMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<OpenJobResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                OpenJobMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 0
            )
        );

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "OpenJob",
                l["Menu:OpenJob"],
                icon: "fa " + Blazorise.Icons.FontAwesome.FontAwesomeIcons.Book
            ).AddItem(
                new ApplicationMenuItem(
                    "OpenJob.Apps",
                    l["Menu:Apps"],
                    url: "~/openjob/apps",
                    icon: "fa " + Blazorise.Icons.FontAwesome.FontAwesomeIcons.ListUl
                ).RequirePermissions(OpenJobPermissions.Apps.Default)
            ).AddItem(
                new ApplicationMenuItem(
                    "OpenJob.Jobs",
                    l["Menu:Jobs"],
                    url: "/jobs",
                    icon: "fa " + Blazorise.Icons.FontAwesome.FontAwesomeIcons.Coffee
                ).RequirePermissions(OpenJobPermissions.Jobs.Default)
            ).AddItem(
                new ApplicationMenuItem(
                    "OpenJob.Tasks",
                    l["Menu:Tasks"],
                    url: "/tasks",
                    icon: "fa " + Blazorise.Icons.FontAwesome.FontAwesomeIcons.Tasks
                ).RequirePermissions(OpenJobPermissions.Tasks.Default)
            ).AddItem(
                new ApplicationMenuItem(
                    "OpenJob.Workers",
                    l["Menu:Workers"],
                    url: "/workers",
                    icon: "fa " + Blazorise.Icons.FontAwesome.FontAwesomeIcons.Server
                ).RequirePermissions(OpenJobPermissions.Workers.Default)
            )
        );

        return Task.CompletedTask;
    }
}
