using System.Text;
using System.Security.Cryptography;
using WEBlaba2.context;
using WEBlaba2.models;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace WEBlaba2.Services
{
    public class ClientService
    {
        private readonly ProductContext _db;

        public ClientService(ProductContext context)
        {
            _db = context;

            // Добавьте эту строку для создания таблиц
            try
            {
                _db.Database.EnsureCreated();
                Console.WriteLine("Таблица Clients проверена/создана");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании таблицы Clients: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string firstname, string lastname, string middlename, string phone, string email, string login, string password)
        {
            try
            {
                // Проверяем существование пользователя
                if (await _db.Clients.AnyAsync(c => c.Email == email))
                {
                    return (false, "Пользователь с таким email уже существует");
                }
                if (await _db.Clients.AnyAsync(c => c.Login == login))
                {
                    return (false, "Пользователь с таким логином уже существует");
                }
                if (await _db.Clients.AnyAsync(c => c.Phone == phone))
                {
                    return (false, "Пользователь с таким телефоном уже существует");
                }

                // Создаем нового пользователя
                var client = new Client
                {
                    FirstName = firstname,
                    LastName = lastname,
                    MiddleName = middlename,
                    Phone = phone,
                    Email = email,
                    Login = login,
                    Password = HashPassword(password)
                };

                _db.Clients.Add(client);
                await _db.SaveChangesAsync();

                return (true, "Регистрация успешна");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при регистрации: {ex.Message}");
                return (false, $"Ошибка регистрации: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, Client Client)> LoginAsync(string login, string password)
        {
            try
            {
                // Ищем по логину ИЛИ email
                var client = await _db.Clients
                    .FirstOrDefaultAsync(c => c.Login == login || c.Email == login);

                if (client == null)
                {
                    return (false, "Пользователь не найден", null);
                }

                if (!VerifyPassword(password, client.Password))
                {
                    return (false, "Неверный пароль", null);
                }

                return (true, "Авторизация успешна", client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при авторизации: {ex.Message}");
                return (false, $"Ошибка авторизации: {ex.Message}", null);
            }
        }

        public List<Client> GetAllClients()
        {
            return _db.Clients.ToList();
        }

        public void AddClient(Client client)
        {
            _db.Clients.Add(client);
            _db.SaveChanges();
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }

        public Client GetClientById(int id)
        {
            return _db.Clients.FirstOrDefault(c => c.ClientId == id);
        }
    }
}