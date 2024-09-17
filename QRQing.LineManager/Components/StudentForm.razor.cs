using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using QRQing.LineManager.Data.Models;

namespace QRQing.LineManager.Components
{
    partial class StudentForm
    {
        [Parameter]
        public Student? Student { get; set; }

        [Parameter]
        public Advisor[]? Advisors { get; set; }

        [Parameter]
        public bool IsBusy { get; set; }

        [Parameter]
        public bool IsEdit { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<bool> OnSubmit { get; set; }

        private async Task HandleValidSubmit()
        {
            if (!(Student?.QueuePosition <= 0) && !string.IsNullOrEmpty(Student?.FirstName) && !string.IsNullOrEmpty(Student?.LastName) && !string.IsNullOrEmpty(Student?.Email))
                if (OnSubmit.HasDelegate)
                {
                    await OnSubmit.InvokeAsync(true);
                }
        }

        private async Task HandleInvalidSubmit()
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(false);
            }
        }

        private async Task HandleCancel()
        {
            if (OnCancel.HasDelegate)
            {
                await OnCancel.InvokeAsync();
            }
        }

    }
}
