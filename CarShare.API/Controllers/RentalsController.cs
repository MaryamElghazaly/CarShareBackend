using CarShare.BLL.DTOs.Rental;
using CarShare.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentalsController : BaseController
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        private async Task<string?> SaveFile(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > 10 * 1024 * 1024)  // 10MB limit
                throw new Exception("File size exceeds the limit.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Invalid file type. Allowed formats: .jpg, .jpeg, .png, .pdf");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + uniqueFileName;
        }

        [Authorize(Roles = "Renter")]
        [HttpPost("proposals")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProposal([FromForm] RentalProposalDTO proposalDTO)
        {
            var renterId = GetCurrentUserId();
            if (renterId == null)
                return Unauthorized("Invalid token: missing user ID.");

            // Save uploaded files and get URLs
            string? licenseUrl = await SaveFile(proposalDTO.LicenseFile);
            string? docsUrl = await SaveFile(proposalDTO.AdditionalDocsFile);

            var cleanDto = new RentalProposalDTO
            {
                CarId = proposalDTO.CarId,
                StartDate = proposalDTO.StartDate,
                EndDate = proposalDTO.EndDate,
                Message = proposalDTO.Message,
                LicenseFile = proposalDTO.LicenseFile,
                AdditionalDocsFile = proposalDTO.AdditionalDocsFile,
                LicenseVerificationUrl = licenseUrl,
                AdditionalDocumentsUrl = docsUrl
            };

            var result = await _rentalService.CreateProposalAsync(cleanDto, renterId.Value);
            return CreatedAtAction(nameof(GetProposal), new { id = result.ProposalId }, result);
        }

        [Authorize(Roles = "CarOwner")]
        [HttpPatch("proposals/{id}/approve")]
        public async Task<IActionResult> ApproveProposal(Guid id)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == null)
                return Unauthorized("Invalid token: missing user ID.");

            await _rentalService.ApproveProposalAsync(id, ownerId.Value);
            return NoContent();
        }

        [HttpGet("proposals/{id}")]
        public async Task<IActionResult> GetProposal(Guid id)
        {
            var proposal = await _rentalService.GetProposalByIdAsync(id);
            if (proposal == null)
                return NotFound();

            return Ok(proposal);
        }
    }
}