using Abp.Collections;

namespace JT.Abp.Configuration
{
    public interface IUserManagementConfig
    {
        ITypeList<object> ExternalAuthenticationSources { get; set; }
    }
}