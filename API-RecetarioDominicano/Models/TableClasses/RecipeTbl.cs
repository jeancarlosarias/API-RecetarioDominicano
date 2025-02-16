using System.ComponentModel.DataAnnotations;

namespace API_RecetarioDominicano.Models.TableClasses
{
    public class RecipeTbl
    {
        [Key]
        public int RecipeId { get; set; }

        public int UserId { get; set; }
        public string RecipeName { get; set; } = null!;
        public string RecipeDescription { get; set; } = null!;
        public string RecipeInstruction { get; set; } = null!;
        public TimeOnly RecipePreparationTime { get; set; }
        public int RecipePortion { get; set; }
        public bool IsExternal { get; set; }
        public long IdExternal { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
