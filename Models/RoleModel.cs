using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Tranning.Validations;

namespace Tranning.Models
{
    public class RoleModel
    {
        public List<RoleDetail> RoleDetailLists { get; set; }
    }
    public class RoleDetail
    {
        public int id { get; set; }

        
        [Required(ErrorMessage = "Enter name, please")]
        public string name { get; set; }

        public string? description { get; set; }

        
        [Required(ErrorMessage = "Choose Status, please")]
        public string status { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }



    }
}
