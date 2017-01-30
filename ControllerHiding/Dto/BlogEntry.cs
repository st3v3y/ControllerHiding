using System;
using System.Collections.Generic;

namespace ControllerHiding.DTO
{
    public class BlogEntry
    {
        public Guid Id { get; set; }
        public Author Author { get; set; }
        public string Title { get; set; }
        public string Teaser { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string[] Tags { get; set; }
        public DateTime Date  { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}