using System.ComponentModel.DataAnnotations;

namespace API_RecetarioDominicano.Models.TableClasses
{
    public class RecipeIngredientTbl
    {
        [Key]
        public int RecipeIngredientId { get; set; }

        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public int RecipeIngredientQuantity { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
