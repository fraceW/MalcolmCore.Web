using Autofac;
using Autofac.Extensions.DependencyInjection;
using MalcolmCore.Data;
using MalcolmCore.Utils;
using MalcolmCore.Utils.Caches;
using MalcolmCore.Utils.Common;
using MalcolmCore.Utils.Filter;
using MalcolmCore.Utils.Swaggers;
using MalcolmCore.WebApi.AutoFac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MalcolmCore.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(swagger => 
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                // 映射生成注释
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);

                //swagger.OperationFilter<AddAuthTokenHeaderParameter>();
            });

            services.AddControllersWithViews();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = ContextBoundObject => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //添加数据库
            services.AddDbContext<CoreFrameDBContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("MysqlConnection")
                , Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql")));

            services.AddDirectoryBrowser();

            //内置依赖注入
            //services.AddTransient<IBaseService, BaseService>();
            //添加缓存
            //services.AddCacheStrategy("Memory");
            services.AddSingleton<MemoryCacheHelp>();

            services.AddSession();
            services.AddLogStrategy();
            //跨域
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(build =>
                {
                    build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc(o =>
            {
                //全局过滤器
                o.Filters.Add(typeof(JwtCheckApi));
            });
            
            services.AddHttpContextAccessor();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // 直接用Autofac注册我们自定义的 
            builder.RegisterModule(new DefaultModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(useswaggerui => 
            {
                useswaggerui.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "api/{controller}/{action}/{id?}");
            });
        }
    }
}
