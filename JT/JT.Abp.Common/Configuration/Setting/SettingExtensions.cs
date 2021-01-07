using Abp.Configuration;

namespace JT.Abp.Configuration
{
    internal static class SettingExtensions
    {
        public static JTSetting ToSetting(this SettingInfo settingInfo)
        {
            return settingInfo == null
                ? null
                : new JTSetting(settingInfo.TenantId, settingInfo.UserId, settingInfo.Name, settingInfo.Value);
        }

        public static SettingInfo ToSettingInfo(this JTSetting setting)
        {
            return setting == null
                ? null
                : new SettingInfo(setting.TenantId, setting.UserId, setting.Name, setting.Value);
        }
    }
}
