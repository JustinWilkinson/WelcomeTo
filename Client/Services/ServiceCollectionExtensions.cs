﻿using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WelcomeTo.Client.Services.SignalR;

namespace WelcomeTo.Client.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHttpService(this IServiceCollection services, string baseAddress) => services.TryAddSingleton<IHttpService>(new HttpService(baseAddress));

        public static void AddBlazorTimer(this IServiceCollection services) => services.TryAddTransient<IBlazorTimer, BlazorTimer>();

        public static void AddHubCommunicator<T>(this IServiceCollection services) where T : HubCommunicator => services.TryAddTransient<T>();

        public static void AddGameStorage(this IServiceCollection services)
        {
            services.AddStorage();
            services.TryAddScoped<GameStorage>();
        }
    }
}