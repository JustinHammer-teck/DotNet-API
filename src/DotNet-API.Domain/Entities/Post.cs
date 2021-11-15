using System;
using System.ComponentModel.DataAnnotations;

namespace DotNet_API.Domain.Entities
{
    public class Post
    {
        [Key] public Guid Id { get; set; }
        public string Title { get; set; }
    }
}