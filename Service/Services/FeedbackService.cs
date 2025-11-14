using AutoMapper;
using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository feedbackRepository, IUserRepository userRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<FeedbackDTO>> CreateAsync(CreateFeedbackDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) return Result<FeedbackDTO>.Failure("Người dùng không tồn tại");

            var feedback = new Feedback
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = dto.UserId,
                RentalOrderId = dto.RentalOrderId,
                CreatedAt = DateTime.Now
            };

            await _feedbackRepository.AddAsync(feedback);

            var feedbackDto = new FeedbackDTO
            {
                Id = feedback.Id,
                Title = feedback.Title,
                Content = feedback.Content,
                CreatedAt = feedback.CreatedAt,
                UserFullName = user.FullName,
                RentalOrderId = feedback.RentalOrderId
            };

            return Result<FeedbackDTO>.Success(feedbackDto, "Tạo feedback thành công");
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null) return Result<bool>.Failure("Feedback không tồn tại");

            feedback.IsDeleted = true;
            await _feedbackRepository.UpdateAsync(feedback);

            return Result<bool>.Success(true, "Xóa feedback thành công");
        }

        public async Task<Result<IEnumerable<FeedbackDTO>>> GetAllAsync()
        {
            var feedbacks = (await _feedbackRepository.GetAllAsync())
                .Where(f => !f.IsDeleted)
                .ToList();

            var dtos = feedbacks.Select(f => new FeedbackDTO
            {
                Id = f.Id,
                Title = f.Title,
                Content = f.Content,
                CreatedAt = f.CreatedAt,
                UserFullName = f.User?.FullName ?? "",
                RentalOrderId = f.RentalOrderId
            });

            return Result<IEnumerable<FeedbackDTO>>.Success(dtos);
        }

        public async Task<Result<FeedbackDTO>> GetByIdAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null || feedback.IsDeleted)
                return Result<FeedbackDTO>.Failure("Feedback không tồn tại");

            var dto = new FeedbackDTO
            {
                Id = feedback.Id,
                Title = feedback.Title,
                Content = feedback.Content,
                CreatedAt = feedback.CreatedAt,
                UserFullName = feedback.User?.FullName ?? "",
                RentalOrderId = feedback.RentalOrderId
            };

            return Result<FeedbackDTO>.Success(dto);
        }

        public async Task<Result<FeedbackDTO>> UpdateAsync(UpdateFeedbackDTO dto)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(dto.Id);
            if (feedback == null || feedback.IsDeleted)
                return Result<FeedbackDTO>.Failure("Feedback không tồn tại");

            feedback.Title = dto.Title ?? feedback.Title;
            feedback.Content = dto.Content ?? feedback.Content;
            feedback.UpdatedAt = DateTime.Now;

            await _feedbackRepository.UpdateAsync(feedback);

            var feedbackDto = new FeedbackDTO
            {
                Id = feedback.Id,
                Title = feedback.Title,
                Content = feedback.Content,
                CreatedAt = feedback.CreatedAt,
                UserFullName = feedback.User?.FullName ?? "",
                RentalOrderId = feedback.RentalOrderId
            };

            return Result<FeedbackDTO>.Success(feedbackDto, "Cập nhật feedback thành công");
        }
    }
}
