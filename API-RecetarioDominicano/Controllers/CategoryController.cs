using API_RecetarioDominicano.Models.Context;
using API_RecetarioDominicano.Models.TableClasses;
using API_RecetarioDominicano.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_RecetarioDominicano.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CategoryController : Controller
    {
        private readonly DbConnectionContext _context;
        private readonly ILogger<CategoryController> _logger;
        private readonly MessageLoggingUtility errorUtility = new();

        public CategoryController(DbConnectionContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("Category")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryTbl recp)
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

                await _context.CategoryTbl.AddAsync(recp);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_CREATE"));
                return Ok(new
                {
                    success = true,
                    message = "Category created successfully",
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
        [Route("Category")]
        public async Task<IActionResult> ActiveCategoryAll([FromQuery] int? group, int? limit)
        {
            try
            {
                if (!group.HasValue || !limit.HasValue)
                {
                    var all = await _context.CategoryTbl.Select(p => new
                    {
                        p.CategoryId,
                        p.CategoryName,
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
                            message = "All Categorys have been uploaded successfully.",
                            result = all
                        });
                    }
                }
                else
                {
                    int Offset = (int)((group * limit) - limit);
                    var recp = await _context.CategoryTbl.OrderBy(p => p.CategoryId)
                        .Skip(Offset)
                        .Take(limit.Value)
                        .Select(p => new
                        {
                            p.CategoryId,
                            p.CategoryName,
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
                            message = "All Categorys have been uploaded successfully.",
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
        [Route("Category/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var recp = await _context.CategoryTbl.FindAsync(id);

                if (recp == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                return Ok(new
                {
                    success = true,
                    message = $"Category with ID: {id}, fetched successfully",
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
        [Route("Category/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryTbl recp)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                var recpTbl = await _context.CategoryTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                recpTbl.CategoryName = recp.CategoryName;
                recpTbl.IsExternal = recp.IsExternal;
                recpTbl.IdExternal = recp.IdExternal;
                recpTbl.ModifiedDateTime = DateTime.UtcNow;

                _context.CategoryTbl.Update(recpTbl);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Category with ID: {id}, updated successfully",
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
        [Route("Category/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var recpTbl = await _context.CategoryTbl.FindAsync(id);

                if (recpTbl == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("DELETE_ED04"));
                    return NotFound(errorUtility.GetErrorDescription("DELETE_ED04"));
                }

                _context.CategoryTbl.Remove(recpTbl);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_DELETE"));
                return Ok(new
                {
                    success = true,
                    message = $"Category with ID: {id}, deleted successfully",
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
