using IMSWebPortal.Data.Models.Email;
using IMSWebPortal.Data.Models.Identity;
using IMSWebPortal.Data.Models.Inventory;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMSWebPortal.Data
{
    public class SeedHelper
    {
        private static IDataProtector _protector;

        public static async Task Initialize(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("empite");

            context.Database.EnsureCreated();

            if((await CreateRoles(roleManager)) && (await CreateSuperAdmin(userManager))){
                var emailAddedStatus = SetEmailDetails(context);
                var userAddedStatus = await CreateTestUsers(userManager);
                var itemAddedStatus = CreateTestItems(context);
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

        public static bool SetEmailDetails(ApplicationDbContext context)
        {
            try
            {
                if (!context.EmailDetails.Any())
                {
                    var email = new EmailDetail();
                    email.Port = 587;
                    email.Host = "smtp.gmail.com";
                    email.UserName = "project.bkw@gmail.com";
                    email.Password = "projectBkw!";
                    context.EmailDetails.Add(email);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
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

        public static bool CreateTestItems(ApplicationDbContext context)
        {
            try
            {
                if (!context.ItemDetails.Any())
                {
                    var itemList = new List<ItemDetail>();

                    var item1 = new ItemDetail();
                    item1.Name = "Triangle Ruler";
                    item1.Sku = "EMP001";
                    item1.Price = Convert.ToDecimal("23.50");
                    item1.Qty = 200;
                    item1.IsDeleted = false;
                    itemList.Add(item1);

                    var item2 = new ItemDetail();
                    item2.Name = "HB Pencil";
                    item2.Sku = "EMP002";
                    item2.Price = Convert.ToDecimal("17.00");
                    item2.Qty = 300;
                    item2.IsDeleted = false;
                    itemList.Add(item2);

                    var item3 = new ItemDetail();
                    item3.Name = "Drawing Board";
                    item3.Sku = "EMP003";
                    item3.Price = Convert.ToDecimal("50.00");
                    item3.Qty = 800;
                    item3.IsDeleted = false;
                    itemList.Add(item3);

                    var item4 = new ItemDetail();
                    item4.Name = "Fabric Paint";
                    item4.Sku = "EMP004";
                    item4.Price = Convert.ToDecimal("130.00");
                    item4.Qty = 70;
                    item4.IsDeleted = false;
                    itemList.Add(item4);

                    var item5 = new ItemDetail();
                    item5.Name = "Mapped Eraser";
                    item5.Sku = "EMP005";
                    item5.Price = Convert.ToDecimal("90.50");
                    item5.Qty = 120;
                    item5.IsDeleted = false;
                    itemList.Add(item5);

                    context.ItemDetails.AddRange(itemList);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static String Encript(string input)
        {
            return _protector.Protect(input);
        }
    }
}
