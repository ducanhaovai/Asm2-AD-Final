﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Tranning.DataDBContext
{
    public class Role
    {
        [Key]
        public int id { get; set; }          

        [Column("name", TypeName = "Varchar(50)"), Required]
        public string name { get; set; }

        [Column("description", TypeName ="Varchar(150)"), AllowNull]
        public string? description { get; set; }


        [Column("status", TypeName = "Varchar(50)"), Required]
        public string status { get; set; }

        [AllowNull]
        public DateTime? created_at { get; set; }
        [AllowNull]
        public DateTime? updated_at { get; set; }
        [AllowNull]
        public DateTime? deleted_at { get; set; }

    }
}
