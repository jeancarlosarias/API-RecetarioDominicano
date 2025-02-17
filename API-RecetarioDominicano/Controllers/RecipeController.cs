using API_RecetarioDominicano.Models.Context;
using API_RecetarioDominicano.Models.TableClasses;
using API_RecetarioDominicano.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_RecetarioDominicano.Controllers
{
    [ApiController]
    [Route("api/")]
    public class RecipeController : Controller
    {
        private readonly DbConnectionContext _context;
        private readonly ILogger<RecipeController> _logger;
        private readonly MessageLoggingUtility errorUtility = new();

        public RecipeController(DbConnectionContext context, ILogger<RecipeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("Recipe")]
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeTbl recp)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                if (recp == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC21"));
                    return NotFound(errorUtility.GetErrorDescription("CREATE_EC21"));
                }
                
                var user = await _context.UserTbl.FindAsync(recp.UserId);

                if (user == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC05"));
                    return NotFound(errorUtility.GetErrorDescription("CREATE_EC05"));
                }

                recp.CreatedDateTime = DateTime.UtcNow;
                recp.ModifiedDateTime = DateTime.UtcNow;

                await _context.RecipeTbl.AddAsync(recp);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_CREATE"));
                return Ok(new
                {
                    success = true,
                    message = "Recipe created successfully",
                    result = recp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorUtility.GetErrorDescription("GENERAL_UNEXPECTED_ERROR"));
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("Recipe")]
        public async Task<IActionResult> ActiveRecipeAll([FromQuery] int? group, int? limit)
        {
            try
            {
                if (!group.HasValue || !limit.HasValue)
                {
                    var all = await _context.RecipeTbl.Select(p => new
                    {
                        p.RecipeId,
                        p.UserId,
                        p.RecipeName,
                        p.RecipeDescription,
                        p.RecipeInstruction,
                        p.RecipePreparationTime,
                        p.RecipePortion,
                        p.IsExternal,
                        p.IdExternal,
                        p.CreatedDateTime,
                        p.ModifiedDateTime
                    }).ToListAsync();

                    if (all.Count == 0)
                    {
                        _logger.LogError(errorUtility.GetErrorDescription("READ_ER10"));
                        return NotFound(errorUtility.GetErrorDescription("READ_ER10"));
                    }
                    else
                    {
                        _logger.LogInformation(errorUtility.GetInformationDescription("SUCCESSGET"));
                        return Ok(new
                        {
                            success = true,
                            message = "All Recipes have been uploaded successfully.",
                            result = all
                        });
                    }
                }
                else
                {
                    int Offset = (int)((group * limit) - limit);
                    var recp = await _context.RecipeTbl.OrderBy(p => p.RecipeId)
                        .Skip(Offset)
                        .Take(limit.Value)
                        .Select(p => new
                        {
                            p.RecipeId,
                            p.UserId,
                            p.RecipeName,
                            p.RecipeDescription,
                            p.RecipeInstruction,
                            p.RecipePreparationTime,
                            p.RecipePortion,
                            p.IsExternal,
                            p.IdExternal,
                            p.CreatedDateTime,
                            p.ModifiedDateTime
                        }).ToListAsync();

                    if (recp.Count == 0)
                    {
                        _logger.LogInformation(errorUtility.GetInformationDescription("SUCCESSGET"));
                        return Ok(new
                        {
                            success = true,
                            message = "The search was successful, but no results were found.",
                            result = recp
                        });
                    }
                    else
                    {
                        _logger.LogInformation(errorUtility.GetInformationDescription("SUCCESSGET"));
                        return Ok(new
                        {
                            success = true,
                            message = "All Recipes have been uploaded successfully.",
                            result = recp
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving recps.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server Error.",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("Recipe/{id}")]
        public async Task<IActionResult> GetRecipe(int id)
        {
            try
            {
                var recp = await _context.RecipeTbl.FindAsync(id);

                if (recp == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                return Ok(new
                {
                    success = true,
                    message = $"Recipe with ID: {id}, fetched successfully",
                    result = recp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorUtility.GetErrorDescription("GENERAL_UNEXPECTED_ERROR"));
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server Error",
                    error = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("Recipe/{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] RecipeTbl recp)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                var recpTbl = await _context.RecipeTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                recpTbl.RecipeName = recp.RecipeName;
                recpTbl.RecipeDescription = recp.RecipeDescription;
                recpTbl.RecipeInstruction = recp.RecipeInstruction;
                recpTbl.RecipePreparationTime = recp.RecipePreparationTime;
                recpTbl.RecipePortion = recp.RecipePortion;
                recpTbl.IsExternal = recp.IsExternal;
                recpTbl.IdExternal = recp.IdExternal;
                recpTbl.ModifiedDateTime = DateTime.UtcNow;

                _context.RecipeTbl.Update(recpTbl);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Recipe with ID: {id}, updated successfully",
                    result = recpTbl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorUtility.GetErrorDescription("GENERAL_UNEXPECTED_ERROR"));
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server Error",
                    error = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("Recipe/{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            try
            {
                var recpTbl = await _context.RecipeTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("DELETE_ED04"));
                    return NotFound(errorUtility.GetErrorDescription("DELETE_ED04"));
                }

                _context.RecipeTbl.Remove(recpTbl);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_DELETE"));
                return Ok(new
                {
                    success = true,
                    message = $"Recipe with ID: {id}, deleted successfully",
                    result = recpTbl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorUtility.GetErrorDescription("GENERAL_UNEXPECTED_ERROR"));
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server Error",
                    error = ex.Message
                });
            }
        }
    }
}
