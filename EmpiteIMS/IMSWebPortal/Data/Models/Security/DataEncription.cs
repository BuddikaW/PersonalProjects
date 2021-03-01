using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMSWebPortal.Data.Models.Security
{
    public class DataEncription
    {
        private readonly IDataProtector _protector;

        public DataEncription(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("empite");
        }

        public String Encript(string input)
        {
            return _protector.Protect(input);
        }

        public String Decript(string input)
        {
            return _protector.Unprotect(input);
        }
    }
}
