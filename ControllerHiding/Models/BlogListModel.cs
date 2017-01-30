using ControllerHiding.DTO;

namespace ControllerHiding.Models
{
    public class BlogListModel
    {
        public PagingModel PagingModel { get; set; }
        public BlogEntry[] Entries { get; set; }
    }
}