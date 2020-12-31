using Abp.AspNetCore.Mvc.Controllers;

namespace JT.Web.Controllers
{
    public abstract class JTControllerBase: AbpController
    {
        protected JTControllerBase()
        {
            LocalizationSourceName = JTConsts.LocalizationSourceName;
        }
    }
}