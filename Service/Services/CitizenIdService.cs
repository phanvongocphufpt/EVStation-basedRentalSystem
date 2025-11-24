using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.EmailConfirmation;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CitizenIdService : ICitizenIdService
    {
        private readonly ICitizenIdRepository _citizenIdRepository;
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        public CitizenIdService(ICitizenIdRepository citizenIdRepository, IMapper mapper, IRentalOrderRepository rentalOrderRepository, EmailService emailService, IUserRepository userRepository)
        {
            _citizenIdRepository = citizenIdRepository;
            _mapper = mapper;
            _rentalOrderRepository = rentalOrderRepository;
            _emailService = emailService;
            _userRepository = userRepository;
        }
        public async Task<Result<IEnumerable<CitizenIdDTO>>> GetAllCitizenIdsAsync()
        {
            var citizenIds = await _citizenIdRepository.GetAllCitizenIdsAsync();
            var dtos = _mapper.Map<IEnumerable<CitizenIdDTO>>(citizenIds);
            return Result<IEnumerable<CitizenIdDTO>>.Success(dtos);
        }
        public async Task<Result<CitizenIdDTO>> GetCitizenIdByIdAsync(int id)
        {
            var citizenId = await _citizenIdRepository.GetCitizenIdByIdAsync(id);
            if (citizenId == null)
            {
                return Result<CitizenIdDTO>.Failure("Chứng minh nhân dân không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<CitizenIdDTO>(citizenId);
            return Result<CitizenIdDTO>.Success(dto);
        }
        public async Task<Result<CitizenIdDTO>> GetCitizenIdByUserIdAsync(int id)
        {
            var citizenId = await _citizenIdRepository.GetCitizenIdsByUserIdAsync(id);
            if (citizenId == null)
            {
                return Result<CitizenIdDTO>.Failure("Chứng minh nhân dân không tồn tại cho User này! Kiểm tra lại UserId.");
            }
            var dto = _mapper.Map<CitizenIdDTO>(citizenId);
            return Result<CitizenIdDTO>.Success(dto);
        }
        public async Task<Result<CreateCitizenIdDTO>> CreateCitizenIdAsync(CreateCitizenIdDTO createCitizenIdDTO)
        {
            var dto = _mapper.Map<CitizenId>(createCitizenIdDTO);
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return Result<CreateCitizenIdDTO>.Failure("User không tồn tại! Kiểm tra lại Id của User.");
            }
            if (user.CitizenIdNavigation != null)
            {
                return Result<CreateCitizenIdDTO>.Failure("User đã có căn cước công dân rồi! Không thể tạo thêm.");
            }
            var citizenId = new CitizenId
            {
                CitizenIdNumber = dto.CitizenIdNumber,
                Name = dto.Name,
                BirthDate = dto.BirthDate,
                Status = DocumentStatus.Pending,
                CreatedAt = DateTime.Now,
                UserId = user.Id,
                User = user
            };
            await _citizenIdRepository.AddCitizenIdAsync(citizenId);
            return Result<CreateCitizenIdDTO>.Success(createCitizenIdDTO, "Tạo căn cước công dân thành công.");
        }
        public async Task<Result<UpdateCitizenIdStatusDTO>> UpdateCitizenIdStatusAsync(UpdateCitizenIdStatusDTO updateCitizenIdStatusDTO)
        {
            var citizenId = await _citizenIdRepository.GetCitizenIdByIdAsync(updateCitizenIdStatusDTO.CitizenIdId);
            if (citizenId == null)
            {
                return Result<UpdateCitizenIdStatusDTO>.Failure("Căn cước công dân không tồn tại! Kiểm tra lại Id.");
            }
            var user = await _userRepository.GetByIdAsync(citizenId.UserId);
            if (user == null)
            {
                return Result<UpdateCitizenIdStatusDTO>.Failure("User không tồn tại! Kiểm tra lại Id của user.");
            }
            citizenId.Status = updateCitizenIdStatusDTO.Status;
            citizenId.UpdatedAt = DateTime.Now;
            await _citizenIdRepository.UpdateCitizenIdAsync(citizenId);
            return Result<UpdateCitizenIdStatusDTO>.Success(updateCitizenIdStatusDTO, "Cập nhật trạng thái căn cước công dân thành công.");
        }
        public async Task<Result<UpdateCitizenIdInfoDTO>> UpdateCitizenIdInfoAsync(UpdateCitizenIdInfoDTO updateCitizenIdInfoDTO)
        {
            var citizenId = await _citizenIdRepository.GetCitizenIdByIdAsync(updateCitizenIdInfoDTO.Id);
            if (citizenId == null)
            {
                return Result<UpdateCitizenIdInfoDTO>.Failure("Căn cước công dân không tồn tại! Kiểm tra lại Id.");
            }
            citizenId.Name = updateCitizenIdInfoDTO.Name;
            citizenId.CitizenIdNumber = updateCitizenIdInfoDTO.CitizenIdNumber;
            citizenId.BirthDate = updateCitizenIdInfoDTO.BirthDate;
            citizenId.UpdatedAt = DateTime.Now;
            await _citizenIdRepository.UpdateCitizenIdAsync(citizenId);
            return Result<UpdateCitizenIdInfoDTO>.Success(updateCitizenIdInfoDTO, "Cập nhật thông tin căn cước công dân thành công.");
        }
    }
}