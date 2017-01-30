using System.Web.Routing;

namespace ControllerHiding.Models
{
    public class PagingModel
    {
        public PagingModel()
        {
            PageNumberRouteParamaterName = "pageNumber";
            RouteValues = new RouteValueDictionary();
        }

        public int EntriesPerPage { get; set; }
        public int EntriesTotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PagesCount => (EntriesTotalCount + EntriesPerPage - 1) / EntriesPerPage;

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Identifier { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public string PageNumberRouteParamaterName { get; set; }
    }
}