using System.ComponentModel.DataAnnotations;

namespace API_RecetarioDominicano.Models.TableClasses
{
    public class IngredientTbl
    {
        [Key]
        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;
        public bool IsExternal { get; set; }
        public long IdExternal { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
