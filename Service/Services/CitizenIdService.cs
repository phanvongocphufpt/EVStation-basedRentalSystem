using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public CitizenIdService(ICitizenIdRepository citizenIdRepository, IMapper mapper, IUserRepository userRepository)
        {
            _citizenIdRepository = citizenIdRepository;
            _mapper = mapper;
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
        public async Task<Result<CitizenIdDTO>> GetCitizenIdByUserIdAsync(int userId)
        {
            var citizenId = await _citizenIdRepository.GetCitizenIdsByUserIdAsync(userId);
            if (citizenId == null)
            {
                return Result<CitizenIdDTO>.Failure("Chứng minh nhân dân không tồn tại cho người dùng này! Kiểm tra lại UserId.");
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
                return Result<CreateCitizenIdDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id của người dùng.");
            }
            if (user.CitizenIdNavigation != null)
            {
                return Result<CreateCitizenIdDTO>.Failure("Người dùng đã có chứng minh nhân dân rồi! Không thể tạo thêm.");
            }
            var citizenId = new CitizenId
            {
                CitizenIdNumber = dto.CitizenIdNumber,
                Name = dto.Name,
                BirthDate = dto.BirthDate,
                ImageUrl = dto.ImageUrl,
                Status = DocumentStatus.Pending,
                CreatedAt = DateTime.Now,
                UserId = user.Id,
                User = user
            };
            await _citizenIdRepository.AddCitizenIdAsync(citizenId);
            return Result<CreateCitizenIdDTO>.Success(createCitizenIdDTO, "Tạo chứng minh nhân dân thành công.");
        }
        public async Task<Result<UpdateCitizenIdStatusDTO>> UpdateCitizenIdStatusAsync(UpdateCitizenIdStatusDTO updateCitizenIdStatusDTO)
        {
            var citizenId = await _citizenIdRepository.GetCitizenIdByIdAsync(updateCitizenIdStatusDTO.CitizenIdId);
            if (citizenId == null)
            {
                return Result<UpdateCitizenIdStatusDTO>.Failure("Chứng minh nhân dân không tồn tại! Kiểm tra lại Id.");
            }
            citizenId.Status = updateCitizenIdStatusDTO.Status;
            citizenId.UpdatedAt = DateTime.Now;
            await _citizenIdRepository.UpdateCitizenIdAsync(citizenId);
            return Result<UpdateCitizenIdStatusDTO>.Success(updateCitizenIdStatusDTO, "Cập nhật trạng thái chứng minh nhân dân thành công.");
        }
        public async Task<Result<UpdateCitizenIdInfoDTO>> UpdateCitizenIdInfoAsync(UpdateCitizenIdInfoDTO updateCitizenIdInfoDTO)
        {
            var citizenId = await _citizenIdRepository.GetCitizenIdByIdAsync(updateCitizenIdInfoDTO.Id);
            if (citizenId == null)
            {
                return Result<UpdateCitizenIdInfoDTO>.Failure("Chứng minh nhân dân không tồn tại! Kiểm tra lại Id.");
            }
            citizenId.Name = updateCitizenIdInfoDTO.Name;
            citizenId.CitizenIdNumber = updateCitizenIdInfoDTO.CitizenIdNumber;
            citizenId.BirthDate = updateCitizenIdInfoDTO.BirthDate;
            citizenId.ImageUrl = updateCitizenIdInfoDTO.ImageUrl;
            citizenId.UpdatedAt = DateTime.Now;
            await _citizenIdRepository.UpdateCitizenIdAsync(citizenId);
            return Result<UpdateCitizenIdInfoDTO>.Success(updateCitizenIdInfoDTO, "Cập nhật thông tin chứng minh nhân dân thành công.");
        }
    }
}
