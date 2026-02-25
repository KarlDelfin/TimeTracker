using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Api.Models;

[Table("Activity")]
public partial class Activity
{
    [Key]
    public Guid ActivityId { get; set; }

    public Guid UserId { get; set; }

    public Guid? AssignedUserId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int EstimatedTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateTimeCreated { get; set; }

    [ForeignKey("AssignedUserId")]
    [InverseProperty("ActivityAssignedUsers")]
    public virtual User? AssignedUser { get; set; }

    [InverseProperty("Activity")]
    public virtual ICollection<Record> Records { get; set; } = new List<Record>();

    [ForeignKey("UserId")]
    [InverseProperty("ActivityUsers")]
    public virtual User User { get; set; } = null!;
}
