using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using Moetech.Zhuangzhou.Controllers;
using Moetech.Zhuangzhou.Data;

namespace Moetech.Zhuangzhou
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // �˷���������ʱ���á�ʹ�ô˷�����������ӷ���
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // ע�����ݿ�������
            //services.AddDbContext<VirtualMachineDB>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServerDefault")));
            services.AddDbContext<VirtualMachineDB>(options => options.UseMySql(Configuration.GetConnectionString("MySqlDefault")));
            services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
            // ���Session
            services.AddSession();
            services.Configure<CookiePolicyOptions>(options =>
           {
               //���lambda��������һ�������������Ƿ���Ҫ�û��ԷǱ���cookie��ͬ�⡣
               options.CheckConsentNeeded = context => false;//Ĭ��Ϊtrue����Ϊfalse
               options.MinimumSameSitePolicy = SameSiteMode.None;
           });

            //���������Ѿ�������FilterControllerע�뵽ȫ��������
            //services.AddMvc(options => {
            //    options.Filters.Add<FilterController>();
            //});
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

            app.UseAuthorization();

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
