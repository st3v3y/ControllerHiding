using System;
using System.Web.Mvc;

namespace ControllerHiding.Routing
{
    public class HomeActionResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            var routeDef = (RouteDefinition)context.RouteData.DataTokens["MyHomepageRouteDefinition"];

            var factory = ControllerBuilder.Current.GetControllerFactory();

            context.RouteData.Values["controller"] = routeDef.ControllerName;
            context.RouteData.Values["action"] = routeDef.ActionName;

            ControllerBase controller = null;

            try
            {
                controller = factory.CreateController(context.RequestContext, routeDef.ControllerName) as ControllerBase;
                if (controller == null)
                {
                    throw new InvalidOperationException("Could not create controller with name " + routeDef.ControllerName + ".");
                }

                CopyControllerData(context, controller);

                ExecuteController(context, controller);
            }
            finally
            {
                DisposeController(controller, factory);
            }
        }

        private static void DisposeController(IController controller, IControllerFactory factory)
        {
            if (controller != null)
            {
                factory.ReleaseController(controller);
            }

            if (controller == null)
            {
                return;
            }

            var disposable = controller as IDisposable;
            disposable?.Dispose();
        }

        private static void ExecuteController(ControllerContext context, IController controller)
        {
            controller.Execute(context.RequestContext);
        }

        /// <summary>
        /// Copy ModelState, ViewData and TempData
        /// </summary>
        private static void CopyControllerData(ControllerContext context, ControllerBase controller)
        {
            controller.ViewData.ModelState.Merge(context.Controller.ViewData.ModelState);

            foreach (var data in context.Controller.ViewData)
            {
                controller.ViewData[data.Key] = data.Value;
            }

            var target = controller as Controller;
            var source = context.Controller as Controller;
            if (target != null && source != null)
            {
                target.TempDataProvider = source.TempDataProvider;
                target.TempData = source.TempData;
                target.TempData.Save(source.ControllerContext, source.TempDataProvider);
            }
        }
    }
}