using Repository.Entities;
using Repository.IRepositories;
using Repository.Repositories;
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
