using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_RecetarioDominicano.Models.TableClasses
{
    public class UserTbl
    {
        [Key]
        public int UserId { get; set; }

        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string UserPhone { get; set; } = null!;
        public string UserRol { get; set; } = null!;
        public string UserSalt { get; set; } = null!;
        public string UserEncryptionKey { get; set; } = null!;
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
