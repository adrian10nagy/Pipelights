using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Pipelights.Database.Services;
using Pipelights.Website.BusinessService;
using Pipelights.Website.Services;
using Pipelights.Website.Services.Interfaces;

namespace Pipelights.Website
{
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
            services.AddControllersWithViews();

            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddSession();

            services.AddSingleton<IOrderDbService>(InitializeCosmosClientInstanceAsyncOrder(Configuration.GetSection("OrderDb")).GetAwaiter().GetResult());
            services.AddSingleton<ILampDbService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("LampDb")).GetAwaiter().GetResult());
            services.AddTransient<ILampService, LampService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICartService, CartService>();
            services.AddTransient<IBlobService, BlobService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IOrderService, OrderService>();

            var serviceBusEndpoint = "DefaultEndpointsProtocol=https;AccountName=samasterpocpipe;AccountKey=R1E85Qr4D5KBWHt+iPC+IFgGumV3aT7J92T3mqE2ki1/vynN4PkNSuvp9XA9uk3FpnH/7tPYuDuzK0ONu8VypA==;EndpointSuffix=core.windows.net";
            services.AddSingleton(d => new BlobServiceClient(serviceBusEndpoint));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static async Task<LampDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];
            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
            var cosmosDbService = new LampDbService(client, databaseName, containerName);

            return cosmosDbService;
        }

        private static async Task<OrderDbService> InitializeCosmosClientInstanceAsyncOrder(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];
            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
            var cosmosDbService = new OrderDbService(client, databaseName, containerName);

            return cosmosDbService;
        }
    }
}
