using Repository.Entities;
using Repository.IRepositories;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            public Task<IEnumerable<Feedback>> GetAllAsync()
            {
                return _feedbackRepository.GetAllAsync();
            }

            public Task<Feedback> GetByIdAsync(int id)
            {
                return _feedbackRepository.GetByIdAsync(id);
            }

            public Task AddAsync(Feedback feedback)
            {
                return _feedbackRepository.AddAsync(feedback);
            }

            public Task UpdateAsync(Feedback feedback)
            {
                return _feedbackRepository.UpdateAsync(feedback);
            }

            public Task DeleteAsync(int id)
            {
                return _feedbackRepository.DeleteAsync(id);
            }

            public Task<bool> ExistsAsync(int id)
            {
                return _feedbackRepository.ExistsAsync(id);
            }
        }
    }

