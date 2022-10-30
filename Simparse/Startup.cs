using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Simparse.Collections;
using Simparse.Domain;
using Simparse.FileStorage;
using Simparse.Identity;
using Simparse.Models;
using Simparse.Vision;

namespace Simparse
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            string mongoConnectionString = "";
            string mongoDBName = "";
            string googleProjectId = "";
            string googleIdentityAPIKey = "";

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoConnectionString);
            IMongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(mongoDBName);

            Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.MongoDB(db, Serilog.Events.LogEventLevel.Warning)
                        .CreateLogger();

            services.AddControllersWithViews();

            var googleCreds = GetGoogleCredential();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Simparse", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddTransient<IFileCollection, FileCollection>(fact =>
            {
                var collection = db.GetCollection<FileDataModel>("FileCollection");
                return new FileCollection(collection);
            });

            services.AddTransient<IFileMappingCollection, FileMappingCollection>(fact =>
            {
                var collection = db.GetCollection<FileMappingItem>("FileMappingCollection");
                return new FileMappingCollection(collection);
            });

            services.AddTransient<IFolderCollection, FolderCollection>(fact =>
            {
                var fileCollection = db.GetCollection<FolderDataModel>("FolderCollection");
                return new FolderCollection(fileCollection);
            });

            services.AddTransient<IFieldMappingDomain, FieldMappingDomain>();

            services.AddTransient<IGoogleVisionHandler, GoogleVisionHandler>(fact =>
            {
                return new GoogleVisionHandler(googleProjectId);
            });

            services.AddTransient<IFileAccessStore, FileAccessStore>(fact =>
            {
                StorageClient storageClient = StorageClient.Create(googleCreds);
                return new FileAccessStore(storageClient, googleProjectId);
            });

            services.AddTransient<IUserCollection, UserCollection>(fact =>
            {
                var mongoCollection = db.GetCollection<ApplicationUser>("UserCollection");
                return new UserCollection(mongoCollection);
            });

            services.AddTransient<IContactCollection, ContactCollection>(fact =>
            {
                var mongoCollection = db.GetCollection<ContactDataItem>("ContactUsCollection");
                return new ContactCollection(mongoCollection);
            });

            services.AddHttpClient("identity", config =>
            {
                config.BaseAddress = new Uri("https://identitytoolkit.googleapis.com");
            });

            services.AddTransient<ISimparseUserStore>(fact =>
            {
                var clientFactory = fact.GetService<IHttpClientFactory>();
                var client = clientFactory.CreateClient("identity");
                return new SimparseUserStore(client, googleIdentityAPIKey);
            });

            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simparse API");
            });
        }

        public static GoogleCredential GetGoogleCredential()
        {
            GoogleCredential googleCredential = GoogleCredential.GetApplicationDefault();
            return googleCredential;
        }
    }



}
