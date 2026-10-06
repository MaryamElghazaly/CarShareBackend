using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CarShare.BLL.DTOs.Rental
{
    public class RentalProposalDTO
    {
        [Required]
        public Guid CarId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? LicenseVerificationUrl { get; set; }

        public string? AdditionalDocumentsUrl { get; set; }

        public string? Message { get; set; }

        public IFormFile? LicenseFile { get; set; }

        public IFormFile? AdditionalDocsFile { get; set; }
    }
}
