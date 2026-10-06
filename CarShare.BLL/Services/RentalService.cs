using AutoMapper;
using CarShare.BLL.DTOs.Rental;
using CarShare.BLL.Interfaces;
using CarShare.DAL.Enums;
using CarShare.DAL.Interfaces;
using CarShare.DAL.Models;

namespace CarShare.BLL.Services
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RentalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RentalResponseDTO> CreateProposalAsync(RentalProposalDTO proposalDTO, Guid renterId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(proposalDTO.CarId);
            if (car == null) throw new KeyNotFoundException("Car not found");
            if (!car.IsApproved) throw new InvalidOperationException("Car is not approved for rental");

            var proposal = _mapper.Map<RentalProposal>(proposalDTO);
            proposal.RenterId = renterId;
            proposal.Status = ProposalStatus.Pending;

            proposal.LicenseVerificationUrl = proposalDTO.LicenseVerificationUrl ?? string.Empty;
            proposal.AdditionalDocumentsUrl = proposalDTO.AdditionalDocumentsUrl;

            await _unitOfWork.RentalProposals.AddAsync(proposal);
            await _unitOfWork.CommitAsync();

            // Reload proposal to include Renter and Car for full DTO mapping
            var fullProposal = await _unitOfWork.RentalProposals.GetByIdWithIncludesAsync(
                p => p.ProposalId == proposal.ProposalId,
                p => p.Car,
                p => p.Renter);

            return _mapper.Map<RentalResponseDTO>(fullProposal ?? proposal);
        }

        public async Task ApproveProposalAsync(Guid proposalId, Guid ownerId)
        {
            var proposal = await _unitOfWork.RentalProposals.GetByIdAsync(proposalId);
            if (proposal == null)
                throw new KeyNotFoundException("Proposal not found");

            var car = await _unitOfWork.Cars.GetByIdAsync(proposal.CarId);
            if (car == null)
                throw new KeyNotFoundException("Associated car not found");

            if (car.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Only the car owner can approve proposals for this car");

            proposal.Status = ProposalStatus.Accepted;
            car.RentalStatus = RentalStatus.Rented;

            await _unitOfWork.CommitAsync();
        }

        public async Task<RentalResponseDTO?> GetProposalByIdAsync(Guid proposalId)
        {
            var proposal = await _unitOfWork.RentalProposals
                .GetByIdWithIncludesAsync(p => p.ProposalId == proposalId,
                                          p => p.Car,
                                          p => p.Renter);

            if (proposal == null)
                return null;

            return _mapper.Map<RentalResponseDTO>(proposal);
        }
    }
}