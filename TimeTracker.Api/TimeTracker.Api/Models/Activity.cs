using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Api.Models
{
    [Table("Activity")]
    public partial class Activity
    {
        public Activity()
        {
            Records = new HashSet<Record>();
        }

        [Key]
        public Guid ActivityId { get; set; }
        public Guid UserId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsAssigned { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateTimeCreated { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Activities")]
        public virtual User User { get; set; } = null!;
        [InverseProperty("Activity")]
        public virtual ICollection<Record> Records { get; set; }
    }
}
