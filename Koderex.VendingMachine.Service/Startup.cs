using Autofac;
using Autofac.Extensions.DependencyInjection;
using Koderex.VendingMachine.Implementation;
using Koderex.VendingMachine.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Xml.XPath;

namespace Koderex.VendingMachine.Service {
    public class Startup {
        private const string API_DESCRIPTION = "[Koderex] : [VendingMachine]";
        private const string CORS = "koderex";
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfigurationRoot ConfigurationRoot { get; }
        public IContainer Container { get; set; }
        public Startup(IHostingEnvironment hostingEnvironment) {
            _hostingEnvironment = hostingEnvironment;
            var builder = new ConfigurationBuilder()
                   .SetBasePath(_hostingEnvironment.ContentRootPath)
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.json", optional: true)
                   .AddEnvironmentVariables();
            ConfigurationRoot = builder.Build();
        }
        public IServiceProvider ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors(x => {
                x.AddPolicy(CORS, xx => xx.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });
            services.AddSwaggerGen(x => {
                x.SwaggerDoc("v1", new Info { Title = API_DESCRIPTION, Version = "v1" });
            });
            buildAutofacContainer(services);
            return new AutofacServiceProvider(Container);
        }
        private void buildAutofacContainer(IServiceCollection services) {
            var builder = new ContainerBuilder();
            builder.Register(x => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()).As<IConfigurationRoot>().SingleInstance();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
            builder.RegisterType<VendingMachineService>().As<IVendingMachineService>();
            builder.Populate(services);
            this.Container = builder.Build();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseCors(CORS);
            app.UseMvc();
            // this is for when deployed to a production environment on kubernetes, or service fabric so that the path can be resolved by swagger.
#if !DEBUG
            app.UseSwagger(x => { x.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.BasePath = "/vending-machine"); }); 
#else
            app.UseSwagger();
#endif
            app.UseSwaggerUI(x => { x.SwaggerEndpoint(ConfigurationRoot["Swagger:Path"], API_DESCRIPTION); });
            applicationLifetime.ApplicationStopped.Register(() => Container.Dispose());
        }
    }
}
