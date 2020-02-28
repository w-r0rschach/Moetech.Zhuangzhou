using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class CommonPersonnelInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [Display(Name = "编号")]
        public int PersonnelId { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Display(Name = "工号")]
        [Required(ErrorMessage = "工号字段是必需的。")]
        public int PersonnelNo { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        [Display(Name = "员工名称")]
        [Required(ErrorMessage = "员工名称字段是必需的。"), StringLength(50)]
        public string PersonnelName { get; set; }

        /// <summary>
        /// 所属部门
        /// 0：普通部门
        /// 1：管理部门
        /// </summary>
        [Display(Name = "角色")]
        public int DepId { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        [StringLength(200)]
        public string Avatar { get; set; }

        /// <summary>
        /// 性别
        /// 0：男
        /// 1：女
        /// </summary>
        [Display(Name = " 性别")]
        public int PersonnelSex { get; set; }

        /// <summary>
        /// 出身日期
        /// </summary>
        [Display(Name = "出身日期")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        [StringLength(18)]
        public string IdentityCard { get; set; }

        /// <summary>
        /// 是否在职
        /// 0：离职
        /// 1：在职
        /// 2：实习
        /// </summary>
        [Display(Name = "是否在职")]
        public int IsWork { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族")]
        [StringLength(6)]
        public string Nation { get; set; }

        /// <summary>
        /// 婚姻状况
        /// 0：未婚
        /// 1：已婚
        /// </summary>
        [Display(Name = "婚姻状况")]
        public int MaritalStatus { get; set; }

        /// <summary>
        /// 现住地址
        /// </summary>
        [Display(Name = "现住地址")]
        [StringLength(200)]
        public string LiveAddress { get; set; }

        /// <summary>
        /// 手机号码
        /// 多个使用逗号分隔
        /// </summary>
        [Display(Name = "手机号码")]
        [StringLength(500)]
        public string Phone { get; set; }

        /// <summary>
        /// 微信账号
        /// </summary>
        [Display(Name = "微信")]
        [StringLength(100)]
        public string WeChatAccount { get; set; }

        /// <summary>
        /// 邮箱
        /// 多个使用逗号分隔
        /// </summary>
        [Display(Name = "邮箱")]
        [StringLength(500)]
        public string Mailbox { get; set; }

        /// <summary>
        /// 学历
        /// 0：未知
        /// 1：小学
        /// 2：初中
        /// 3：高中
        /// 4：大专
        /// 5：本科
        /// 6：研究生
        /// 7：博士
        /// </summary>
        [Display(Name = "学历")]
        public int Degree { get; set; }

        /// <summary>
        /// 户籍地址
        /// </summary>
        [Display(Name = "户籍地址")]
        [StringLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        [Display(Name = "入职时间")]
        [DataType(DataType.Date)]
        public DateTime? OnboardingTime { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        [Display(Name = "离职时间")]
        [DataType(DataType.Date)]
        public DateTime? DepartureTime { get; set; }

        /// <summary>
        /// 试用期或实习期结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        [DataType(DataType.Date)]
        public DateTime? TrialTime { get; set; }

        /// <summary>
        /// 是否奋斗者
        /// 0：非奋斗者
        /// 1：奋斗者
        /// </summary>
        [Display(Name = "是否奋斗者")]
        public int IsStruggle { get; set; }

        /// <summary>
        /// 是否保密人员
        /// 0：非保密人员
        /// 1：保密人员
        /// </summary>
        [Display(Name = "是否保密人员")]
        public int IsSecrecy { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Display(Name = "登录账号")]
        [Required(ErrorMessage = "登录账号字段是必需的。"), StringLength(255)]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Display(Name = "登录密码")]
        [Required(ErrorMessage = "登录密码字段是必需的。"), StringLength(255)]
        public string Password { get; set; }

        /// <summary>
        /// 最大数量
        /// 虚拟机自动审批功能，超过指定数量需要管理员审批
        /// </summary>
        [Display(Name = "自动审批数量")]
        public int AppMaxCount { get; set; }
    }
}
