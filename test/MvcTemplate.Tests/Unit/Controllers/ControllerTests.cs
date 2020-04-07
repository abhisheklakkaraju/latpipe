using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;

namespace MvcTemplate.Controllers.Tests
{
    public abstract class ControllerTests : IDisposable
    {
        public abstract void Dispose();

        protected ViewResult NotFoundView(BaseController controller)
        {
            controller.NotFoundView().Returns(new ViewResult());

            return controller.NotFoundView();
        }
        protected ViewResult NotEmptyView(BaseController controller, Object model)
        {
            controller.NotEmptyView(model).Returns(new ViewResult());

            return controller.NotEmptyView(model);
        }

        protected RedirectToActionResult RedirectToDefault(BaseController controller)
        {
            RedirectToActionResult result = new RedirectToActionResult(null, null, null);
            controller.RedirectToDefault().Returns(result);

            return result;
        }
        protected RedirectToActionResult RedirectToAction(BaseController controller, String action)
        {
            RedirectToActionResult result = new RedirectToActionResult(null, null, null);
            controller.RedirectToAction(action).Returns(result);

            return result;
        }
        protected RedirectToActionResult RedirectToAction(BaseController baseController, String action, String controller)
        {
            RedirectToActionResult result = new RedirectToActionResult(null, null, null);
            baseController.RedirectToAction(action, controller).Returns(result);

            return result;
        }
    }
}
