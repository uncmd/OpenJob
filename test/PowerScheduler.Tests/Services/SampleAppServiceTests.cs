using Volo.Abp.Identity;

namespace PowerScheduler.Tests.Services;

public class SampleAppServiceTests : PowerSchedulerTestBase
{
    private readonly IIdentityUserAppService _userAppService;

    public SampleAppServiceTests()
    {
        _userAppService = GetRequiredService<IIdentityUserAppService>();
    }

    [Fact]
    public async Task Initial_Data_Should_Contain_Admin_User()
    {
        var result = await _userAppService.GetListAsync(new GetIdentityUsersInput());

        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(u => u.UserName == "admin");
    }
}
