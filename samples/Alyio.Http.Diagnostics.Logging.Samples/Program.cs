﻿// MIT License

using Alyio.Http.Diagnostics.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#if NET6_0
Console.WriteLine("NOT SUPPORTED NET 6.0");
#else
HostApplicationBuilder builder = Host.CreateApplicationBuilder();

// Configure the default HTTP client to log raw HTTP messages
// builder.Services.ConfigureHttpClientDefaults(builder =>
// {
//     builder.AddHttpRawMessageLogging(
//         categoryName: "Alyio.Http.Diagnostics.Samples",
//         logLevel: LogLevel.Information,
//         ignoreRequestContent: false,
//         ignoreResponseContent: false,
//         ignoreRequestHeaders: ["User-Agent"],
//         ignoreResponseHeaders: ["Date"]);
// });

builder.Services
    .AddHttpClient<IOpenWeatherMapService, OpenWeatherMapService>(client =>
    {
        client.BaseAddress = new Uri("http://samples.openweathermap.org");
    })
    .AddHttpRawMessageLogging(options =>
    {
        options.IgnoreRequestContent = false;
        options.IgnoreResponseContent = false;
    });

builder.Services.AddHostedService<OpenWeatherMapHostedService>();

builder.Logging
    .SetMinimumLevel(LogLevel.Warning)
    .AddFilter("System.Net.Http.HttpClient", LogLevel.Information);

IHost app = builder.Build();

await app.StartAsync();


sealed class OpenWeatherMapHostedService(IOpenWeatherMapService weatherMapService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return weatherMapService.GetAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

internal interface IOpenWeatherMapService
{
    Task GetAsync();
}

sealed class OpenWeatherMapService(HttpClient client) : IOpenWeatherMapService
{
    public Task GetAsync()
    {
        return client.GetAsync("/data/2.5/weather?q=London,uk&appid=b1b15e88fa797225412429c1c50c122a1");
    }
}
#endif
