using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace JaegerTracing
{
    public static class OpenTelemetryServiceExtensions
    {
        public static IServiceCollection AddJaegerTracing(this IServiceCollection services, params string[] sources)
        {
            services.AddOpenTelemetryTracing((provider, builder) =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                
                var name = config.GetValue<string>("Jaeger:ServiceName");
                var host = config.GetValue<string>("Jaeger:Host");
                var port = config.GetValue<int>("Jaeger:Port");
                
                if (name is {Length: > 0} && host is {Length: > 0} && port > 0)
                {
                    builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(name))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddGrpcClientInstrumentation();

                    if (sources.Length > 0)
                    {
                        builder.AddSource(sources);
                    }

                    if (provider.GetService<IConnectionMultiplexer>() is { } c)
                    {
                        builder.AddRedisInstrumentation(c);
                    }
                    
                    builder.AddJaegerExporter(options =>
                        {
                            options.AgentHost = host;
                            options.AgentPort = port;
                        });
                }
            });
            
            return services;
        }
    }
}
