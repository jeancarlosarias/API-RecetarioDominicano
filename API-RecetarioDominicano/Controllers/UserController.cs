using API_RecetarioDominicano.Models.Context;
using API_RecetarioDominicano.Utility;
using API_RecetarioDominicano.Models.TableClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StripeMatosBakery.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UserController : Controller
    {
        private readonly DbConnectionContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly MessageLoggingUtility errorUtility = new();

        public UserController(DbConnectionContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("CreateLogin/")]
        public async Task<IActionResult> CreateLogin([FromBody] UserTbl user, [FromHeader] string clientPassword)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                if (string.IsNullOrEmpty(user.UserEmail) || string.IsNullOrEmpty(clientPassword))
                {
                    _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC21"));
                    return NotFound(errorUtility.GetErrorDescription("CREATE_EC21"));
                }

                user.UserEmail = user.UserEmail.ToLower();

                var authenticationObjectFound = await _context.UserTbl.Where(a => a.UserEmail == user.UserEmail).FirstOrDefaultAsync();

                if (authenticationObjectFound != null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("AUTHENTICATION_AT04"));
                    return NotFound(errorUtility.GetErrorDescription("AUTHENTICATION_AT04"));
                }

                string salt = KeyValidationUtility.GenerateSalt();

                string encryptedPw = KeyValidationUtility.EncryptApiKey(clientPassword, salt);

                user.UserRol = string.IsNullOrEmpty(user.UserRol) || !"User|Admin".Contains(user.UserRol) ? "User" : user.UserRol;


                var authenticationTblNewObject = new UserTbl()
                {
                    UserFirstName = user.UserFirstName,
                    UserLastName = user.UserLastName,
                    UserEmail = user.UserEmail,
                    UserPhone = user.UserPhone,
                    UserRol = user.UserRol,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    UserSalt = salt,
                    UserEncryptionKey = encryptedPw,
                };

                await _context.UserTbl.AddAsync(authenticationTblNewObject);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_CREATE"));
                return Ok(new
                {
                    success = true,
                    message = "Login successfully registered",
                    result = authenticationTblNewObject
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
        [Route("Users")]
        public async Task<IActionResult> UserAll([FromQuery] int? group, int? limit)
        {
            try
            {
                if (!group.HasValue || !limit.HasValue)
                {
                    var all = await _context.UserTbl.Select(p => new
                    {
                        p.UserId,
                        p.UserFirstName,
                        p.UserLastName,
                        p.UserEmail,
                        p.UserPhone,
                        p.UserRol,
                        p.UserSalt,
                        p.UserEncryptionKey,
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
                            message = "All authentications have been uploaded successfully.",
                            result = all
                        });
                    }
                }
                else
                {
                    int Offset = (int)((group * limit) - limit);
                    var authentc = await _context.UserTbl.OrderBy(p => p.UserId)
                        .Skip(Offset)
                        .Take(limit.Value)
                        .Select(p => new
                        {
                            p.UserId,
                            p.UserFirstName,
                            p.UserLastName,
                            p.UserEmail,
                            p.UserPhone,
                            p.UserRol,
                            p.UserSalt,
                            p.UserEncryptionKey,
                            p.CreatedDateTime,
                            p.ModifiedDateTime
                        }).ToListAsync();

                    if (authentc.Count == 0)
                    {
                        _logger.LogInformation(errorUtility.GetInformationDescription("SUCCESSGET"));
                        return Ok(new
                        {
                            success = true,
                            message = "The search was successful, but no results were found.",
                            result = authentc
                        });
                    }
                    else
                    {
                        _logger.LogInformation(errorUtility.GetInformationDescription("SUCCESSGET"));
                        return Ok(new
                        {
                            success = true,
                            message = "All authentication have been uploaded successfully.",
                            result = authentc
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving clients.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server Error.",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("Users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var authetc = await _context.UserTbl.Select(p => new
                {
                    p.UserId,
                    p.UserFirstName,
                    p.UserLastName,
                    p.UserEmail,
                    p.UserPhone,
                    p.UserRol,
                    p.UserSalt,
                    p.UserEncryptionKey,
                    p.CreatedDateTime,
                    p.ModifiedDateTime
                }).FirstOrDefaultAsync(au => au.UserId == id);

                if (authetc == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                return Ok(new
                {
                    success = true,
                    message = $"User with ID: {id}, fetched successfully",
                    result = authetc
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
        [Route("Users/")]
        public async Task<IActionResult> UpdateUser([FromBody] UserTbl user, [FromHeader] string clientPassword)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC17"));
                return BadRequest(errorUtility.GetErrorDescription("CREATE_EC17"));
            }
            try
            {
                if (string.IsNullOrEmpty(user.UserEmail) || string.IsNullOrEmpty(clientPassword))
                {
                    _logger.LogError(errorUtility.GetErrorDescription("CREATE_EC21"));
                    return NotFound(errorUtility.GetErrorDescription("CREATE_EC21"));
                }
                user.UserEmail = user.UserEmail.ToLower();

                var authenticationObjectFound = await _context.UserTbl.Where(a => a.UserEmail == user.UserEmail).FirstOrDefaultAsync();

                if (authenticationObjectFound == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("AUTHENTICATION_AT02"));
                    return NotFound(errorUtility.GetErrorDescription("AUTHENTICATION_AT02"));
                }

                string salt = KeyValidationUtility.GenerateSalt();

                string encryptedPw = KeyValidationUtility.EncryptApiKey(clientPassword, salt);

                authenticationObjectFound.UserFirstName = user.UserFirstName;
                authenticationObjectFound.UserLastName = user.UserLastName;
                authenticationObjectFound.UserEmail = user.UserEmail;
                authenticationObjectFound.UserPhone = user.UserPhone;
                authenticationObjectFound.ModifiedDateTime = DateTime.UtcNow;
                authenticationObjectFound.UserSalt = salt;
                authenticationObjectFound.UserEncryptionKey = encryptedPw;

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_UPDATE"));
                _context.UserTbl.Update(authenticationObjectFound);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    message = "User updated successfully",
                    result = authenticationObjectFound
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
        [Route("Users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var authct = await _context.UserTbl.FindAsync(id);

                if (authct == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                    return NotFound(errorUtility.GetErrorDescription("FILE_NOT_FOUND"));
                }

                _context.UserTbl.Remove(authct);
                await _context.SaveChangesAsync();

                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_DELETE"));
                return Ok(new
                {
                    success = true,
                    message = $"User with ID: {id}, deleted successfully",
                    result = authct
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
        [Route("Login/")]
        public async Task<IActionResult> User([FromHeader] string clientEmail, [FromHeader] string clientPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(clientEmail) || string.IsNullOrEmpty(clientPassword))
                {
                    _logger.LogError(errorUtility.GetErrorDescription("AUTHENTICATION_AT01"));
                    return NotFound(errorUtility.GetErrorDescription("AUTHENTICATION_AT01"));
                }

                clientEmail = clientEmail.ToLower();

                var client = await _context.UserTbl.Where(c => c.UserEmail == clientEmail).FirstOrDefaultAsync();

                if (client == null)
                {
                    _logger.LogError(errorUtility.GetErrorDescription("AUTHENTICATION_AT01"));
                    return NotFound(errorUtility.GetErrorDescription("AUTHENTICATION_AT01"));
                }

                var keyValidationUtility = new KeyValidationUtility(_context, Request);
                var validationResult = keyValidationUtility.ValidateClientLogin(clientEmail, clientPassword);

                if (validationResult != "Success.")
                {
                    _logger.LogError(errorUtility.GetErrorDescription("AUTHENTICATION_AT01"));
                    return Unauthorized(new
                    {
                        success = true,
                        message = "Access denied. Password or Email do not match. Please try again.",
                        result = false
                    });
                }
                _logger.LogError(errorUtility.GetErrorDescription("SUCCESS_LOGIN"));
                return Ok(new
                {
                    success = true,
                    message = "Access granted. Welcome back.",
                    result = true
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

