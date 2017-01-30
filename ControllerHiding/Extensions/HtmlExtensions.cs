using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ControllerHiding.Constants;
using ControllerHiding.DTO;
using ControllerHiding.Routing;

namespace ControllerHiding.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString RenderModuleArea(this HtmlHelper htmlHelper, Page page, string moduleAreaName)
        {
            Contract.Requires(page != null);
            Contract.Requires(moduleAreaName != null);

            var moduleArea = page.ModuleAreas.FirstOrDefault(x => x.Name == moduleAreaName);
            if (moduleArea == null)
            {
                throw new Exception("Module Area does not exist. Name: " + moduleAreaName);
            }

            var htmlResult = new StringBuilder();
            foreach (var module in moduleArea.Modules)
            {
                htmlResult.Append(htmlHelper.ChildAction(module.Action, module.Controller, module.Identifier));
            }
            return MvcHtmlString.Create(htmlResult.ToString());
        }

        public static MvcHtmlString ChildAction(this HtmlHelper htmlHelper, string actionName, string controllerName, string identifier)
        {
            var viewData = htmlHelper.ViewData;
            var tempData = htmlHelper.ViewContext.TempData;

            object model = null;
            bool isFormSubmitted = false;

            object getData;
            if (tempData.TryGetValue(KeyConstants.GetData, out getData) && getData is HiddenGetData)
            {
                var hiddenGetData = (HiddenGetData)getData;
                if (hiddenGetData.Identifier == identifier)
                {
                    isFormSubmitted = true;
                    if (viewData.ModelState.IsValid)
                    {
                        actionName = hiddenGetData.RedirectAction;
                        model = hiddenGetData.SubModel;
                    }
                }
            }

            return htmlHelper.Action(actionName, controllerName, new { Identifier = identifier, Model = model, IsFormSubmitted = isFormSubmitted });
        }
    }
}