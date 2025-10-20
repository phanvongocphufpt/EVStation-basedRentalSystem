using Repository.Entities;
using Repository.IRepositories;
using Repository.Repositories;
using Service.Common.Service.Common;
using Service.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _feedbackRepository.GetAllAsync();
        }
        // 🔹 Lấy tất cả feedback (phân trang + tìm kiếm)
        public async Task<Pagination<Feedback>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null)
        {
            var allFeedbacks = await _feedbackRepository.GetAllAsync();

            var filtered = allFeedbacks.Where(f => !f.IsDeleted);

            // Tìm kiếm theo keyword (nếu có)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filtered = filtered.Where(f =>
                    (f.Content != null && f.Content.Contains(keyword)) ||
                    (f.Title != null && f.Title.Contains(keyword)) 
                );
            }

            // Tổng số phần tử
            var totalCount = filtered.Count();

            // Lấy phần dữ liệu trang hiện tại
            var items = filtered
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            return new Pagination<Feedback>
            {
                TotalItemsCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = items
            };
        }

        public async Task<Feedback> GetByCarName(string carName)
        {
            return await _feedbackRepository.GetByCarName(carName);
        }

        public async Task AddAsync(Feedback feedback)
        {
            await _feedbackRepository.AddAsync(feedback);
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task DeleteAsync(int id)
        {
            await _feedbackRepository.DeleteAsync(id);
        }

        
    }
}
