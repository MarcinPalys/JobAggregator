using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAggregator.Application.Options
{
    public class AdzunaOptions
    {
        public const string SectionName = "Adzuna";
        public string AppId { get; set; } = string.Empty;
        public string AppKey { get; set; } = string.Empty;
        public string Country { get; set; } = "pl";
    }
}
