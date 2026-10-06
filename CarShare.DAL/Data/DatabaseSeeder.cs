using CarShare.DAL.Enums;
using CarShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CarShare.DAL.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(CarShareDbContext context)
        {
            // Ensure DB is created and migrated
            await context.Database.MigrateAsync();

            // Seed Users if not present
            if (!await context.Users.AnyAsync())
            {
                var adminSalt = GenerateSalt();
                var adminHash = HashPassword("Password123!", adminSalt);
                var adminUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@carshare.com",
                    FirstName = "System",
                    LastName = "Admin",
                    PhoneNumber = "+201000000001",
                    Role = UserRole.Admin,
                    IsVerified = true,
                    IsActive = true,
                    PasswordHash = adminHash,
                    PasswordSalt = adminSalt,
                    CreatedAt = DateTime.UtcNow
                };

                var ownerSalt = GenerateSalt();
                var ownerHash = HashPassword("Password123!", ownerSalt);
                var ownerUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "carowner",
                    Email = "owner@carshare.com",
                    FirstName = "Mohamed",
                    LastName = "Ali",
                    PhoneNumber = "+201000000002",
                    Role = UserRole.CarOwner,
                    IsVerified = true,
                    IsActive = true,
                    PasswordHash = ownerHash,
                    PasswordSalt = ownerSalt,
                    CreatedAt = DateTime.UtcNow
                };

                var renterSalt = GenerateSalt();
                var renterHash = HashPassword("Password123!", renterSalt);
                var renterUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "renter",
                    Email = "renter@carshare.com",
                    FirstName = "Sara",
                    LastName = "Ahmed",
                    PhoneNumber = "+201000000003",
                    Role = UserRole.Renter,
                    IsVerified = true,
                    IsActive = true,
                    PasswordHash = renterHash,
                    PasswordSalt = renterSalt,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddRangeAsync(adminUser, ownerUser, renterUser);
                await context.SaveChangesAsync();

                // Seed Sample Approved Cars for Owner
                var car1 = new Car
                {
                    CarId = Guid.NewGuid(),
                    OwnerId = ownerUser.UserId,
                    Title = "Mercedes-Benz C200 2023",
                    Description = "Luxury sedan in pristine condition, perfect for business trips and weekend getaways.",
                    Brand = "Mercedes-Benz",
                    Model = "C200",
                    Year = 2023,
                    CarType = CarType.Luxury,
                    Transmission = TransmissionType.Automatic,
                    FuelType = FuelType.Gasoline,
                    Seats = 5,
                    LicensePlate = "ABC-1234",
                    Location = "Cairo, New Cairo",
                    PricePerDay = 1500.00m,
                    RentalStatus = RentalStatus.Available,
                    IsApproved = true,
                    AverageRating = 4.8m,
                    CreatedAt = DateTime.UtcNow
                };

                var car2 = new Car
                {
                    CarId = Guid.NewGuid(),
                    OwnerId = ownerUser.UserId,
                    Title = "Toyota Camry 2024",
                    Description = "Comfortable, fuel efficient, and reliable modern sedan with full insurance.",
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2024,
                    CarType = CarType.Sedan,
                    Transmission = TransmissionType.Automatic,
                    FuelType = FuelType.Hybrid,
                    Seats = 5,
                    LicensePlate = "XYZ-5678",
                    Location = "Giza, Sheikh Zayed",
                    PricePerDay = 900.00m,
                    RentalStatus = RentalStatus.Available,
                    IsApproved = true,
                    AverageRating = 4.9m,
                    CreatedAt = DateTime.UtcNow
                };

                var car3 = new Car
                {
                    CarId = Guid.NewGuid(),
                    OwnerId = ownerUser.UserId,
                    Title = "Hyundai Tucson 2023",
                    Description = "Spacious modern SUV suitable for family vacations and long road trips.",
                    Brand = "Hyundai",
                    Model = "Tucson",
                    Year = 2023,
                    CarType = CarType.SUV,
                    Transmission = TransmissionType.Automatic,
                    FuelType = FuelType.Gasoline,
                    Seats = 5,
                    LicensePlate = "EGY-9988",
                    Location = "Alexandria, Stanley",
                    PricePerDay = 1100.00m,
                    RentalStatus = RentalStatus.Available,
                    IsApproved = true,
                    AverageRating = 4.7m,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Cars.AddRangeAsync(car1, car2, car3);
                await context.SaveChangesAsync();
            }
        }

        private static byte[] GenerateSalt()
        {
            using var hmac = new HMACSHA512();
            return hmac.Key;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
