using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using QRQing.LineManager.Authentication;
using QRQing.LineManager.Components;
using QRQing.LineManager.Data;
using QRQing.LineManager.Data.Models;
using QRQing.LineManager.Shared;
using QRQing.LineManager.Utilities;
using System.Reflection.Metadata.Ecma335;

namespace QRQing.LineManager.Pages
{
    public partial class AdvisorLogin : Login
    {
        private string? inputCode;
        private bool enterEmailStep = true;
        private bool codeIncorrect = false;
        private bool emailHasBeenModified = false;
        private bool modifiedEmail = false;
        private bool notInSystemWarning = false;
        private bool timeElapsed = false;
        private int minCodeWindow = 2; // 2 minutes


        private string nextURL = $"/students/list/1";
        public string? FormTitle => "Faculty Login";

        protected override UserType PageUserType => UserType.Advisor;





        protected async Task EnterEmail()
        {

            using var context = ContextFactory.CreateDbContext();
            UserAccount? userAccount = await GetUserAccount(context);

            if (userAccount == null)
            {
                notInSystemWarning = true;
                return;
            }

            await HelperMethods.SendConfirmationCode(userAccount, emailSender);
            context.SaveChanges();

            enterEmailStep = false;
        }

        protected async Task SubmitCode()
        {
            using var context = ContextFactory.CreateDbContext();
            UserAccount? userAccount = await GetUserAccount(context);

            if (inputCode != userAccount.ConfirmationCode)
            {
                codeIncorrect = true;
                return;
            }
            if (DateTime.Now > userAccount.CodeSent + new TimeSpan(0, minCodeWindow, 0))
            {
                timeElapsed = true;
                return;
            }
            if (HelperMethods.InputCodeIsValid(inputCode, userAccount))
            {
                var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
                await customAuthStateProvider.UpdateAuthenticationState(
                    new UserSession { Email = userAccount.Email, Role = userAccount.Role });

                navManager.NavigateTo(nextURL);
            }
        }


    }
}