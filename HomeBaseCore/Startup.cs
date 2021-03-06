﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chip.Identity.Common.Extensions;
using HomeBaseCore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace HomeBaseCore
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
            services.AddMvc();
			services.AddChipIdentity("media", "secret", new string[0], "http://potatochip.io/Identity");
			//services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			//	.AddCookie();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

			if (Directory.Exists(FileStorage.ContentDirectory) == false)
				Directory.CreateDirectory(FileStorage.ContentDirectory);

			if (Directory.Exists(FileStorage.FileDirectory) == false)
				Directory.CreateDirectory(FileStorage.FileDirectory);

			//app.UseAuthentication();

			app.UseChipIdentity();

			app.UseStaticFiles();
			app.UseStaticFiles(new StaticFileOptions {
				FileProvider = new PhysicalFileProvider(FileStorage.ContentDirectory),
				RequestPath = "/Content"
			});

			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
