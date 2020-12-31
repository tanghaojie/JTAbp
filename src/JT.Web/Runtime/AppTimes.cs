using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Web.Runtime
{
    public class AppTimes : ISingletonDependency
    {
        public DateTime StartupTime { get; set; }
    }
}
