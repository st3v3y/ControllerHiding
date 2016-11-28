using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace ControllerHiding.Filters
{
    public class HideActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var viewData = filterContext.Controller.ViewData;
            var tempData = filterContext.Controller.TempData;

            object identifier;
            if (filterContext.RouteData.Values.TryGetValue("identifier", out identifier))
            {
                viewData.Add("identifier", identifier);

                object formIdentifier;
                if (tempData.TryGetValue("formIdentifier", out formIdentifier) && identifier.ToString() == formIdentifier?.ToString())
                {
                    object modelState;
                    if (tempData.TryGetValue("ModelState", out modelState))
                    {
                        viewData["IsPostedModel"] = true;
                        viewData.ModelState.Merge((ModelStateDictionary)modelState);
                    }
                }
            }
        }
    }
}