using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace OpenJob.Data;

public class SampleRepositoryTests : OpenJobTestBase
{
    private readonly IRepository<IdentityUser, Guid> _appUserRepository;

    public SampleRepositoryTests()
    {
        _appUserRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
    }

    [Fact]
    public async Task Should_Query_AppUser()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var adminUser = await (await _appUserRepository.GetQueryableAsync())
                .Where(u => u.UserName == "admin")
                .FirstOrDefaultAsync();

            adminUser.ShouldNotBeNull();
        });
    }
}
