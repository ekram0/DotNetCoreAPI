using AutoMapper;
using CoreCodeCamp.Controllers;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreCodeCamp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<ICampRepository, CampRepository>();
            services.AddDbContext<CampContext>();

            //services.AddDbContext<CampContext>(sp => sp.UseSqlServer(Configuration.GetConnectionString("CodeCamp")));

            services.AddAutoMapper(typeof(Startup));

            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
                opt.Conventions.Controller<TalksController>()
                .HasApiVersion(new ApiVersion(2, 0));
                //opt.ApiVersionReader = new UrlSegmentApiVersionReader();
                //opt.ApiVersionReader = ApiVersionReader.Combine(
                //     new QueryStringApiVersionReader("v","ver"),
                //     new HeaderApiVersionReader("v")
                //    );
            });

            services.AddHttpContextAccessor();

            services.AddControllersWithViews();
            services.AddRazorPages();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endPoints =>
            {
                endPoints.MapControllers();

                //endPoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller}/{action?}/{id?}");

                endPoints.MapRazorPages();
            });

        }
    }
}
