using Autofac;
using Autofac.Extensions.DependencyInjection;
using MalcolmCore.Data;
using MalcolmCore.IService;
using MalcolmCore.Service;
using MalcolmCore.Utils;
using MalcolmCore.Utils.Common;
using MalcolmCore.Utils.Filter;
using MalcolmCore.Web.AutoFac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MalcolmCore.Web
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
            services.AddControllersWithViews();
            services.Configure<CookiePolicyOptions>(options => 
            {
                options.CheckConsentNeeded = ContextBoundObject => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //������ݿ�
            services.AddDbContext<CoreFrameDBContext>(options => 
                options.UseMySql(Configuration.GetConnectionString("MysqlConnection")
                , Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql")));

            services.AddDirectoryBrowser();

            services.AddLogStrategy();

            //��������ע��
            //services.AddTransient<IBaseService, BaseService>();
            //��ӻ���
            services.AddCacheStrategy("Memory");
            services.AddSession();
            //����
            services.AddCors(options => 
            {
                options.AddDefaultPolicy(build => 
                {
                    build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            services.AddSingleton<IHostedService, MalcolmCore.Utils.HostedServices.TokenRefreshService>();
            //services.AddScoped<SkipAttribute>();
            services.AddMvc(o =>
            {
                //ȫ�ֹ�����
                o.Filters.Add(typeof(CheckLogin));
            });
            services.AddHttpContextAccessor();
            
            //Autofac
            //var containerBuilder = new ContainerBuilder();
            //containerBuilder.RegisterModule<DefaultModule>();
            //containerBuilder.Populate(services);
            //var container = containerBuilder.Build();
            //return new AutofacServiceProvider(container);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // ֱ����Autofacע�������Զ���� 
            builder.RegisterModule(new DefaultModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //��������������ܵ�
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            

            //�Զ����м��
            //app.Use(async(context, next) => 
            //{
            //    await next.Invoke();
            //});
            //���þ�̬��Դ
            var download = Path.Combine(Directory.GetCurrentDirectory(), "DownLoad");
            if (!Directory.Exists(download))
            {
                //�����ڣ��򴴽���·��
                Directory.CreateDirectory(download);
            }
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(download),
                //�������·���������ǰ��������һ���ģ���ȻҲ�������ģ�ע��ǰ��Ҫ��/��
                RequestPath = "/DownLoad"
            });
            app.UseCookiePolicy();
            app.UseSession();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseDirectoryBrowser(new DirectoryBrowserOptions 
            {
                FileProvider = new PhysicalFileProvider(download),
                //�������·���������ǰ��������һ���ģ���ȻҲ�������ģ�ע��ǰ��Ҫ��/��
                RequestPath = "/DownLoad"
            });

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
