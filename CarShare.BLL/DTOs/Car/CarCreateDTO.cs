using System.ComponentModel.DataAnnotations;

namespace CarShare.BLL.DTOs.Car
{
    public class CarCreateDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string CarType { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public string Transmission { get; set; } = string.Empty;

        public int Seats { get; set; } = 5;

        public string FuelType { get; set; } = "Gasoline";

        [Required]
        public decimal PricePerDay { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string LicensePlate { get; set; } = string.Empty;
    }
}