using API_RecetarioDominicano.Models.Context;
using API_RecetarioDominicano.Models.TableClasses;
using API_RecetarioDominicano.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_RecetarioDominicano.Controllers
{
    [ApiController]
    [Route("api/")]
    public class FavoriteController : Controller
    {
        private readonly DbConnectionContext _context;
        private readonly ILogger<FavoriteController> _logger;
        private readonly MessageLoggingUtility errorUtility = new();

        public FavoriteController(DbConnectionContext context, ILogger<FavoriteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("Favorite")]
        public async Task<IActionResult> CreateFavorite([FromBody] FavoriteTbl recp)
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
                var recpe = await _context.RecipeTbl.FindAsync(recp.RecipeId);

                if (user == null || recpe == null) 
                {
                    _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC05"));
                    return NotFound(errorUtility.GetErrorDescription("CREATE_EC05"));
                }

                recp.CreatedDateTime = DateTime.UtcNow;
                recp.ModifiedDateTime = DateTime.UtcNow;

                await _context.FavoriteTbl.AddAsync(recp);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_CREATE"));
                return Ok(new
                {
                    success = true,
                    message = "Favorite created successfully",
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
        [Route("Favorite")]
        public async Task<IActionResult> ActiveFavoriteAll([FromQuery] int? group, int? limit)
        {
            try
            {
                if (!group.HasValue || !limit.HasValue)
                {
                    var all = await _context.FavoriteTbl.Select(p => new
                    {
                        p.FavoriteId,
                        p.UserId,
                        p.RecipeId,
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
                            message = "All Favorites have been uploaded successfully.",
                            result = all
                        });
                    }
                }
                else
                {
                    int Offset = (int)((group * limit) - limit);
                    var recp = await _context.FavoriteTbl.OrderBy(p => p.FavoriteId)
                        .Skip(Offset)
                        .Take(limit.Value)
                        .Select(p => new
                        {
                            p.FavoriteId,
                            p.UserId,
                            p.RecipeId,
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
                            message = "All Favorites have been uploaded successfully.",
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
        [Route("Favorite/{id}")]
        public async Task<IActionResult> GetFavorite(int id)
        {
            try
            {
                var recp = await _context.FavoriteTbl.FindAsync(id);

                if (recp == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                return Ok(new
                {
                    success = true,
                    message = $"Favorite with ID: {id}, fetched successfully",
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
        [Route("Favorite/{id}")]
        public async Task<IActionResult> UpdateFavorite(int id, [FromBody] FavoriteTbl recp)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                var recpTbl = await _context.FavoriteTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                recpTbl.UserId = recp.UserId;
                recpTbl.RecipeId = recp.RecipeId;
                recpTbl.ModifiedDateTime = DateTime.UtcNow;

                _context.FavoriteTbl.Update(recpTbl);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Favorite with ID: {id}, updated successfully",
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
        [Route("Favorite/{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            try
            {
                var recpTbl = await _context.FavoriteTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("DELETE_ED04"));
                    return NotFound(errorUtility.GetErrorDescription("DELETE_ED04"));
                }

                _context.FavoriteTbl.Remove(recpTbl);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_DELETE"));
                return Ok(new
                {
                    success = true,
                    message = $"Favorite with ID: {id}, deleted successfully",
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
