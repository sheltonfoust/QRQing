using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QRQing.LineManager.Data;
using QRQing.LineManager.Data.Models;
using QRQing.LineManager.Utilities;

namespace QRQing.LineManager.Pages
{
    partial class EditStudent
    {
        [Parameter]
        public int StudentId { get; set; }

        private int? OldQueuePosition;

        private Student? StudentToEdit { get; set; }

        private Advisor[]? Advisors { get; set; }

        private bool IsBusy { get; set; }

        private string? ErrorMessage { get; set; }

        private HubConnection? hubConnection; // signalR
        private List<string> messages = new List<string>(); // for debugging signalR

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder().WithUrl(Navigation.ToAbsoluteUri("/msghub")).Build();
            hubConnection.On<string>("ReceiveUpdate", async message =>
            {
                var encodedMsg = $"{message}";
                messages.Add(encodedMsg);

            });
            await hubConnection.StartAsync();
        }



        protected override async Task OnParametersSetAsync()
        {
            IsBusy = true;

            try
            {
                using var context = ContextFactory.CreateDbContext();
                Advisors = await context.Advisors
                                 .AsNoTracking()
                                 .OrderBy(adv => adv.FirstName)
                                 .ToArrayAsync();
                StudentToEdit = await context.Students
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(stu => stu.Id == StudentId);
                OldQueuePosition = StudentToEdit?.QueuePosition;
            }
            finally
            {
                IsBusy = false;
            }
        }



        private async Task HandleSubmit(bool isValid)
        {
            if (StudentToEdit is null || IsBusy || !isValid)
            {
                ErrorMessage = null;
                return;
            }

            IsBusy = true;
          
            try
            {
                using var context = ContextFactory.CreateDbContext();
               
                context.Update(StudentToEdit);

                await context.SaveChangesAsync();

                

                NavigateToOverviewPage();

                if (hubConnection is not null)
                {
                    await hubConnection.SendAsync("SendUpdate", "A change has been made in student overview");
                    if (StudentToEdit?.QueuePosition != OldQueuePosition) 
                    {
                        HelperMethods.UpdatePosition(StudentToEdit?.AdvisorId, context);
                    }
                }
                

            }
            catch (DbUpdateConcurrencyException)
            {
                ErrorMessage = "The student was modified by another user. Please reload this page.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error while saving student: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }

            
        }

        

        private void NavigateToOverviewPage()
        {
            NavigationManager.NavigateTo($"/students/list/{StateContainer.StudentOverviewPage}");
        }
    }
}
