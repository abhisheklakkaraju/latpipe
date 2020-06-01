using MvcTemplate.Data;
using System;

namespace MvcTemplate.Services
{
    public abstract class AService : IService
    {
        public Int64 CurrentAccountId { get; set; }
        protected IUnitOfWork UnitOfWork { get; }

        protected AService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}
