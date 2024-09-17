using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using QRQing.LineManager.Authentication;
using QRQing.LineManager.Components;
using QRQing.LineManager.Data.Models;
using QRQing.LineManager.Migrations;
using QRQing.LineManager.Utilities;
using System;

namespace QRQing.LineManager.Pages
{
    public partial class StudentJoin
    {
        [Parameter]
        public int AdvisorId { get; set; }

        private Advisor? Advisor { get; set; }
        private Student? Student {  get; set; }

        private Student? PreExistingStudent { get; set; }


        private string? FirstName { get; set; }
        private string? LastName { get; set; }
        private string? Email { get; set; }




        private string? inputCode;
        private bool enterEmailStep = true;
        private bool codeIncorrect = false;
        private bool timeElapsed = false;
        private bool modifiedEmail = false;
        private bool modifiedFirstName = false;
        private bool modifiedLastName = false;

        private readonly string nextURL = $"/studentview";
        private const int minCodeWindow = 2;
        private HubConnection? hubConnection;

        protected override async Task OnInitializedAsync()
        {
            using var dbcontext = await ContextFactory.CreateDbContextAsync();
            Advisor = await dbcontext.Advisors.FindAsync(AdvisorId);
            hubConnection = new HubConnectionBuilder().WithUrl(navManager.ToAbsoluteUri("/msghub")).Build();
            await hubConnection.StartAsync();

        }

        private async Task NotifyUpdate()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendUpdate", "A change has been made in student overview");
            }
        }

        protected async Task SubmitInfo()
        {
            if (!ParametersAreValid())
            {
                (modifiedEmail, modifiedFirstName, modifiedLastName) = (true, true, true);
                return;
            }
            using var context = await ContextFactory.CreateDbContextAsync();
            
            PreExistingStudent = await context.Students
                                 .Where(stu => stu.Email == Email)
                                 .Include(stu => stu.Advisor)
                                 .FirstOrDefaultAsync();
            if (PreExistingStudent is not null)
            {
                return;
            }



            Student = new Student()
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                AdvisorId = AdvisorId,
                StudentAccount = new StudentAccount()
                {
                    Email = Email,
                },
            };

            await HelperMethods.SendConfirmationCode(Student.StudentAccount, emailSender);

            enterEmailStep = false;
        }
        protected void GoToLogin()
        {
            navManager.NavigateTo("/studentlogin", true);
        }



        protected async Task SubmitCode()
        {
            if (string.IsNullOrWhiteSpace(inputCode))
            {
                return;
            }

            if (Student == null)
            {
                enterEmailStep = true;
                return;
            }

            var userAccount = Student.StudentAccount;

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

                var context = ContextFactory.CreateDbContext();

                var studentCount = await context.Students
                                    .Where(stu => stu.AdvisorId == AdvisorId)
                                    .Where(stu => stu.InLine)
                                    .CountAsync();

                Student.QueuePosition = studentCount + 1;
                Student.TimeJoined = DateTime.Now;
                Student.InLine = true;
                context.Add(Student);
                HelperMethods.UpdatePosition(AdvisorId, context);
                await NotifyUpdate();


                navManager.NavigateTo(nextURL);
            }
        }



        private bool ParametersAreValid()
        {
            return !(string.IsNullOrWhiteSpace(FirstName)
                        || string.IsNullOrWhiteSpace(LastName)
                        || string.IsNullOrWhiteSpace(Email));
        }
    }
}
