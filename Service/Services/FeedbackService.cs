using Repository.Entities;
using Repository.Repositories;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementations
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

        public async Task<Feedback> GetByCarName(string carName)
        {
            return await _feedbackRepository.GetByCarName(carName);
        }
        public async Task<IEnumerable<Feedback>> GetByUserIdAsync(int userId)
        {
            return await _feedbackRepository.GetByUserIdAsync(userId);
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
