using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using ControllerHiding.DTO;
using ControllerHiding.Repositories;

namespace ControllerHiding.Routing
{
    public class PagePathConstraint : IRouteConstraint
    {
        private readonly PageRepository _pageRepository;

        public PagePathConstraint()
        {
            _pageRepository = new PageRepository();
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            route.DataTokens.Clear();

            if (values[parameterName] == null)
            {
                return true;
            }

            var pagePath = values[parameterName].ToString();

            string[] pageSlugs = pagePath.Split('/');

            string[] identifierParams;
            var pages = ReceivePages(pageSlugs, out identifierParams);

            route.DataTokens.Add("page", pages.LastOrDefault());
            route.DataTokens.Add("pages", pages);
            route.DataTokens.Add("path", pagePath);
            route.DataTokens.Add("params", identifierParams);

            return true;
        }

        private List<Page> ReceivePages(string[] urlSegments, out string[] identifierParams)
        {
            var pages = new List<Page>();

            int i = 0;
            Page page;
            while (i < urlSegments.Length && IsPage(urlSegments[i], string.Join("/", urlSegments.Take(i + 1)), out page))
            {
                pages.Add(page);
                i++;
            }

            identifierParams = urlSegments.Skip(i).ToArray();

            return pages;
        }

        private bool IsPage(string urlSegment, string path, out Page page)
        {
            page = _pageRepository.GetPageByRoute(urlSegment);
            if (page == null)
            {
                return false;
            }
            return page.RoutePath == path;
        }
    }
}