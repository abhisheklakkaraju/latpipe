using System;

namespace MvcTemplate.Services
{
    public interface IService : IDisposable
    {
        Int64 CurrentAccountId { get; set; }
    }
}
