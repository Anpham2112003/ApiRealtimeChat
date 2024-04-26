using Domain.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MailerServices : IMailerServices
    {
        private readonly IOptionsMonitor<MailSetting> _setting;

        public MailerServices(IOptionsMonitor<MailSetting> setting)
        {
            _setting = setting;
        }

        public async Task SendMailAsync( MailContent content)
        {
            var message = new MimeMessage();

            message.Sender = new MailboxAddress(_setting.CurrentValue.DispayName, _setting.CurrentValue.Email);
            message.From.Add(new MailboxAddress(_setting.CurrentValue.DispayName,_setting.CurrentValue.Email));
            message.To.Add( MailboxAddress.Parse(content.To)) ;
            message.Subject = content.Subject;



            var builder = new BodyBuilder();

            builder.HtmlBody = content.Content;

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {

                await  smtp.ConnectAsync(_setting.CurrentValue.Host,int.Parse(_setting.CurrentValue.Port!),true);

                await smtp.AuthenticateAsync(_setting.CurrentValue.Email, _setting.CurrentValue.Password);

                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
        }

       
    }
    public class MailContent
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
    }
}
