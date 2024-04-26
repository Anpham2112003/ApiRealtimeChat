using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings
{
    public class MailSetting
    {
        public const string Setting = "Mailer";
        public string? DispayName {  get; set; }
        public string? Email { get; set; } 
        public string? Password { get; set; }
        public string? Host { get; set; }
        public string? Port { get; set; }
    }
}
