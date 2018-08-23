using System;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace PassportValidationService
{
    public class UserNamePassValidator :UserNamePasswordValidator
    {
        //Authentication
        public override void Validate(string userName, string password)
        {
            if (userName == null || password == null)
            {
                throw new ArgumentNullException("Username or password are empty");
            }

            if (!(userName == ConfigurationManager.AppSettings["Username"] && password == ConfigurationManager.AppSettings["Password"]))
            {
                 throw new SecurityTokenException("Unknown Username or Password");
            }
        }
    }
}