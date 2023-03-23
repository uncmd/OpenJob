using OpenJob.Apps;
using Shouldly;
using Volo.Abp.Validation;
using Xunit;

namespace OpenJob.Jobs;

public class AppInfoAppService_Tests : OpenJobApplicationTestBase
{
    private readonly IAppInfoAppService _appInfoService;

    public AppInfoAppService_Tests()
    {
        _appInfoService = GetRequiredService<IAppInfoAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Apps()
    {
        //Act
        var result = await _appInfoService.GetListAsync(
            new GetAndFilterListDto()
        );

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(b => b.Name == "Default");
    }

    [Fact]
    public async Task Should_Create_A_Valid_App()
    {
        //Act
        var result = await _appInfoService.CreateAsync(
            new AppInfoDto
            {
                Name = "New test app",
                Description = "a test app",
                IsEnabled = true,
            }
        );

        //Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("New test app");
    }

    [Fact]
    public async Task Should_Not_Create_A_App_Without_Name()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _appInfoService.CreateAsync(
                new AppInfoDto
                {
                    Name = "",
                    Description = "without name",
                    IsEnabled = true,
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }
}
