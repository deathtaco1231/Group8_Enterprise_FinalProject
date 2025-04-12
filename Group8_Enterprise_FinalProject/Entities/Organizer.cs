using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Entities
{
    //Admin Role
    public class Organizer
    {
        //PK
        public int OrganizerId { get; set; }
        
        //Username (required)
        [Required(ErrorMessage = "Please enter a username")]
        public string? Name { get; set; }

        //Password (required)
        [Required(ErrorMessage = "Please enter a password")]
        public string? Password { get; set; }
    }
}
