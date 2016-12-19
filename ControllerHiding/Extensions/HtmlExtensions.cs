using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ControllerHiding.Constants;
using ControllerHiding.Models;

namespace ControllerHiding.Extensions
{
    public static class HtmlExtensions
    {
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