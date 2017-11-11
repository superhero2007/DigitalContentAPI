#if FEATURE_LDAP
using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.MultiTenancy;

namespace Digital_Content.DigitalContent.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}
#endif