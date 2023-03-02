namespace OpenJob.VirtualFileSystem;

public class VirtualFileSystem_Tests : OpenJobTestBase
{
    [Fact]
    public async Task Get_Virtual_File()
    {
        var result = await GetResponseAsStringAsync(
            "/SampleFiles/test1.js"
        );

        result.ShouldBe("test1.js-content");
    }
}
