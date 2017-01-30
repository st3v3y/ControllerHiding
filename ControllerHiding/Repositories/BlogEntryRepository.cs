using System;
using System.Linq;
using ControllerHiding.DTO;

namespace ControllerHiding.Repositories
{
    public class BlogEntryRepository : JsonBlogRepository
    {
        public BlogEntry[] GetAllBlogEntries()
        {
            return BlogEntries();
        }

        public BlogEntry GetBlogEntryByGuid(Guid entryGuid)
        {
            return BlogEntries().FirstOrDefault(x => x.Id == entryGuid);
        }
    }
}