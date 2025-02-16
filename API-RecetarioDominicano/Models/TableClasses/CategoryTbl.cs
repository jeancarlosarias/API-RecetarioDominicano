using System.ComponentModel.DataAnnotations;

namespace API_RecetarioDominicano.Models.TableClasses
{
    public class CategoryTbl
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;
        public bool IsExternal { get; set; }
        public long IdExternal { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
