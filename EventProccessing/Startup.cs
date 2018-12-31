using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventProccessing.Hubs;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventProccessing
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services
                   .AddSignalR(options => options.SupportedProtocols = new List<string> { "messagepack" })
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
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSignalR(routes =>
            {
                routes.MapHub<DefaultHub>("/ws");
            });
        }
    }
}
