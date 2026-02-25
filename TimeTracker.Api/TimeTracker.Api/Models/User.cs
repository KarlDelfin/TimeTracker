using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Api.Models;

[Table("User")]
public partial class User
{
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

    public int? VerificationCode { get; set; }

    [InverseProperty("AssignedUser")]
    public virtual ICollection<Activity> ActivityAssignedUsers { get; set; } = new List<Activity>();

    [InverseProperty("User")]
    public virtual ICollection<Activity> ActivityUsers { get; set; } = new List<Activity>();

    [InverseProperty("User")]
    public virtual ICollection<Record> Records { get; set; } = new List<Record>();

    [InverseProperty("User")]
    public virtual ICollection<UserAssignment> UserAssignments { get; set; } = new List<UserAssignment>();
}
