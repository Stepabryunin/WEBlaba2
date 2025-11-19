using Microsoft.EntityFrameworkCore;
using WEBlaba2.Models;
using WEBlaba2.context;

namespace WEBlaba2.Services
{
    public class FeedbackService
    {
        private readonly ProductContext _context;

        public FeedbackService(ProductContext context)
        {
            _context = context;
        }

        // Получить все отзывы
        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync(); // ← УБЕРИ .Include(f => f.Client)
        }

        // Добавить новый отзыв
        public async Task<bool> AddFeedbackAsync(Feedback feedback)
        {
            try
            {
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding feedback: {ex.Message}");
                return false;
            }
        }
    }
}