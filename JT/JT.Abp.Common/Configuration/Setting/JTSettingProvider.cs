using Abp.Configuration;
using Abp.Localization;
using System.Collections.Generic;

namespace JT.Abp.Configuration.Setting
{
    public class JTSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new List<SettingDefinition>
                   {
                       new SettingDefinition(
                           JTSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                           "false",
                           new FixedLocalizableString("Is email confirmation required for login."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.OrganizationUnits.MaxUserMembershipCount,
                           int.MaxValue.ToString(),
                           new FixedLocalizableString("Maximum allowed organization unit membership count for a user."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.TwoFactorLogin.IsEnabled,
                           "true",
                           new FixedLocalizableString("Is two factor login enabled."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled,
                           "true",
                           new FixedLocalizableString("Is browser remembering enabled for two factor login."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.TwoFactorLogin.IsEmailProviderEnabled,
                           "true",
                           new FixedLocalizableString("Is email provider enabled for two factor login."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.TwoFactorLogin.IsSmsProviderEnabled,
                           "true",
                           new FixedLocalizableString("Is sms provider enabled for two factor login."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.UserLockOut.IsEnabled,
                           "true",
                           new FixedLocalizableString("Is user lockout enabled."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                           "5",
                           new FixedLocalizableString("Maxumum Failed access attempt count before user lockout."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                           "300", //5 minutes
                           new FixedLocalizableString("User lockout in seconds."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                           "false",
                           new FixedLocalizableString("Require digit."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                           "false",
                           new FixedLocalizableString("Require lowercase."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                           "false",
                           new FixedLocalizableString("Require non alphanumeric."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                           "false",
                           new FixedLocalizableString("Require upper case."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           ),

                       new SettingDefinition(
                           JTSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                           "3",
                           new FixedLocalizableString("Required length."),
                           scopes: SettingScopes.Application | SettingScopes.Tenant,
                           clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                           )
                   };
        }
    }
}
