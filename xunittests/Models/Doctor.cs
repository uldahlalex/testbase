﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dataaccess.Models;

[Table("doctors")]
public partial class Doctor
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("specialty")]
    public string Specialty { get; set; } = null!;

    [Column("years_experience")]
    public int? YearsExperience { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
}
