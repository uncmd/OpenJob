using Volo.Abp.Settings;

namespace OpenJob.Settings;

public class OpenJobSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(OpenJobSettings.MySetting1));
    }
}
