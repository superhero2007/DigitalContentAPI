using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.UI;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.TermsOfService;
using Digital_Content.DigitalContent.TermsOfService.Dto;
using Digital_Content.DigitalContent.TermsOfServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.TermsOfServices
{
    public class TermsOfServicesAppService : DigitalContentAppServiceBase, ITermsOfServicesAppService
    {
        private readonly IRepository<TermsOfService, long> _termsOfServiceRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;


        public TermsOfServicesAppService(IRepository<TermsOfService, long> termsOfServiceRepository,
                                         IRepository<User, long> userRepository,
                                         IRepository<UserRole, long> userRoleRepository,
                                         IRepository<Role> roleRepository)
        {
            _termsOfServiceRepository = termsOfServiceRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> CreateOrUpdateTosContentAsync(TermsOfServiceInput input)
        {
            try
            {
                var roleId = _userRoleRepository.FirstOrDefault(item => item.UserId == AbpSession.UserId).RoleId;
                var role = _roleRepository.FirstOrDefault(item => item.Id == roleId);
                if (AbpSession.TenantId == null && role.DisplayName.Equals("Admin"))
                {
                    var existingTerm = await _termsOfServiceRepository.FirstOrDefaultAsync(term => term.TenantId == null);
                    if (existingTerm != null)
                    {
                        existingTerm.TosContent = input.TosContent;
                        existingTerm.Version++;
                    }
                    else
                    {
                        existingTerm = SetTermObject(input.TosContent, role);
                        existingTerm.Version++;
                    }
                    await _termsOfServiceRepository.InsertOrUpdateAsync(existingTerm);
                    return true;
                }
                //To do: uncomment when tenant implemented
                else if (role.DisplayName.Equals("TenantAdmin"))//(AbpSession.TenantId != null)
                {
                    var currentTenantTos = await _termsOfServiceRepository.FirstOrDefaultAsync(item => item.RevicedBy.Id == AbpSession.UserId);
                    if (currentTenantTos != null)
                    {
                        currentTenantTos.TosContent = input.TosContent;
                        await _termsOfServiceRepository.UpdateAsync(currentTenantTos);
                    }
                    return true;
                }
                else
                {
                    throw new UserFriendlyException("Admin role needed");
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task<TermsOfServiceInput> GetAndAssignDefaultTosAsync()
        {
            try
            {
                var roleId = _userRoleRepository.FirstOrDefault(item => item.UserId == AbpSession.UserId).RoleId;
                var role = _roleRepository.FirstOrDefault(item => item.Id == roleId);
                var defaultToes = await _termsOfServiceRepository.FirstOrDefaultAsync(item => item.IsDefault == true)?? new TermsOfService();
                if (role.DisplayName.Equals("Admin"))
                {
                    return new TermsOfServiceInput()
                    {
                        TosContent = defaultToes.TosContent
                    };
                }
                else if(role.DisplayName.Equals("TenantAdmin"))
                {
                    var tenantAdminTos = _termsOfServiceRepository.FirstOrDefault(item => item.RevicedBy.Id == AbpSession.UserId);
                    if (tenantAdminTos == null)
                    {
                        var tosId = await _termsOfServiceRepository.InsertAndGetIdAsync(SetTermObject(defaultToes.TosContent, role));

                        return new TermsOfServiceInput()
                        {
                            TosContent = _termsOfServiceRepository.Get(tosId).TosContent
                        };
                    }
                    else
                    {
                        return new TermsOfServiceInput()
                        {
                            TosContent = tenantAdminTos.TosContent
                        };
                    }
                }
                return new TermsOfServiceInput(); 
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        private TermsOfService SetTermObject(string tosContent, Role role)
        {
            try
            {
                var term = new TermsOfService();
                var user = _userRepository.Get((long)AbpSession.UserId);
                term.RevicedBy = user;
                term.DateRevised = DateTime.Now;
                term.TosContent = tosContent;
                term.TenantId = AbpSession.TenantId;
                term.IsDefault = role.DisplayName.Equals("Admin") ? true : false;
                return term;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<TermsOfServiceOutput> ResetDefaultTosContent()
        {
            try
            {
                var roleId = _userRoleRepository.FirstOrDefault(item => item.UserId == AbpSession.UserId).RoleId;
                var role = _roleRepository.FirstOrDefault(item => item.Id == roleId);
                var defaultTos = await _termsOfServiceRepository.FirstOrDefaultAsync(item => item.IsDefault == true) ?? new TermsOfService();
                if (role.DisplayName.Equals("TenantAdmin"))
                {
                    var currentTos = await _termsOfServiceRepository.FirstOrDefaultAsync(item => item.RevicedBy.Id == AbpSession.UserId) ?? new TermsOfService();
                    currentTos.TosContent = defaultTos.TosContent;
                    await _termsOfServiceRepository.UpdateAsync(currentTos);
                    return new TermsOfServiceOutput()
                    {
                        TosContent = currentTos.TosContent
                    };
                }
                else
                {
                    return new TermsOfServiceOutput()
                    {
                        TosContent = defaultTos.TosContent
                    };
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
          
        }

       
    }
}
