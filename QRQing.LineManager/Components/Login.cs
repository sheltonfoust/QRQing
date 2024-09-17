using QRQing.LineManager.Data.Models;
using QRQing.LineManager.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using QRQing.LineManager.Authentication;
using QRQing.LineManager.Utilities;

namespace QRQing.LineManager.Components
{
    public abstract class Login : ComponentBase
    {
        public string? Email { get; set; }

        protected enum UserType
        {
            Student,
            Advisor
        }
        protected abstract UserType PageUserType { get; }

        public async Task<UserAccount?> GetUserAccount(LineManagerDbContext context)
        {
        

        UserAccount? userAccount;


            if (PageUserType == UserType.Advisor)
            {
                userAccount = await context.AdvisorAccounts
                    .Where(adv => adv.Email == Email)
                    .FirstOrDefaultAsync();
            }
            else if (PageUserType == UserType.Student)
            {
                userAccount = await context.StudentAccounts
                    .Where(stu => stu.Email == Email)
                    .FirstOrDefaultAsync();
            }
            else
            {
                throw new Exception("Page does not have a valid user type.");

            }

            return userAccount;
        }

        
    }


}

