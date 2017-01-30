using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ControllerHiding.Attributes;
using ControllerHiding.Constants;
using ControllerHiding.Controllers.Base;
using ControllerHiding.CustomAttributes;
using ControllerHiding.Models;
using ControllerHiding.Repositories;

namespace ControllerHiding.Controllers
{
    public class BlogController : BaseHideController
    {
        private readonly BlogEntryRepository _blogRepository;
        private int postsPerPage = 4;

        public BlogController()
        {
            _blogRepository = new BlogEntryRepository();
        }

        [ChildRoute("{entryGuid}")]
        public ActionResult BlogEntry(Guid entryGuid)
        {
            var blogEntry = _blogRepository.GetBlogEntryByGuid(entryGuid);
            if (blogEntry == null)
            {
                return HttpNotFound();
            }

            return View(blogEntry);
        }

        [ChildRoute("{year}/{pageNumber}")]
        [ChildRouteDefault("year", "all")]
        [ChildRouteBlogEntryYearConstraint("year")]
        public ActionResult BlogEntryList(int? year, int pageNumber = 1)
        {
            var allBlogEntries = _blogRepository.GetAllBlogEntries();

            if (year.HasValue)
            {
                allBlogEntries = allBlogEntries.Where(x => x.Date.Year == year.Value).ToArray();
            }

            var blogEntries = allBlogEntries
                .OrderByDescending(x => x.Date)
                .Skip(postsPerPage * (pageNumber-1))
                .Take(postsPerPage)
                .ToArray();

            return View(new BlogListModel()
            {
                PagingModel = new PagingModel()
                {
                    Controller = "Blog",
                    Action = "BlogEntryList",
                    Identifier = ViewData[KeyConstants.Identifier]?.ToString(),
                    CurrentPage = pageNumber,
                    EntriesTotalCount = allBlogEntries.Count(),
                    EntriesPerPage = postsPerPage,
                    RouteValues = new RouteValueDictionary(new { year })
                },
                Entries = blogEntries,
            });
        }
    }
}