using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using QRQing.LineManager;
using QRQing.LineManager.Shared;
using Microsoft.EntityFrameworkCore;
using QRQing.LineManager.Data;
using QRQing.LineManager.Data.Models;
using QRQing.LineManager.Authentication;
using QRQing.LineManager.Components;
using QRQing.LineManager.Utilities;

namespace QRQing.LineManager.Pages
{
    public partial class StudentLogin : Login
    {
        private string? inputCode;
        private bool enterEmailStep = true;
        private bool codeIncorrect = false;
        private bool modifiedEmail = false;
        private bool notInLineWarning = false;
        private bool timeElapsed = false;
        private int minCodeWindow = 2; // 2 minutes

        private string nextURL = $"/studentview";
        public string? FormTitle => "Student Login";

        protected override UserType PageUserType => UserType.Student;



        protected async Task EnterEmail()
        {

            using var context = ContextFactory.CreateDbContext();
            UserAccount? userAccount = await GetUserAccount(context);

            if (userAccount == null)
            {
                notInLineWarning = true;
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