using Abp.Authorization;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;

namespace Digital_Content.DigitalContent.Authorization
{
    /// <summary>
    /// Implements <see cref="PermissionChecker"/>.
    /// </summary>
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(IUserManager userManager)
            : base(userManager)
        {

        }
    }
}
