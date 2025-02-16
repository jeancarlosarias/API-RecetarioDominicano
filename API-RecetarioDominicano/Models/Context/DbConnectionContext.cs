using Microsoft.EntityFrameworkCore;
using API_RecetarioDominicano.Models.TableClasses;

namespace API_RecetarioDominicano.Models.Context
{
    public class DbConnectionContext : DbContext
    {
        public DbConnectionContext(DbContextOptions<DbConnectionContext> options) : base(options)
        {
        }

        public DbSet<CategoryTbl> CategoryTbl { get; set; } = null!;
        public DbSet<FavoriteTbl> FavoriteTbl { get; set; } = null!;
        public DbSet<IngredientTbl> IngredientTbl { get; set; } = null!;
        public DbSet<RecipeIngredientTbl> RecipeIngredientTbl { get; set; } = null!;
        public DbSet<RecipeCategoryTbl> RecipeCategoryTbl { get; set; } = null!;
        public DbSet<RecipeTbl> RecipeTbl { get; set; } = null!;
        public DbSet<UserTbl> UserTbl { get; set; } = null!;
    }
}
