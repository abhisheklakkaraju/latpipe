using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;

namespace MvcTemplate.Controllers.Tests
{
    public abstract class ControllerTests : IDisposable
    {
        public abstract void Dispose();

        protected void ReturnCurrentAccountId(BaseController controller, Int64 id)
        {
            controller.When(sub => { Int64 get = sub.CurrentAccountId; }).DoNotCallBase();
            controller.CurrentAccountId.Returns(id);
        }

        protected ViewResult NotFoundView(BaseController controller)
        {
            controller.When(sub => sub.NotFoundView()).DoNotCallBase();
            controller.NotFoundView().Returns(new ViewResult());

            return controller.NotFoundView();
        }
        protected ViewResult NotEmptyView(BaseController controller, Object model)
        {
            controller.When(sub => sub.NotEmptyView(model)).DoNotCallBase();
            controller.NotEmptyView(model).Returns(new ViewResult());

            return controller.NotEmptyView(model);
        }

        protected RedirectToActionResult RedirectToDefault(BaseController controller)
        {
            RedirectToActionResult result = new RedirectToActionResult(null, null, null);
            controller.When(sub => sub.RedirectToDefault()).DoNotCallBase();
            controller.RedirectToDefault().Returns(result);

            return result;
        }
        protected RedirectToActionResult RedirectToAction(BaseController controller, String action)
        {
            RedirectToActionResult result = new RedirectToActionResult(null, null, null);
            controller.When(sub => sub.RedirectToAction(action)).DoNotCallBase();
            controller.RedirectToAction(action).Returns(result);

            return result;
        }
        protected RedirectToActionResult RedirectToAction(BaseController baseController, String action, String controller)
        {
            RedirectToActionResult result = new RedirectToActionResult(null, null, null);
            baseController.When(sub => sub.RedirectToAction(action, controller)).DoNotCallBase();
            baseController.RedirectToAction(action, controller).Returns(result);

            return result;
        }
    }
}
