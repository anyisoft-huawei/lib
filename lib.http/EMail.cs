using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Web;
using System.Net.Mail;

namespace lib.http
{
    /// <summary>
    /// Email 帮助类
    /// </summary>
    public class EMail : IDisposable
    {
        #region 发送电子邮件
        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="host">SMTP服务器</param>
        /// <param name="ssl">是否启用SSL加密</param>
        /// <param name="user">登录帐号</param>
        /// <param name="pswd">登录密码</param>
        /// <param name="nick">发件人昵称</param>
        /// <param name="from">发件人</param>
        /// <param name="to">收件人</param>
        /// <param name="title">主题</param>
        /// <param name="body">内容</param>
        public static void Send(string host, string user, string pswd, string nick, string from, string to, string title, string body, bool ssl = false)
        {
            SmtpClient _smtp = new SmtpClient();
            _smtp.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            _smtp.Host = host;//指定SMTP服务器
            _smtp.Credentials = new System.Net.NetworkCredential(user, pswd);//用户名和密码
            _smtp.EnableSsl = ssl;
            MailAddress _from = new MailAddress(from, nick);
            MailAddress _to = new MailAddress(to);
            MailMessage _mailMessage = new MailMessage(_from, _to);
            _mailMessage.Subject = title;//主题
            _mailMessage.Body = body;//内容
            _mailMessage.BodyEncoding = System.Text.Encoding.Default;//正文编码
            _mailMessage.IsBodyHtml = true;//设置为HTML格式
            _mailMessage.Priority = MailPriority.Normal;//优先级
            _smtp.Send(_mailMessage);
        }

        #endregion

        SmtpClient _client = null;
        MailAddress _address = null;

        /// <summary>
        /// 创建邮件客户端
        /// </summary>
        /// <param name="host">服务器</param>
        /// <param name="user">用户名</param>
        /// <param name="pswd">密码</param>
        /// <param name="nick">昵称</param>
        /// <param name="from">地址</param>
        /// <param name="ssl">开启ssl</param>
        public EMail(string host, string user, string pswd, string nick, string from, bool ssl = false)
        {
            _client = new SmtpClient();
            _client.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            _client.Host = host;//指定SMTP服务器
            _client.Credentials = new System.Net.NetworkCredential(user, pswd);//用户名和密码
            _client.EnableSsl = ssl;
            _address = new MailAddress(from, nick);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="title">标题</param>
        /// <param name="body">html正文</param>
        public void Send(string to, string title, string body)
        {
            MailAddress _to = new MailAddress(to);
            MailMessage _mailMessage = new MailMessage(_address, _to);
            _mailMessage.Subject = title;//主题
            _mailMessage.Body = body;//内容
            _mailMessage.BodyEncoding = System.Text.Encoding.Default;//正文编码
            _mailMessage.IsBodyHtml = true;//设置为HTML格式
            _mailMessage.Priority = MailPriority.Normal;//优先级
            _client.Send(_mailMessage);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _address = null;
        }

    }
}
