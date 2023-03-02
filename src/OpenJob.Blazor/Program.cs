using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenJob.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var application = await builder.AddApplicationAsync<OpenJobBlazorModule>(options =>
{
    options.UseAutofac();
});

var host = builder.Build();

await application.InitializeApplicationAsync(host.Services);

await host.RunAsync();
