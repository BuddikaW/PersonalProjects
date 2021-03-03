using IMSWebPortal.Areas.Security;
using IMSWebPortal.Data.Models.Identity;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMSWebPortal.Pages.Dtos.DtoMapping
{
    public class UserDtoMap
    {
        private readonly IDataProtectionProvider _provider;
        public UserDtoMap(IDataProtectionProvider provider)
        {
            _provider = provider;
        }

        public String Encript(string input)
        {
            return new DataEncription(_provider).Encript(input);
        }

        public String Decript(string input)
        {
            return new DataEncription(_provider).Decript(input);
        }

        public UserDetailModel Map(AppUser appUser)
        {
            try
            {
                var userDetailModel = new UserDetailModel();
                userDetailModel.Id = appUser.Id;
                userDetailModel.FirstName = Decript(appUser.FirstName);
                userDetailModel.LastName = Decript(appUser.LastName);
                userDetailModel.UserName = appUser.UserName;
                userDetailModel.IsEnabled = appUser.IsEnabled;
                return userDetailModel;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<UserDetailModel> Map(List<AppUser> appUsers)
        {
            var userDetailList = new List<UserDetailModel>();
            appUsers.ToList().ForEach(e => userDetailList.Add(Map(e)));
            return userDetailList;
        }

        public AppUser Map(UserDetailModel userDetail)
        {
            try
            {
                var appUser = new AppUser();
                appUser.Id = userDetail.Id;
                appUser.FirstName = Encript(userDetail.FirstName);
                appUser.LastName = Encript(userDetail.LastName);
                appUser.UserName = userDetail.UserName;
                appUser.IsEnabled = userDetail.IsEnabled;
                return appUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AppUser> Map(List<UserDetailModel> userDetailList)
        {
            var appUsers = new List<AppUser>();
            userDetailList.ToList().ForEach(e => appUsers.Add(Map(e)));
            return appUsers;
        }
    }
}
