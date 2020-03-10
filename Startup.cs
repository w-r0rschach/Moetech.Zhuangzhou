using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Email;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Service;

namespace Moetech.Zhuangzhou
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        public IConfiguration Configuration { get; }

        // 此方法由运行时调用。使用此方法向容器添加服务。
        public void ConfigureServices(IServiceCollection services)
        {
            // 邮箱配置
            EmailConfig emailConfig = new EmailConfig();
            Configuration.GetSection("EmailConfig").Bind(emailConfig);
            EmailHelper email = new EmailHelper(emailConfig);

            // vSphere配置
            VSphereConfig sphereConfig = new VSphereConfig();
            Configuration.GetSection("VSphereConfig").Bind(sphereConfig);

            services.AddControllersWithViews();
            // 注册数据库上下文
            services.AddDbContext<VirtualMachineDB>(options => options.UseMySql(Configuration.GetConnectionString("LocalMysql")));
            //services.AddDbContext<VirtualMachineDB>(options => options.UseMySql(Configuration.GetConnectionString("MySqlDefault")));
            // HTML编码
            services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
            // 添加Session
            services.AddSession();
            // 注入接口
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IVmware, VmwareService>();
			services.AddScoped<IVmwareManage, VmwareManageService>();
            services.AddScoped<ILogs, LoggerService>();
            services.Configure<CookiePolicyOptions>(options =>
            {
               //这个lambda决定对于一个给定的请求是否需要用户对非必需cookie的同意。
               options.CheckConsentNeeded = context => false;//默认为true，改为false
               options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // 注入定时任务
            services.AddHostedService<StartTimedTask>();
            services.AddScoped<ITimedTask, TimedTaskService>();

        }

        // 此方法由运行时调用。使用此方法配置HTTP请求管道。
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error.cshtml");
                // 默认的HSTS值是30天。您可能希望针对生产场景更改此设置，请参见https://aka.ms/aspnetcore-hsts。
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization(); // 身份验证
            app.UseSession();
            //app.UseWebSockets();            //websocket
            //app.Use(async (context, next) =>
            //{
            //    if (context.WebSockets.IsWebSocketRequest)
            //    {
            //        using (IServiceScope scope = app.ApplicationServices.CreateScope())
            //        {
            //            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //            await SocketHandler.SocketConnect(context, webSocket, scope);
            //        }
            //    }
            //    else
            //    {
            //        await next();
            //    }
            //});
            app.Map("/WebSocketHandle", WebSocketHandle.Map);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Vmware}/{action=Index}/{id?}");
            });
        }
    }
}
