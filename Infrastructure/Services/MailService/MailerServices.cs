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

namespace Infrastructure.Services.MailService
{
    public class MailerServices : IMailerServices
    {
        private readonly IOptions<MailSetting> _setting;

        public MailerServices(IOptions<MailSetting> setting)
        {
            _setting = setting;
        }

        public async Task SendMailAsync(MailContent content)
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = content.Content;

            var message = new MimeMessage();

            message.Sender = new MailboxAddress(_setting.Value.DispayName, _setting.Value.Email);
            message.From.Add(new MailboxAddress(_setting.Value.DispayName, _setting.Value.Email));
            message.To.Add(MailboxAddress.Parse(content.To));
            message.Subject = content.Subject;
            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {

                await smtp.ConnectAsync(_setting.Value.Host, int.Parse(_setting.Value.Port!), true);

                await smtp.AuthenticateAsync(_setting.Value.Email, _setting.Value.Password);

                await smtp.SendAsync(message);


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
