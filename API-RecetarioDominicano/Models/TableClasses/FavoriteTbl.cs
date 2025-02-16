using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace API_RecetarioDominicano.Models.TableClasses
{
    public class FavoriteTbl
    {
        [Key]
        public int FavoriteId { get; set; }

        public int UsertId { get; set; }
        public int RecipeId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
