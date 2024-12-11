using Business.Email.Dtos;
using Business.Email.Validator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Model.Enums;
using Model.Models;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CantactUsController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ILogger<WishlistController> _logger;
        public CantactUsController(EmailService emailService, ILogger<WishlistController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }
        [HttpPost("ContactUs")]
        [EnableRateLimiting("ContactUsPolicy")]
        public async Task<IActionResult> ContactUs([FromBody] ContactDto contactobject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var SentEmail = await _emailService.ReceiveEmail(contactobject);
                    if (SentEmail.Success)
                    {
                        return Ok(SentEmail);
                    }
                    else
                    {
                        _logger.LogError(SentEmail.DevelopMessage);
                        return Ok(SentEmail);
                    }
                }
                else
                {
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    return BadRequest(new { Errors = errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
        [HttpPost("GetAllEmails")]
        public async Task<IActionResult> GetAllEmails(EmailPagination emailmodel)
        {
            try
            {
                var result = await _emailService.GetAllReceivedEmails(emailmodel);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    _logger.LogError(result.DevelopMessage);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
        [HttpPost("EditEmailStatus")]
        public async Task<IActionResult> EditEmailStatus(ContactInfo contactobject)
        {
            try
            {
                var result = await _emailService.EditEmailStatus(contactobject);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    _logger.LogError(result.DevelopMessage);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
    }
}
