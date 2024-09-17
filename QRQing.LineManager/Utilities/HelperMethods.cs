using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QRQing.LineManager.Authentication;
using QRQing.LineManager.Data;
using QRQing.LineManager.Data.Models;

namespace QRQing.LineManager.Utilities
{
    public static class HelperMethods
    {
        public static string GenerateConfirmationCode()
        {
            // does not include I and l, O, o, or 0 because they look similar
            var confirmationChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";
            var random = new Random();
            var codeLength = 4;
            return new string(Enumerable.Repeat(confirmationChars, codeLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool InputCodeIsValid(string? inputCode, UserAccount? userAccount)
        {
            if (userAccount == null || inputCode == null)
            {
                return false;
            }
            return userAccount?.ConfirmationCode == inputCode // code is correct
                && DateTime.Now < userAccount.CodeSent + new TimeSpan(0, 2, 0); // within 2 minutes
            
        }

        public static async void UpdatePosition(int? advisorId, LineManagerDbContext context)
        {
            var Students = await context.Students
                .Where(stu => stu.AdvisorId == advisorId && stu.InLine)
                .OrderBy(stu => stu.QueuePosition)
                .ToListAsync();
            int fixedQueuePos = 1;
            foreach (var stu in Students)
            {
                stu.QueuePosition = fixedQueuePos;
                fixedQueuePos++;
            }
            context.SaveChanges();
        }

        public static async Task SendConfirmationCode(UserAccount userAccount, IEmailSender emailSender)
        {
            var confirmationCode = HelperMethods.GenerateConfirmationCode();
            userAccount.ConfirmationCode = confirmationCode;
            userAccount.CodeSent = DateTime.Now;

            if (userAccount.Email == null)
            {
                throw new ArgumentNullException();
            }
            await emailSender.SendEmailAsync(userAccount.Email,
                "[" + confirmationCode + "] ECE Queue System Confirmation Code",
                "Your confirmation code is " + confirmationCode + ".");
        }


    }
}
