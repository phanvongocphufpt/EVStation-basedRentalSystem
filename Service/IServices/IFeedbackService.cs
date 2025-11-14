using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IFeedbackService
    {
        Task<Result<IEnumerable<FeedbackDTO>>> GetAllAsync();
        Task<Result<FeedbackDTO>> GetByIdAsync(int id);
        Task<Result<FeedbackDTO>> CreateAsync(CreateFeedbackDTO dto);
        Task<Result<FeedbackDTO>> UpdateAsync(UpdateFeedbackDTO dto);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
