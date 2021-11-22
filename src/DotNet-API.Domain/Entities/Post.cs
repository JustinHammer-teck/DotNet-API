using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DotNet_API.Domain.Entities
{
    public class Post
    {
        [Key] public Guid Id { get; set; }
        public string Title { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}