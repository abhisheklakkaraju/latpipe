using Microsoft.AspNetCore.Mvc.ModelBinding;
using MvcTemplate.Components.Notifications;
using System;

namespace MvcTemplate.Validators
{
    public interface IValidator : IDisposable
    {
        ModelStateDictionary ModelState { get; set; }
        Int64 CurrentAccountId { get; set; }
        Alerts Alerts { get; set; }
    }
}
