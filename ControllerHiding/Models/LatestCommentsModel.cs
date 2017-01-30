using ControllerHiding.DTO;

namespace ControllerHiding.Models
{
    public class LatestCommentsModel
    {
        public PagingModel PagingModel { get; set; }
        public Comment[] Entries { get; set; }
    }
}