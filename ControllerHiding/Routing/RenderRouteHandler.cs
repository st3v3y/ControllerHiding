using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ControllerHiding.Constants;
using ControllerHiding.Extensions;
using ControllerHiding.Models;

namespace ControllerHiding.Routing
{
    /// <summary>
    /// In this route handler we gonna catch the names of the hidden controllers/actions and return their routehandler
    /// </summary>
    public class RenderRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpRequestBase = requestContext.HttpContext.Request;

            string formData = httpRequestBase.Form.Get(KeyConstants.FormData);
            string getData = httpRequestBase.QueryString.Get(KeyConstants.GetData);

            if (string.IsNullOrEmpty(formData) && string.IsNullOrEmpty(getData))
            {
                return new MvcHandler(requestContext);
            }

            string hiddenController = null;
            string hiddenAction = null;
            //string area; //ignore area to make it easier in the beginning
            string identifier = null;

            if (httpRequestBase.RequestType == "POST")
            {
                var hiddenFormData = formData.Decrypt<HiddenFormData>();

                hiddenController = hiddenFormData.Controller;
                hiddenAction = hiddenFormData.Action;
                identifier = hiddenFormData.Identifier;
            }
            else if (requestContext.HttpContext.Request.RequestType == "GET")
            {
                //later maybe we want to support GET as well
            }
            else
            {
                //we don't want to handle other RequestTypes
                return null;
            }

            requestContext.RouteData.Values["controller"] = hiddenController;
            requestContext.RouteData.Values["action"] = hiddenAction;
            requestContext.RouteData.Values[KeyConstants.Identifier] = identifier;

            return new MvcHandler(requestContext);
        }
    }
}