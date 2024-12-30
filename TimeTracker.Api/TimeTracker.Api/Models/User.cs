using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Api.Models
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            Activities = new HashSet<Activity>();
            Records = new HashSet<Record>();
            UserAssignments = new HashSet<UserAssignment>();
        }

        [Key]
        public Guid UserId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        [StringLength(100)]
        public string Password { get; set; } = null!;
        public string Image { get; set; } = null!;

        [InverseProperty("User")]
        public virtual ICollection<Activity> Activities { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Record> Records { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserAssignment> UserAssignments { get; set; }
    }
}
