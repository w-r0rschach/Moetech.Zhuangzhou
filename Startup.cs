using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

        // �˷���������ʱ���á�ʹ�ô˷�����������ӷ���
        public void ConfigureServices(IServiceCollection services)
        {
            // ��������
            EmailConfig emailConfig = new EmailConfig();
            Configuration.GetSection("EmailConfig").Bind(emailConfig);
            EmailHelper email = new EmailHelper(emailConfig);

            // vSphere����
            VSphereConfig sphereConfig = new VSphereConfig();
            Configuration.GetSection("VSphereConfig").Bind(sphereConfig);

            services.AddControllersWithViews();
            // ע�����ݿ�������
            services.AddDbContext<VirtualMachineDB>(options => options.UseMySql(Configuration.GetConnectionString("TestDefault")));
            //services.AddDbContext<VirtualMachineDB>(options => options.UseMySql(Configuration.GetConnectionString("MySqlDefault")));
            // HTML����
            services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
            // ���Session
            services.AddSession();
            // ע��ӿ�
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IVmware, VmwareService>();
			services.AddScoped<IVmwareManage, VmwareManageService>();
            services.AddScoped<ILogs, LoggerService>();
            services.Configure<CookiePolicyOptions>(options =>
            {
               //���lambda��������һ�������������Ƿ���Ҫ�û��ԷǱ���cookie��ͬ�⡣
               options.CheckConsentNeeded = context => false;//Ĭ��Ϊtrue����Ϊfalse
               options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // ע�붨ʱ����
            services.AddHostedService<StartTimedTask>();
            services.AddScoped<ITimedTask, TimedTaskService>();
        }

        // �˷���������ʱ���á�ʹ�ô˷�������HTTP����ܵ���
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error.cshtml");
                // Ĭ�ϵ�HSTSֵ��30�졣������ϣ����������������Ĵ����ã���μ�https://aka.ms/aspnetcore-hsts��
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization(); // �����֤
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Vmware}/{action=Index}/{id?}");
            });
        }
    }
}
