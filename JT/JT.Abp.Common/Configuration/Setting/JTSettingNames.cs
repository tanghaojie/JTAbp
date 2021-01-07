namespace JT.Abp.Configuration.Setting
{
    public static class JTSettingNames
    {
        public static class UserManagement
        {
            public const string IsEmailConfirmationRequiredForLogin = "JT.UserManagement.IsEmailConfirmationRequiredForLogin";

            public static class UserLockOut
            {
                public const string IsEnabled = "JT.UserManagement.UserLockOut.IsEnabled";

                public const string MaxFailedAccessAttemptsBeforeLockout = "JT.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout";

                public const string DefaultAccountLockoutSeconds = "JT.UserManagement.UserLockOut.DefaultAccountLockoutSeconds";
            }

            public static class TwoFactorLogin
            {
                public const string IsEnabled = "JT.UserManagement.TwoFactorLogin.IsEnabled";

                public const string IsEmailProviderEnabled = "JT.UserManagement.TwoFactorLogin.IsEmailProviderEnabled";

                public const string IsSmsProviderEnabled = "JT.UserManagement.TwoFactorLogin.IsSmsProviderEnabled";

                public const string IsRememberBrowserEnabled = "JT.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled";
            }

            public static class PasswordComplexity
            {
                public const string RequiredLength = "JT.UserManagement.PasswordComplexity.RequiredLength";

                public const string RequireNonAlphanumeric = "JT.UserManagement.PasswordComplexity.RequireNonAlphanumeric";

                public const string RequireLowercase = "JT.UserManagement.PasswordComplexity.RequireLowercase";

                public const string RequireUppercase = "JT.UserManagement.PasswordComplexity.RequireUppercase";

                public const string RequireDigit = "JT.UserManagement.PasswordComplexity.RequireDigit";
            }
        }

        public static class OrganizationUnits
        {
            public const string MaxUserMembershipCount = "JT.OrganizationUnits.MaxUserMembershipCount";
        }
    }
}
