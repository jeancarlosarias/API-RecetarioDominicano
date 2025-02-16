using System.ComponentModel.DataAnnotations;

namespace API_RecetarioDominicano.Models.TableClasses
{
    public class RecipeCategoryTbl
    {
        [Key]
        public int RecipeCategoryId { get; set; }

        public int RecipeId { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
