using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using QRQing.LineManager.Authentication;
using QRQing.LineManager.Data.Models;
using QRQing.LineManager.Utilities;
using System;

namespace QRQing.LineManager.Pages
{
    partial class StudentOverview
    {
        [Parameter]
        public int? CurrentPage { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        

        private Advisor? Advisor {  get; set; }
        private Student[]? Students { get; set; }

        private int TotalPages { get; set; }

        private const int ItemsPerPage = 20;

        private HubConnection? hubConnection; // signalR
        private List<string> messages = new List<string>(); // for debugging signalR

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder().WithUrl(Navigation.ToAbsoluteUri("/msghub")).Build();
            hubConnection.On<string>("ReceiveUpdate", async message =>
            {
                var encodedMsg = $"{message}";
                messages.Add(encodedMsg);
                await LoadData();
                await InvokeAsync(StateHasChanged);

            });
            await hubConnection.StartAsync();
        }
        private async Task NotifyUpdate()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendUpdate", "A change has been made in student overview");
            }
        }
        protected override async Task OnParametersSetAsync()
        {
            await LoadData();
        }

        

        private async Task LoadData()
        {
            if (CurrentPage is null or < 1)
            {
                NavigationManager.NavigateTo("/students/list/1");
                return;
            }

            using var dbcontext = ContextFactory.CreateDbContext();


            var itemsToSkip = (CurrentPage.Value - 1) * ItemsPerPage;

            var authState = await authenticationState;

            var email = authState.User.Identity?.Name;


            Advisor = await dbcontext.Advisors
                .Where(adv => adv.Email == email)
                .FirstOrDefaultAsync();


            var studentCount = await dbcontext.Students.Where(stu => stu.AdvisorId == Advisor.Id).CountAsync();
            TotalPages = studentCount == 0
              ? 1
              : (int)Math.Ceiling((double)studentCount / ItemsPerPage);

            if (CurrentPage > TotalPages)
            {
                NavigationManager.NavigateTo($"/students/list/{TotalPages}");
                return;
            }

            StateContainer.StudentOverviewPage = CurrentPage.Value;

            

            
            if (Advisor == null)
                return;
            

            Students = await dbcontext.Students
              .Include(stu => stu.Advisor)
              .Where(stu => stu.AdvisorId == Advisor.Id && stu.InLine)
              .OrderBy(stu => stu.QueuePosition)
              .Skip(itemsToSkip)
              .Take(ItemsPerPage)
              .ToArrayAsync();
        }

        

        private async Task HandleDelete(Student? student)
        {
            if (student == null)
            {
                return;
            }
            var isOk = await JS.InvokeAsync<bool>("confirm",
              $"Remove student {student.FirstName} {student.LastName}?");

            if (isOk)
            {
                try
                {
                    using var context = ContextFactory.CreateDbContext();
                    context.Students.Remove(student);
                    await context.SaveChangesAsync();
                    HelperMethods.UpdatePosition(student.AdvisorId, context);
                    await LoadData();
                    await NotifyUpdate();
                    if (student.QueuePosition == 1)
                    {
                        var studentToNotify = await context.Students
                            .Where(stu =>stu.AdvisorId == Advisor.Id)
                            .FirstOrDefaultAsync(stu => stu.QueuePosition == 1);
                        if (studentToNotify != null)
                        {
                            await emailSender.SendEmailAsync(studentToNotify.Email,
                                "Front of Line",
                                "You are in the front of the line for office hours for " + @Advisor?.FirstName + " " + @Advisor?.LastName + ".");

                        }

                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }

                
            }
        }
    }
}
