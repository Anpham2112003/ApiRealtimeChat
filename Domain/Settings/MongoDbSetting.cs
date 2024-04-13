using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings
{
    public class MongoDbSetting
    {
        public const string Setting = "MongoDB";
        public string? Conection { get; set; }
        public string? Database { get; set; }
    }
}
