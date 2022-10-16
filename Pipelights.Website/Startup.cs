using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pipelights.Database.Services;
using Pipelights.Website.BusinessService;
using Pipelights.Website.Services;
using Pipelights.Website.Services.Interfaces;
using RazorHtmlEmails.RazorClassLib.Services;
using System.Threading.Tasks;

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
            services.AddSingleton<ICategoryDbService>(InitializeCosmosClientInstanceAsyncCategory(Configuration.GetSection("CategoryDb")).GetAwaiter().GetResult());
            services.AddTransient<ILampService, LampService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICartService, CartService>();
            services.AddTransient<IBlobService, BlobService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IOrderService, OrderService>();

            var serviceBusEndpoint = "DefaultEndpointsProtocol=https;AccountName=samasterpocpipe2;AccountKey=yuvUONwWzMN5YeCDGqWOFiCRdRKwOPmQyfCO7H9zKNZnIGBQJXvp8zh3TWwRSbG42tEsT6cjD82n+ASt9+7Zgw==;EndpointSuffix=core.windows.net";
            services.AddSingleton(d => new BlobServiceClient(serviceBusEndpoint));
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

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

        private static async Task<CategoryDbService> InitializeCosmosClientInstanceAsyncCategory(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];
            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
            var cosmosDbService = new CategoryDbService(client, databaseName, containerName);

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
