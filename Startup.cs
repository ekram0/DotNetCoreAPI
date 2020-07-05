using AutoMapper;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
