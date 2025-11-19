using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.Models;
using WEBlaba2.Services;
using System.ComponentModel.DataAnnotations;
using WEBlaba2.models;

namespace WEBlaba2.Pages.feedback
{
    public class FeedbackModel : AuthPageModel
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }

        private readonly ClientService _authService;
        private readonly FeedbackService _feedbackService;

        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        [BindProperty]
        public FeedbackInputModel FeedbackInput { get; set; }

        public FeedbackModel(SessionService sessionService, ClientService authService, FeedbackService feedbackService) : base(sessionService)
        {
            _authService = authService;
            _feedbackService = feedbackService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            IsAuthenticated = _sessionService.IsAuthenticated();

            if (IsAuthenticated)
            {
                UserName = _sessionService.GetCurrentUserName();
                UserId = _sessionService.GetCurrentUserId() ?? 0;
            }

            // Загружаем все отзывы для просмотра
            Feedbacks = await _feedbackService.GetAllFeedbacksAsync();

            return Page();
        }

        // Обработка отправки отзыва
        public async Task<IActionResult> OnPostAddFeedbackAsync()
        {
            Console.WriteLine("=== DEBUG: OnPostAddFeedbackAsync STARTED ===");

            // 1. Проверяем аутентификацию
            IsAuthenticated = _sessionService.IsAuthenticated();
            Console.WriteLine($"DEBUG: IsAuthenticated = {IsAuthenticated}");

            if (IsAuthenticated)
            {
                UserName = _sessionService.GetCurrentUserName();
                UserId = _sessionService.GetCurrentUserId() ?? 0;
                Console.WriteLine($"DEBUG: UserName = {UserName}, UserId = {UserId}");
            }

            if (!IsAuthenticated)
            {
                Console.WriteLine("DEBUG: User not authenticated, redirecting to login");
                return RedirectToPage("/Auth/Authorization");
            }

            // 2. Проверяем, пришли ли данные модели
            Console.WriteLine($"DEBUG: FeedbackInput is null = {FeedbackInput == null}");

            if (FeedbackInput != null)
            {
                Console.WriteLine($"DEBUG: Title = '{FeedbackInput.Title}'");
                Console.WriteLine($"DEBUG: Message = '{FeedbackInput.Message}'");
                Console.WriteLine($"DEBUG: Category = '{FeedbackInput.Category}'");
                Console.WriteLine($"DEBUG: Rating = {FeedbackInput.Rating}");
            }
            else
            {
                Console.WriteLine("DEBUG: FeedbackInput is NULL - проблема с биндингом модели");
            }

            // 3. Проверяем валидацию модели
            Console.WriteLine($"DEBUG: ModelState.IsValid = {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("DEBUG: Model validation FAILED. Errors:");

                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    var fieldValue = key.Contains("FeedbackInput") ?
                        $"Field: {key}" :
                        $"Field: {key}, Value: {GetFieldValue(key)}";

                    Console.WriteLine($"  {fieldValue}");

                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"    - Error: {error.ErrorMessage}");
                        if (!string.IsNullOrEmpty(error.Exception?.Message))
                        {
                            Console.WriteLine($"      Exception: {error.Exception.Message}");
                        }
                    }
                }

                // Перезагружаем отзывы
                Feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                Console.WriteLine("DEBUG: Returning Page() with validation errors");
                return Page();
            }

            Console.WriteLine("DEBUG: Model validation PASSED");

            try
            {
                // 4. Создаем отзыв
                var feedback = new Feedback
                {
                    UserId = UserId,
                    UserName = UserName,
                    Title = FeedbackInput.Title?.Trim(),
                    Message = FeedbackInput.Message?.Trim(),
                    Category = FeedbackInput.Category,
                    Rating = FeedbackInput.Rating,
                    CreatedDate = DateTime.UtcNow
                };

                Console.WriteLine("DEBUG: Attempting to save feedback to database...");

                var result = await _feedbackService.AddFeedbackAsync(feedback);
                Console.WriteLine($"DEBUG: Save result = {result}");

                if (result)
                {
                    Console.WriteLine("DEBUG: Feedback saved successfully, setting success message");
                    TempData["SuccessMessage"] = "Ваш отзыв успешно добавлен!";
                    return RedirectToPage();
                }
                else
                {
                    Console.WriteLine("DEBUG: Failed to save feedback to database");
                    ModelState.AddModelError("", "Произошла ошибка при сохранении отзыва.");
                    Feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: EXCEPTION in AddFeedback: {ex.Message}");
                Console.WriteLine($"DEBUG: StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "Произошла непредвиденная ошибка.");
                Feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                return Page();
            }
            finally
            {
                Console.WriteLine("=== DEBUG: OnPostAddFeedbackAsync COMPLETED ===");
            }
        }

        // Вспомогательный метод для получения значений полей
        private string GetFieldValue(string key)
        {
            try
            {
                if (key == "FeedbackInput.Title") return FeedbackInput?.Title ?? "null";
                if (key == "FeedbackInput.Message") return FeedbackInput?.Message ?? "null";
                if (key == "FeedbackInput.Category") return FeedbackInput?.Category ?? "null";
                if (key == "FeedbackInput.Rating") return FeedbackInput?.Rating.ToString() ?? "null";
                return "unknown field";
            }
            catch
            {
                return "error getting value";
            }
        }

        

    }

    // Модель для ввода отзыва
    public class FeedbackInputModel
    {
        [Required(ErrorMessage = "Введите заголовок отзыва")]
        [StringLength(100, ErrorMessage = "Заголовок не должен превышать 100 символов")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Введите текст отзыва")]
        [StringLength(1000, ErrorMessage = "Отзыв не должен превышать 1000 символов")]
        public string Message { get; set; }

        public string Category { get; set; }

        [Required(ErrorMessage = "Выберите оценку")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Rating { get; set; }
    }
}