using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moetech.Zhuangzhou.Data;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 预设数据
    /// </summary>
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new VirtualMachineDB(serviceProvider.GetRequiredService<DbContextOptions<VirtualMachineDB>>()))
            {
                // 员工数据
                if (context.CommonPersonnelInfo.Any())
                {
                    return;
                }

                context.CommonPersonnelInfo.Add(
                    new CommonPersonnelInfo
                    {
                        PersonnelNo = 10000,
                        PersonnelName = "管理员",
                        DepId = 1,
                        Avatar = null,
                        PersonnelSex = 0,
                        BirthDate = DateTime.Now,
                        IdentityCard = null,
                        IsWork = 1,
                        Nation = "汉族",
                        MaritalStatus = 0,
                        LiveAddress = "四川省成都市高新区",
                        Phone = null,
                        WeChatAccount = null,
                        Mailbox = null,
                        Degree = 0,
                        Address = "四川省成都市高新区",
                        OnboardingTime = DateTime.Now,
                        DepartureTime = DateTime.Parse("1970-01-01"),
                        TrialTime = DateTime.Now.AddDays(90),
                        IsStruggle = 0,
                        IsSecrecy = 0,
                        UserName = "admin",
                        Password = "123456",
                        AppMaxCount = 3
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
