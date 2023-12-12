using System.ComponentModel.DataAnnotations;
using Tranning.Validations;

namespace Tranning.Models
{
    public class UserModel
    {
        public List<UserDetail> UserDetailLists { get; set; }
    }

    public class UserDetail
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Enter Role, please")]
        public int role_id { get; set; }

        

        [Required(ErrorMessage = "Enter username, please")]
        public string username { get; set; }
        [Required(ErrorMessage = "Enter password, please")]
        public string password { get; set; }
        [Required(ErrorMessage = "Enter email, please")]
        public string email { get; set; }
        [Required(ErrorMessage = "Enter phone, please")]
        public string phone { get; set; }

        
        public string? address { get; set; }

        [Required(ErrorMessage = "Choose Gender, please")]
        public string gender { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? birthday { get; set; }
        [Required(ErrorMessage = "Enter full name, please")]
        public string full_name { get; set; }

        



        public string? avatar { get; set; }

        [Required(ErrorMessage = "Choose Status, please")]
        public string status { get; set; }
        

        [Required(ErrorMessage = "Choose file, please")]
        [AllowedExtensionFile(new string[] { ".png", ".jpg", ".jpeg" })]
        [AllowedSizeFile(5 * 1024 * 1024)]
        public IFormFile? Photo { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }
}
