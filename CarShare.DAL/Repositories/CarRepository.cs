using CarShare.DAL.Data;
using CarShare.DAL.Interfaces;
using CarShare.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CarShare.DAL.Repositories
{
    public class CarRepository : Repository<Car>, ICarRepository
    {
        public CarRepository(CarShareDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.Owner)
                .Where(c => c.RentalStatus == Enums.RentalStatus.Available)
                .Where(c => c.IsApproved)
                .Where(c => !c.RentalProposals.Any(rp =>
                    rp.Status == Enums.ProposalStatus.Accepted &&
                    rp.StartDate <= endDate &&
                    rp.EndDate >= startDate))
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarsByOwnerAsync(Guid ownerId)
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .Include(c => c.CarImages)
                .Where(c => c.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> SearchCarsAsync(string searchTerm, decimal? maxPrice)
        {
            var query = _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.Owner)
                .Where(c => c.IsApproved);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Brand.Contains(searchTerm) ||
                    c.Model.Contains(searchTerm) ||
                    (c.Description != null && c.Description.Contains(searchTerm)));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.PricePerDay <= maxPrice.Value);
            }

            return await query.ToListAsync();
        }
    }
}