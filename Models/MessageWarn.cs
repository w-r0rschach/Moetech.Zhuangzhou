using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    public class MessageWarn
    {
        /// <summary>
        /// 消息主键
        /// </summary>
        [Key]
        
        public int MessageId { set; get; }

        /// <summary>
        /// 消息主题
        /// </summary>
        [StringLength(100)]
        [Display(Name = "消息主题")]
        public string MessageTitle { set; get; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [StringLength(255)]
        [Display(Name = "消息内容")]
        public string MessageContent { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "用户ID")]
        public int PonsonalId { set; get; }

        /// <summary>
        /// 提醒时间
        /// </summary>
        [Display(Name = "提醒时间")]
        public DateTime MessageWarnDate { set; get; }

        /// <summary>
        /// (消息类型)是否已读 0：未读  1：已读
        /// </summary>
        [Display(Name = "消息类型")]
        public int MessageType { set; get; }

        /// <summary>
        /// 已读时间
        /// </summary>
        [Display(Name = "已读时间")]
        public DateTime MessageReadDate { set; get; }
    }
}
