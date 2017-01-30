using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using ControllerHiding.Controllers.Base;

namespace ControllerHiding.Helper
{
    public static class ControllerHelper
    {
        public static Type GetControllerType(string controllerName)
        {
            return Assembly.GetCallingAssembly()
                .GetTypes()
                .FirstOrDefault(type => (type.IsSubclassOf(typeof(ChildController)) || type.IsSubclassOf(typeof(Controller))) && type.Name == controllerName + "Controller");
        }
    }
}