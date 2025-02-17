using API_RecetarioDominicano.Models.Context;
using API_RecetarioDominicano.Models.TableClasses;
using API_RecetarioDominicano.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_RecetarioDominicano.Controllers
{
    [ApiController]
    [Route("api/")]
    public class RecipeCategoryController : Controller
    {
        private readonly DbConnectionContext _context;
        private readonly ILogger<RecipeCategoryController> _logger;
        private readonly MessageLoggingUtility errorUtility = new();

        public RecipeCategoryController(DbConnectionContext context, ILogger<RecipeCategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("RecipeCategory")]
        public async Task<IActionResult> CreateRecipeCategory([FromBody] RecipeCategoryTbl recp)
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

                recp.CreatedDateTime = DateTime.UtcNow;
                recp.ModifiedDateTime = DateTime.UtcNow;

                await _context.RecipeCategoryTbl.AddAsync(recp);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_CREATE"));
                return Ok(new
                {
                    success = true,
                    message = "RecipeCategory created successfully",
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
        [Route("RecipeCategory")]
        public async Task<IActionResult> ActiveRecipeCategoryAll([FromQuery] int? group, int? limit)
        {
            try
            {
                if (!group.HasValue || !limit.HasValue)
                {
                    var all = await _context.RecipeCategoryTbl.Select(p => new
                    {
                        p.RecipeCategoryId,
                        p.RecipeId,
                        p.CategoryId,
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
                            message = "All RecipeCategorys have been uploaded successfully.",
                            result = all
                        });
                    }
                }
                else
                {
                    int Offset = (int)((group * limit) - limit);
                    var recp = await _context.RecipeCategoryTbl.OrderBy(p => p.RecipeCategoryId)
                        .Skip(Offset)
                        .Take(limit.Value)
                        .Select(p => new
                        {
                            p.RecipeCategoryId,
                            p.RecipeId,
                            p.CategoryId,
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
                            message = "All RecipeCategorys have been uploaded successfully.",
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
        [Route("RecipeCategory/{id}")]
        public async Task<IActionResult> GetRecipeCategory(int id)
        {
            try
            {
                var recp = await _context.RecipeCategoryTbl.FindAsync(id);

                if (recp == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                return Ok(new
                {
                    success = true,
                    message = $"RecipeCategory with ID: {id}, fetched successfully",
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
        [Route("RecipeCategory/{id}")]
        public async Task<IActionResult> UpdateRecipeCategory(int id, [FromBody] RecipeCategoryTbl recp)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                var recpTbl = await _context.RecipeCategoryTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                recpTbl.RecipeId = recp.RecipeId;
                recpTbl.CategoryId = recp.CategoryId;
                recpTbl.ModifiedDateTime = DateTime.UtcNow;

                _context.RecipeCategoryTbl.Update(recpTbl);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"RecipeCategory with ID: {id}, updated successfully",
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
        [Route("RecipeCategory/{id}")]
        public async Task<IActionResult> DeleteRecipeCategory(int id)
        {
            try
            {
                var recpTbl = await _context.RecipeCategoryTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("DELETE_ED04"));
                    return NotFound(errorUtility.GetErrorDescription("DELETE_ED04"));
                }

                _context.RecipeCategoryTbl.Remove(recpTbl);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_DELETE"));
                return Ok(new
                {
                    success = true,
                    message = $"RecipeCategory with ID: {id}, deleted successfully",
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
