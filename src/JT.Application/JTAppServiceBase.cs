using Abp.Application.Services;

namespace JT
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class JTAppServiceBase : ApplicationService
    {
        protected JTAppServiceBase()
        {
            LocalizationSourceName = JTConsts.LocalizationSourceName;
        }
    }
}