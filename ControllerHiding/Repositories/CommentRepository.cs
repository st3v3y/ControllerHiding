using System;
using System.Linq;
using ControllerHiding.DTO;

namespace ControllerHiding.Repositories
{
    public class CommentRepository : JsonBlogRepository
    {
        public Comment[] GetAllComments()
        {
            return BlogEntries().SelectMany(x => x.Comments).ToArray();
        }

        public Comment[] GetAllCommentsOfBlogEntry(Guid entryGuid)
        {
            return BlogEntries().Where(x => x.Id == entryGuid).SelectMany(x => x.Comments).ToArray();
        }

        public Comment[] GetLatestComments(int number)
        {
            return BlogEntries()
                .SelectMany(x => x.Comments)
                .OrderByDescending(x => x.Date)
                .Take(number)
                .ToArray();
        }
    }

}