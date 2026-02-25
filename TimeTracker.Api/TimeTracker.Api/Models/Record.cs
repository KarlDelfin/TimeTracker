using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Api.Models;

[Table("Record")]
public partial class Record
{
    [Key]
    public Guid RecordId { get; set; }

    public Guid UserId { get; set; }

    public Guid ActivityId { get; set; }

    public float? CurrentRunningTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateTimeCreated { get; set; }

    [ForeignKey("ActivityId")]
    [InverseProperty("Records")]
    public virtual Activity Activity { get; set; } = null!;

    [InverseProperty("Record")]
    public virtual ICollection<StatusLog> StatusLogs { get; set; } = new List<StatusLog>();

    [ForeignKey("UserId")]
    [InverseProperty("Records")]
    public virtual User User { get; set; } = null!;
}
