using System;
using System.Collections.Generic;
using System.Net;
using Events.Hubs;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Events
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSignalR(options => options.SupportedProtocols = new List<string> {"messagepack"})
                .AddMessagePackProtocol()
                .AddRedis(_config["REDIS"]);
            services.AddSingleton<IEventStoreConnection>(c =>
            {
                var conn = EventStoreConnection.Create(new Uri(_config["ENVENTSTORE_URI"]));
                conn.ConnectAsync().Wait();
                return conn;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSignalR(routes =>
            {
                routes.MapHub<DefaultHub>("/ws");
            });
        }
    }
}
