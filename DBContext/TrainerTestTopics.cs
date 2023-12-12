using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Tranning.DBContext
{
    public class TrainerTestTopics
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CourseId")]
        public int CourseId { get; set; }
        public TrainerTestTopics TrainerTestCTopics { get; set; }  // reference navigation

        [Column("NameTopic", TypeName = "Varchar(50)")]
        public string NameTopic { get; set; }

        [Column("Description", TypeName = "Varchar(100)")]
        public string Description { get; set; }

        [Column("Videos", TypeName = "Varchar(200)")]
        public string Videos { get; set; }

        [Column("Documents", TypeName = "Varchar(200)")]
        public string Documents { get; set; }

        [Column("Attach_File", TypeName = "Varchar(200)")]
        public string Attach_File { get; set; }

        [Column("Status", TypeName = "Integer")]
        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
