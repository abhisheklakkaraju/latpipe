using MvcTemplate.Data;
using MvcTemplate.Objects;
using System;
using System.Linq.Expressions;

namespace MvcTemplate.Validators.Tests
{
    public class AValidatorProxy : AValidator
    {
        public AValidatorProxy(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public Boolean BaseIsSpecified<TView>(TView view, Expression<Func<TView, Object?>> property) where TView : AView
        {
            return IsSpecified(view, property);
        }
    }
}
