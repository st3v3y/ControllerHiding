using System;

namespace ControllerHiding.DTO
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Author Author { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}