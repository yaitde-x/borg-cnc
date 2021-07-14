using Borg.Machine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yuni.Library;
using Yuni.Settings;

namespace borg_api
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
            var yuniRoot = Configuration["YuniRoot"];

            services.AddSingleton<ApplicationSettings>(new ApplicationSettings(yuniRoot));
            services.AddSingleton<GrblResponseParser>();
            services.AddSingleton<IRS274Interpreter, GenericRS274Interpreter>();
            services.AddSingleton<IRS274Controller, PythonApiMachineController>();
            services.AddSingleton<IMachine, Machine>();
            services.AddSingleton<RS274MetaBuilder>();
            services.AddSingleton<ILibraryRepo>((sp) => new FileLibraryRepo(yuniRoot));

            services.AddHttpClient();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.EnvironmentName.Equals("dev"))
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
