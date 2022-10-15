using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Restaurant.Serivces.Identity.Models;
using System.Security.Claims;

namespace Restaurant.Services.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimPricipalFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProfileService(IUserClaimsPrincipalFactory<ApplicationUser> userClaimPricipalFactory, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userClaimPricipalFactory = userClaimPricipalFactory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string sub = context.Subject.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(sub);
            ClaimsPrincipal userClaims = await _userClaimPricipalFactory.CreateAsync(user);
            List<Claim> claims = userClaims.Claims.ToList();
            claims = claims.Where(claim=>context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            if (_userManager.SupportsUserRole)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                foreach(var rolename in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
                    claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                    if (_roleManager.SupportsRoleClaims)
                    {
                        IdentityRole role= await _roleManager.FindByNameAsync(rolename);
                        if (role != null)
                        {
                            claims.AddRange(await _roleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
