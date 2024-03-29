﻿using Microsoft.OpenApi.Models;
using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using SampleWebApi.Models;
using SampleWebApi.Services;
using Serilog;

namespace SampleWebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var logger = AppLog.Logger;

        var asmSerilogAspNetCore = new AssemblyInfo(typeof(SerilogApplicationBuilderExtensions).Assembly);
        logger.LogInformation("AssemblyTitle:{title}; Version:{version} Framework:{framework}",
            asmSerilogAspNetCore.Title, asmSerilogAspNetCore.InformationalVersion, asmSerilogAspNetCore.FrameworkName);

        var asmOptions = new AssemblyInfo(typeof(OptionsServiceCollectionExtensions).Assembly);
        logger.LogInformation("AssemblyTitle:{title}; Version:{version} Framework:{framework}",
            asmOptions.Title, asmOptions.InformationalVersion, asmOptions.FrameworkName);

        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SampleWebApi", Version = "v1" });
        });

        services.AddEventAggregator();

        // Register singleton EA subscriber services
        services.AddSingleton<RootEvent1Subscriber>();      // IAsyncSubscriber<TEvent>
        services.SubscribeAsync<Event1, RootEvent1Subscriber>(lifetime: SubscriberLifetime.Singleton);
        // or use delegate SubscribeAsync<TEvent>(delegate)
        services.SubscribeAsync<Event1>((ev, sp, ct) => sp.GetRequiredService<RootEvent1Subscriber>().HandleAsync(ev, ct), lifetime: SubscriberLifetime.Singleton);

        services.AddScoped<Event2Subscriber>();             // ISubscriber<TEvent>
        services.Subscribe<BaseEvent2, Event2Subscriber>();
        // or use delegate Subscribe<TEvent>(delegate)
        services.Subscribe<BaseEvent2>((ev, sp) => sp.GetRequiredService<Event2Subscriber>().Handle(ev));

        services.AddScoped<Event3Subscriber>();
        services.Subscribe<IEvent3, Event3Subscriber>();

        services.AddScoped<Event1Subscriber>();
        services.SubscribeAsync<Event1, Event1Subscriber>();

        services.AddSingleton<RootEvent2Subscriber>();
        services.AddSingleton<RootEvent3Subscriber>();
        services.SubscribeAsync<Event2, RootEvent2Subscriber>(lifetime: SubscriberLifetime.Singleton);
        services.Subscribe<IEvent3>((ev, sp) => sp.GetRequiredService<RootEvent3Subscriber>().Handle3(ev), lifetime: SubscriberLifetime.Singleton);

        services.AddTransient<TransientService1>();
        services.AddTransient<TransientService2>();
        services.AddSingleton<SingletonService1>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleWebApi v1"));
        }

        app.UseSerilogRequestLoggingLevel();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
