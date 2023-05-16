using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityLogin.Test.AspNetCore.Models
{
    public class ValueStore
    {
        [Key]
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public int Value { get; set; }

        public IdentityUser User { get; set; }
    }
}
