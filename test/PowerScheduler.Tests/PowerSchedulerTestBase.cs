using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Net;
using System.Text.Json;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Uow;

namespace PowerScheduler.Tests;

public class PowerSchedulerTestBase : PowerSchedulerTestBase<Startup>
{
    
}

public abstract class PowerSchedulerTestBase<TStartup> : AbpAspNetCoreIntegratedTestBase<TStartup>
    where TStartup : class
{
    protected virtual async Task<T> GetResponseAsObjectAsync<T>(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        var strResponse = await GetResponseAsStringAsync(url, expectedStatusCode);
        return JsonSerializer.Deserialize<T>(strResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    protected virtual async Task<string> GetResponseAsStringAsync(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        using (var response = await GetResponseAsync(url, expectedStatusCode))
        {
            return await response.Content.ReadAsStringAsync();
        }
    }

    protected virtual async Task<HttpResponseMessage> GetResponseAsync(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK, bool xmlHttpRequest = false)
    {
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
        {
            requestMessage.Headers.Add("Accept-Language", CultureInfo.CurrentUICulture.Name);
            if (xmlHttpRequest)
            {
                requestMessage.Headers.Add(HeaderNames.XRequestedWith, "XMLHttpRequest");
            }
            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.ShouldBe(expectedStatusCode);
            return response;
        }
    }

    protected virtual Task WithUnitOfWorkAsync(Func<Task> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task WithUnitOfWorkAsync(AbpUnitOfWorkOptions options, Func<Task> action)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                await action();

                await uow.CompleteAsync();
            }
        }
    }

    protected virtual Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(AbpUnitOfWorkOptions options, Func<Task<TResult>> func)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                var result = await func();
                await uow.CompleteAsync();
                return result;
            }
        }
    }
}
