using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class Settings : BaseClass
    {
        public string SettingsKey { get; set; }
        public string Value { get; set; }
    }
}
