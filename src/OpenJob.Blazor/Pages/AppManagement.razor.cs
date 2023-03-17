using Blazorise;
using OpenJob.Apps;
using OpenJob.Localization;
using OpenJob.Permissions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace OpenJob.Blazor.Pages;

public partial class AppManagement
{
    protected PageToolbar Toolbar { get; } = new();

    private List<TableColumn> AppManagementTableColumns => TableColumns.Get<AppManagement>();

    public AppManagement()
    {
        //ObjectMapperContext = typeof(OpenJobBlazorModule);
        LocalizationResource = typeof(OpenJobResource);

        CreatePolicyName = OpenJobPermissions.Apps.Create;
        UpdatePolicyName = OpenJobPermissions.Apps.Update;
        DeletePolicyName = OpenJobPermissions.Apps.Delete;
    }

    protected virtual async Task OnSearchTextChanged(string value)
    {
        GetListInput.Filter = value;
        CurrentPage = 1;
        await GetEntitiesAsync();
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:OpenJob"].Value));
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Apps"].Value));
        return base.SetBreadcrumbItemsAsync();
    }

    protected override string GetDeleteConfirmationMessage(AppInfoDto entity)
    {
        return string.Format(L["AppDeletionConfirmationMessage"], entity.Name);
    }

    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<AppManagement>()
            .AddRange(new EntityAction[]
            {
                    new EntityAction
                    {
                        Text = L["Edit"],
                        Visible = (data) => HasUpdatePermission,
                        Clicked = async (data) => await OpenEditModalAsync(data.As<AppInfoDto>())
                    },
                    new EntityAction
                    {
                        Text = L["Delete"],
                        Visible = (data) => HasDeletePermission && data.As<AppInfoDto>().Name != "Default",
                        Clicked = async (data) => await DeleteEntityAsync(data.As<AppInfoDto>()),
                        ConfirmationMessage = (data) => GetDeleteConfirmationMessage(data.As<AppInfoDto>())
                    }
            });

        return base.SetEntityActionsAsync();
    }

    protected override ValueTask SetTableColumnsAsync()
    {
        AppManagementTableColumns
            .AddRange(new TableColumn[]
            {
                    new TableColumn
                    {
                        Title = L["Actions"],
                        Actions = EntityActions.Get<AppManagement>(),
                    },
                    new TableColumn
                    {
                        Title = L["Name"],
                        Data = nameof(AppInfoDto.Name),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = L["Description"],
                        Data = nameof(AppInfoDto.Description),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = L["IsEnabled"],
                        Data = nameof(AppInfoDto.IsEnabled),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = L["CreationTime"],
                        Data = nameof(AppInfoDto.CreationTime),
                        Sortable = true,
                    }
            });

        AppManagementTableColumns.AddRange(GetExtensionTableColumns(OpenJobModuleExtensionConsts.ModuleName,
            OpenJobModuleExtensionConsts.EntityNames.App));
        return base.SetEntityActionsAsync();
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewApp"], OpenCreateModalAsync,
            IconName.Add,
            requiredPolicyName: CreatePolicyName);

        return base.SetToolbarItemsAsync();
    }
}
