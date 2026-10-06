namespace CarShare.BLL.DTOs.Rental
{
    public class RentalResponseDTO
    {
        public Guid ProposalId { get; set; }
        public Guid CarId { get; set; }
        public Guid RenterId { get; set; }
        public string? CarTitle { get; set; }
        public string? RenterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? LicenseVerificationUrl { get; set; }
        public string? AdditionalDocumentsUrl { get; set; }
        public string? Message { get; set; }
    }
}