using Business.Email.Dtos;
using Business.Email.Validator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Model.Models;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CantactUsController : Controller
    {
        private readonly EmailService _emailService;
        public CantactUsController(EmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost("ContactUs")]
        [EnableRateLimiting("ContactUsPolicy")]
        public IActionResult ContactUs([FromBody] ContactDto contactobject)
        {
            if (ModelState.IsValid) {
                var SentEmail = _emailService.RecieveEmail(contactobject);
                if (SentEmail)
                {
                    return Ok();
                }
                return BadRequest();
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
    }
}
