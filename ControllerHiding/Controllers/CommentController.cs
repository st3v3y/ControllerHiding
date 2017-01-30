using System;
using System.Linq;
using System.Web.Mvc;
using ControllerHiding.Attributes;
using ControllerHiding.Constants;
using ControllerHiding.Controllers.Base;
using ControllerHiding.Models;
using ControllerHiding.Repositories;

namespace ControllerHiding.Controllers
{
    public class CommentController : BaseHideController
    {
        private readonly CommentRepository _commentRepository;
        private int commentsPerPage = 4;

        public CommentController()
        {
            _commentRepository = new CommentRepository();
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildRoute("{entryGuid}")]
        public ActionResult CommentList(Guid entryGuid)
        {
            var comments = _commentRepository.GetAllCommentsOfBlogEntry(entryGuid).OrderBy(x => x.Date).ToArray();
            return View(comments);
        }

        // GET: Comment
        [ChildRoute("{pageNumber}")]
        public ActionResult Latest(int pageNumber = 1)
        {
            var allComments = _commentRepository.GetLatestComments(10);
            var comments = allComments
                .OrderByDescending(x => x.Date)
                .Skip(commentsPerPage * (pageNumber - 1))
                .Take(commentsPerPage)
                .ToArray();

            return View(new LatestCommentsModel()
            {
                PagingModel = new PagingModel()
                {
                    Controller = "Comment",
                    Action = "Latest",
                    Identifier = ViewData[KeyConstants.Identifier]?.ToString(),
                    CurrentPage = pageNumber,
                    EntriesTotalCount = allComments.Count(),
                    EntriesPerPage = commentsPerPage, 
                },
                Entries = comments
            });
        }
    }
}