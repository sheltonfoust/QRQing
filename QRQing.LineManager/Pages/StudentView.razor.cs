using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using QRQing.LineManager.Authentication;
using QRQing.LineManager.Data.Models;
using QRQing.LineManager.Utilities;

namespace QRQing.LineManager.Pages
{
    public partial class StudentView
    {
        private Student? _Student;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState {  get; set; }


        private HubConnection? hubConnection;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder().WithUrl(NavManager.ToAbsoluteUri("/msghub")).Build();
            hubConnection.On<string>("ReceiveUpdate", async message =>
            {
                await LoadStudent();
                await InvokeAsync(StateHasChanged);

            });
            await hubConnection.StartAsync();
        }

        private async Task NotifyUpdate()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendUpdate", "A student has removed themself");
            }
        }


        protected override async Task OnParametersSetAsync()
        {
            await LoadStudent();
        }

        

        private async Task LoadStudent()
        {
            var authState = await authenticationState;
            var email = authState.User.Identity?.Name;

            using var dbcontext = ContextFactory.CreateDbContext();

            _Student = await dbcontext.Students
                .Where(stu => stu.Email == email)
                .Include(stu => stu.Advisor)
                .FirstOrDefaultAsync();

            if (_Student == null)
                return;
        }

        private async Task LeaveQueue()
        {
            using var context = ContextFactory.CreateDbContext();
            context.Students.Remove(_Student);
            await context.SaveChangesAsync();
            HelperMethods.UpdatePosition(_Student.AdvisorId, context);

            var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
            await customAuthStateProvider.UpdateAuthenticationState(null);
            await NotifyUpdate();
            NavManager.NavigateTo("/studentlogin", true);
            
        }
    }

    
}
