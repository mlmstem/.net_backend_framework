
using System;
using System.Collections.Generic;
namespace API.Models{
    public class Post
    {
        public int Id { get; set; } // Unique identifier for the post
        public string Title { get; set; } = string.Empty; // Title of the post
        public string Author { get; set; } = string.Empty; // Author's name
        public string Date { get; set; } = string.Empty; // Date of creation or update
        public string Category { get; set; } = string.Empty; // Category of the post
        public string Content { get; set; } = string.Empty; // Content of the post
        public List<Comment> Comments { get; set; } = new List<Comment>(); // List of comments
        public bool ShowComments { get; set; } // Indicates whether to show comments
    }

    public class Comment
    {
        public string Content { get; set; } = string.Empty; // Comment content
        public string Date { get; set; } = string.Empty; // Date of the comment
    }

}