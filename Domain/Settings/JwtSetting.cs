using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings
{
    public class JwtSetting
    {
        public const string Jwtseting = "Jwt";

        public string? AcccessKey  { get; set; }

        public string? ReFreshKey {  get; set; }

       
    }
}
