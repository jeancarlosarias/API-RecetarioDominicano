using API_RecetarioDominicano.Models.Context;
using API_RecetarioDominicano.Models.TableClasses;
using API_RecetarioDominicano.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_RecetarioDominicano.Controllers
{
    [ApiController]
    [Route("api/")]
    public class RecipeIngredientController : Controller
    {
        private readonly DbConnectionContext _context;
        private readonly ILogger<RecipeIngredientController> _logger;
        private readonly MessageLoggingUtility errorUtility = new();

        public RecipeIngredientController(DbConnectionContext context, ILogger<RecipeIngredientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("RecipeIngredient")]
        public async Task<IActionResult> CreateRecipeIngredient([FromBody] RecipeIngredientTbl recp)
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

                var recipe = await _context.RecipeTbl.FindAsync(recp.RecipeId);
                var ingr = await _context.RecipeTbl.FindAsync(recp.IngredientId);

                if (recipe == null || ingr ==  null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC05"));
                    return NotFound(errorUtility.GetErrorDescription("CREATE_EC05"));
                }

                recp.CreatedDateTime = DateTime.UtcNow;
                recp.ModifiedDateTime = DateTime.UtcNow;

                await _context.RecipeIngredientTbl.AddAsync(recp);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_CREATE"));
                return Ok(new
                {
                    success = true,
                    message = "RecipeIngredient created successfully",
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
        [Route("RecipeIngredient")]
        public async Task<IActionResult> ActiveRecipeIngredientAll([FromQuery] int? group, int? limit)
        {
            try
            {
                if (!group.HasValue || !limit.HasValue)
                {
                    var all = await _context.RecipeIngredientTbl.Select(p => new
                    {
                        p.RecipeIngredientId,
                        p.IngredientId,
                        p.RecipeId,
                        p.RecipeIngredientQuantity,
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
                            message = "All RecipeIngredients have been uploaded successfully.",
                            result = all
                        });
                    }
                }
                else
                {
                    int Offset = (int)((group * limit) - limit);
                    var recp = await _context.RecipeIngredientTbl.OrderBy(p => p.RecipeIngredientId)
                        .Skip(Offset)
                        .Take(limit.Value)
                        .Select(p => new
                        {
                            p.RecipeIngredientId,
                            p.IngredientId,
                            p.RecipeId,
                            p.RecipeIngredientQuantity,
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
                            message = "All RecipeIngredients have been uploaded successfully.",
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
        [Route("RecipeIngredient/{id}")]
        public async Task<IActionResult> GetRecipeIngredient(int id)
        {
            try
            {
                var recp = await _context.RecipeIngredientTbl.FindAsync(id);

                if (recp == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                return Ok(new
                {
                    success = true,
                    message = $"RecipeIngredient with ID: {id}, fetched successfully",
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
        [Route("RecipeIngredient/{id}")]
        public async Task<IActionResult> UpdateRecipeIngredient(int id, [FromBody] RecipeIngredientTbl recp)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                var recpTbl = await _context.RecipeIngredientTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                recpTbl.RecipeId = recp.RecipeId;
                recpTbl.IngredientId = recp.IngredientId;
                recpTbl.RecipeIngredientQuantity = recp.RecipeIngredientQuantity;
                recpTbl.ModifiedDateTime = DateTime.UtcNow;

                _context.RecipeIngredientTbl.Update(recpTbl);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"RecipeIngredient with ID: {id}, updated successfully",
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
        [Route("RecipeIngredient/{id}")]
        public async Task<IActionResult> DeleteRecipeIngredient(int id)
        {
            try
            {
                var recpTbl = await _context.RecipeIngredientTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("DELETE_ED04"));
                    return NotFound(errorUtility.GetErrorDescription("DELETE_ED04"));
                }

                _context.RecipeIngredientTbl.Remove(recpTbl);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_DELETE"));
                return Ok(new
                {
                    success = true,
                    message = $"RecipeIngredient with ID: {id}, deleted successfully",
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
