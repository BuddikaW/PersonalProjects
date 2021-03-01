using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace IMSWebPortal.Data
{
    public class SeedHelper
    {
        private static IDataProtector _protector;

        public static String Encript(string input)
        {
            return _protector.Protect(input);
        }

        public static async Task Initialize(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("empite");

            context.Database.EnsureCreated();

            if((await CreateRoles(roleManager)) && (await CreateSuperAdmin(userManager))){
                var userAddedStatus = await CreateTestUsers(userManager);
                var itemAddedStatus = await CreateTestItems();
            }
        }

        public static async Task<bool> CreateRoles(RoleManager<AppRole> roleManager)
        {
            try
            {
                var role1 = "Admin";
                var role2 = "Manager";
                var role3 = "Viewer";

                if (await roleManager.FindByNameAsync(role1) == null)
                {
                    await roleManager.CreateAsync(new AppRole() { Name = role1 });
                }
                if (await roleManager.FindByNameAsync(role2) == null)
                {
                    await roleManager.CreateAsync(new AppRole() { Name = role2 });
                }
                if (await roleManager.FindByNameAsync(role3) == null)
                {
                    await roleManager.CreateAsync(new AppRole() { Name = role3 });
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> CreateUser(UserManager<AppUser> userManager, string email, string password, string firstName, string lastName, string role)
        {
            firstName = Encript(firstName);
            lastName = Encript(lastName);
            if (await userManager.FindByNameAsync(email) == null)
            {
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    IsEnabled = true
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            return true;
        }

        public static async Task<bool> CreateSuperAdmin(UserManager<AppUser> userManager)
        {
            try
            {
                var password = "Admin@123";
                var email = "admin@empite.com";
                var firstName = "Phil";
                var lastName = "Coulson";
                var role = "Admin";
                return await CreateUser(userManager, email, password, firstName, lastName, role);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> CreateTestUsers(UserManager<AppUser> userManager)
        {
            try
            {
                #region Create test manager
                var managerPassword = "Manager@123";
                var managerEmail = "manager@empite.com";
                var managerFirstName = "Chloe";
                var managerLastName = "Bennet";
                var managerRole = "Manager";
                var manageAdded = await CreateUser(userManager, managerEmail, managerPassword, managerFirstName, managerLastName, managerRole);
                #endregion

                #region Create test viewer
                var viewerPassword = "Viewer@123";
                var viewerEmail = "viewer@empite.com";
                var viewerFirstName = "Leo";
                var viewerLastName = "Fitz";
                var viewerRole = "Viewer";
                var viewerAdded = await CreateUser(userManager, viewerEmail, viewerPassword, viewerFirstName, viewerLastName, viewerRole);
                #endregion

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> CreateTestItems()
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
