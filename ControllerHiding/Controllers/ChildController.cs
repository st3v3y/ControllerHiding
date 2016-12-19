using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Profile;
using System.Web.Routing;
using ControllerHiding.Constants;
using ControllerHiding.Extensions;
using ControllerHiding.Models;
using ControllerHiding.Routing;

namespace ControllerHiding.Controllers
{
    public class ChildController : ControllerBase, IActionFilter, IDisposable
    {
        private ITempDataProvider _tempDataProvider;
        private IDependencyResolver _resolver;
        private IActionInvoker _actionInvoker;
        private ViewEngineCollection _viewEngineCollection;

        /// <summary>
        /// Gets the route data for the current request.
        /// </summary>
        /// <returns>
        /// The route data.
        /// </returns>
        public RouteData RouteData => ControllerContext?.RouteData;

        /// <summary>
        /// Gets the model state dictionary object that contains the state of the model and of model-binding validation.
        /// </summary>
        /// 
        /// <returns>
        /// The model state dictionary.
        /// </returns>
        public ModelStateDictionary ModelState => ViewData.ModelState;

        /// <summary>
        /// Gets HTTP-specific information about an individual HTTP request.
        /// </summary>
        /// 
        /// <returns>
        /// The HTTP context.
        /// </returns>
        public HttpContextBase HttpContext => ControllerContext?.HttpContext;

        /// <summary>
        /// Gets the user security information for the current HTTP request.
        /// </summary>
        /// 
        /// <returns>
        /// The user security information for the current HTTP request.
        /// </returns>
        public IPrincipal User => HttpContext?.User;

        /// <summary>
        /// Gets the HTTP context profile.
        /// </summary>
        /// 
        /// <returns>
        /// The HTTP context profile.
        /// </returns>
        public ProfileBase Profile => HttpContext?.Profile;

        /// <summary>
        /// Gets the HttpResponseBase object for the current HTTP response.
        /// </summary>
        /// 
        /// <returns>
        /// The HttpResponseBase object for the current HTTP response.
        /// </returns>
        public HttpResponseBase Response => HttpContext?.Response;

        /// <summary>
        /// Gets the HttpRequestBase object for the current HTTP request.
        /// </summary>
        /// 
        /// <returns>
        /// The request object.
        /// </returns>
        public HttpRequestBase Request => HttpContext?.Request;

        /// <summary>
        /// Gets the HttpServerUtilityBase object that provides methods that are used during Web request processing.
        /// </summary>
        /// 
        /// <returns>
        /// The HTTP server object.
        /// </returns>
        public HttpServerUtilityBase Server => HttpContext?.Server;

        /// <summary>
        /// Gets the HttpSessionStateBase object for the current HTTP request.
        /// </summary>
        /// 
        /// <returns>
        /// The HTTP session-state object for the current HTTP request.
        /// </returns>
        public HttpSessionStateBase Session => HttpContext?.Session;

        /// <summary>
        /// Gets the temporary-data provider object that is used to store data for the next request.
        /// </summary>
        /// 
        /// <returns>
        /// The temporary-data provider.
        /// </returns>
        public ITempDataProvider TempDataProvider
        {
            get { return _tempDataProvider ?? (_tempDataProvider = CreateTempDataProvider()); }
            set
            {
                _tempDataProvider = value;
            }
        }

        /// <summary>
        /// Represents a replaceable dependency resolver providing services. By default, it uses the <see cref="P:System.Web.Mvc.DependencyResolver.CurrentCache"/>.
        /// </summary>
        public IDependencyResolver Resolver
        {
            get
            {
                return _resolver ?? DependencyResolver.Current;
            }
            set
            {
                _resolver = value;
            }
        }

        /// <summary>
        /// Gets the view engine collection.
        /// </summary>
        /// 
        /// <returns>
        /// The view engine collection.
        /// </returns>
        public ViewEngineCollection ViewEngineCollection
        {
            get
            {
                return _viewEngineCollection ?? ViewEngines.Engines;
            }
            set
            {
                _viewEngineCollection = value;
            }
        }

        /// <summary>
        /// Gets the action invoker for the controller.
        /// </summary>
        /// 
        /// <returns>
        /// The action invoker.
        /// </returns>
        public IActionInvoker ActionInvoker
        {
            get { return _actionInvoker ?? (_actionInvoker = CreateActionInvoker()); }
            set
            {
                _actionInvoker = value;
            }
        }


        /// <summary>
        /// Creates a temporary data provider.
        /// </summary>
        /// 
        /// <returns>
        /// A temporary data provider.
        /// </returns>
        protected virtual ITempDataProvider CreateTempDataProvider()
        {
            ITempDataProviderFactory service = Resolver.GetService<ITempDataProviderFactory>();
            if (service != null)
            {
                return service.CreateInstance();
            }
            return Resolver.GetService<ITempDataProvider>() ?? new SessionStateTempDataProvider();
        }

        /// <summary>
        /// Creates an action invoker.
        /// </summary>
        /// 
        /// <returns>
        /// An action invoker.
        /// </returns>
        protected virtual IActionInvoker CreateActionInvoker()
        {
            IAsyncActionInvokerFactory service1 = Resolver.GetService<IAsyncActionInvokerFactory>();
            if (service1 != null)
            {
                return service1.CreateInstance();
            }

            IActionInvokerFactory service2 = Resolver.GetService<IActionInvokerFactory>();
            if (service2 != null)
            {
                return service2.CreateInstance();
            }
            return Resolver.GetService<IAsyncActionInvoker>() ?? Resolver.GetService<IActionInvoker>() ?? new AsyncControllerActionInvoker();
        }


        protected override void ExecuteCore()
        {
            PossiblyLoadTempData();
            try
            {
                string actionName = RouteData.GetRequiredString("action");
                if (ActionInvoker.InvokeAction(ControllerContext, actionName))
                {
                    return;
                }
                if (string.IsNullOrEmpty(actionName))
                {

                }
            }
            finally
            {
                PossiblySaveTempData();
            }
        }

        internal void PossiblyLoadTempData()
        {
            if (ControllerContext.IsChildAction) //TODO: always child? -> but it would never load tempdata
            {
                return;
            }
            TempData.Load(ControllerContext, TempDataProvider);
        }

        internal void PossiblySaveTempData()
        {
            if (ControllerContext.IsChildAction) //TODO: always child? -> but it would never save tempdata
            {
                return;
            }
            TempData.Save(ControllerContext, TempDataProvider);
        }

        protected virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        protected virtual void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            OnActionExecuting(filterContext);
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Returns an instance of the <see cref="T:System.Web.Mvc.HttpNotFoundResult"/> class.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of the <see cref="T:System.Web.Mvc.HttpNotFoundResult"/> class.
        /// </returns>
        protected internal HttpNotFoundResult HttpNotFound()
        {
            return HttpNotFound(null);
        }

        /// <summary>
        /// Returns an instance of the <see cref="T:System.Web.Mvc.HttpNotFoundResult"/> class.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of the <see cref="T:System.Web.Mvc.HttpNotFoundResult"/> class.
        /// </returns>
        /// <param name="statusDescription">The status description.</param>
        protected internal virtual HttpNotFoundResult HttpNotFound(string statusDescription)
        {
            return new HttpNotFoundResult(statusDescription);
        }

        /// <summary>
        /// Redirects to the specified action using the action name, controller name, and route values.
        /// </summary>
        /// 
        /// <returns>
        /// The redirect result object.
        /// </returns>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="model"></param>
        protected internal virtual RedirectToRouteResult RedirectToAction(string actionName, object model = null)
        {
            TempData[KeyConstants.GetData] = new HiddenGetData()
            {
                Identifier = RouteData.Values[KeyConstants.Identifier]?.ToString(),
                RedirectAction = actionName,
                SubModel = model,
            };

            return new RedirectToRouteResult(new RouteValueDictionary(new
            {
                controller = "Home",
                action = "Index"
            }));
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult"/> object that renders a view to the response.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="M:System.Web.Mvc.Controller.View"/> result that renders a view to the response.
        /// </returns>
        protected internal ActionResult View()
        {
            return View(null, null, null);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult"/> object by using the model that renders a view to the response.
        /// </summary>
        /// 
        /// <returns>
        /// The view result.
        /// </returns>
        /// <param name="model">The model that is rendered by the view.</param>
        protected internal ActionResult View(object model)
        {
            return View(null, null, model);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult"/> object by using the view name that renders a view.
        /// </summary>
        /// 
        /// <returns>
        /// The view result.
        /// </returns>
        /// <param name="viewName">The name of the view that is rendered to the response.</param>
        protected internal ActionResult View(string viewName)
        {
            return View(viewName, null, null);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult"/> object using the view name and master-page name that renders a view to the response.
        /// </summary>
        /// 
        /// <returns>
        /// The view result.
        /// </returns>
        /// <param name="viewName">The name of the view that is rendered to the response.</param><param name="masterName">The name of the master page or template to use when the view is rendered.</param>
        protected internal ActionResult View(string viewName, string masterName)
        {
            return View(viewName, masterName, null);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult"/> object that renders the specified IView object.
        /// </summary>
        /// 
        /// <returns>
        /// The view result.
        /// </returns>
        /// <param name="viewName">The view that is rendered to the response.</param><param name="model">The model that is rendered by the view.</param>
        protected internal ActionResult View(string viewName, object model)
        {
            return View(viewName, null, model);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult"/> object using the view name, master-page name, and model that renders a view.
        /// </summary>
        /// 
        /// <returns>
        /// The view result.
        /// </returns>
        /// <param name="viewName">The name of the view that is rendered to the response.</param><param name="masterName">The name of the master page or template to use when the view is rendered.</param><param name="model">The model that is rendered by the view.</param>
        protected internal virtual ActionResult View(string viewName, string masterName, object model)
        {
            if (ControllerContext.ParentActionViewContext == null)
            {
                TempData[KeyConstants.GetData] = new HiddenGetData()
                {
                    Identifier = RouteData.Values[KeyConstants.Identifier]?.ToString(),
                    RedirectAction = viewName,
                    SubModel = model,
                };

                RouteData.Values["controller"] = "Home";
                RouteData.Values["action"] = "Index";

                return new HomeActionResult();
            }

            if (model != null)
            {
                ViewData.Model = model;
            }

            return new ViewResult
            {
                ViewName = viewName,
                TempData = TempData,
                ViewData = ViewData,
                ViewEngineCollection = ViewEngineCollection
            };
        }

        /// <summary>
        /// Releases all resources that are used by the current instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}