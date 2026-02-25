using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Api.Models;

[Table("StatusLog")]
public partial class StatusLog
{
    [Key]
    public Guid StatusLogId { get; set; }

    public Guid RecordId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime DateTimeCreated { get; set; }

    [ForeignKey("RecordId")]
    [InverseProperty("StatusLogs")]
    public virtual Record Record { get; set; } = null!;
}
