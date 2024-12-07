using Microsoft.AspNetCore.Mvc;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "public");

        [HttpPost("saveFile")]
        public async Task<IActionResult> SaveFile()
        {
            if (Request.Form.Files.Count == 0)
            {
                return BadRequest(new { Message = "No file uploaded." });
            }

            var file = Request.Form.Files[0];
            if (file.Length <= 0)
            {
                return BadRequest(new { Message = "Empty file." });
            }

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            var filePath = Path.Combine(_basePath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/public/{file.FileName}";
            return Ok(new { Url = fileUrl });
        }
    }

}
