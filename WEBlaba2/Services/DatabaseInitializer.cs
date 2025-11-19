using Microsoft.EntityFrameworkCore;
using WEBlaba2.context;
using WEBlaba2.Models;

namespace WEBlaba2.Services
{
    public class DatabaseInitializer
    {
        private readonly ProductContext _context;

        public DatabaseInitializer(ProductContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                Console.WriteLine("=== DATABASE INITIALIZER STARTED ===");

                // 1. Сначала создаем саму БД
                _context.Database.EnsureCreated();
                Console.WriteLine("Database ensured created");

                // 2. Создаем таблицы используя сырые SQL команды
                CreateTableIfNotExists("Products", @"
                    CREATE TABLE Products (
                        ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Price REAL NOT NULL,
                        Description TEXT,
                        ImageUrl TEXT,
                        Category TEXT,
                        PageUrl TEXT,
                        count INTEGER,
                        CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP
                    )");

                CreateTableIfNotExists("Clients", @"
                    CREATE TABLE Clients (
                        ClientId INTEGER PRIMARY KEY AUTOINCREMENT,
                        FirstName TEXT NOT NULL,
                        LastName TEXT NOT NULL,
                        MiddleName TEXT NOT NULL,
                        Phone TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        Login TEXT NOT NULL,
                        Password TEXT NOT NULL
                    )");

                CreateTableIfNotExists("CartItems", @"
                    CREATE TABLE CartItems (
                        CartItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProductId INTEGER NOT NULL,
                        ClientId INTEGER NOT NULL,
                        Quantity INTEGER NOT NULL
                    )");

                CreateTableIfNotExists("Feedbacks", @"
                    CREATE TABLE Feedbacks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        UserName TEXT NOT NULL,
                        Title TEXT NOT NULL,
                        Message TEXT NOT NULL,
                        Category TEXT,
                        Rating INTEGER NOT NULL,
                        CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP
                    )");

                // 3. Заполняем начальные данные
                SeedInitialData();

                Console.WriteLine("=== DATABASE INITIALIZATION COMPLETED ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void CreateTableIfNotExists(string tableName, string createSql)
        {
            try
            {
                // Используем сырой SQL запрос для проверки существования таблицы
                var connection = _context.Database.GetDbConnection();
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";

                var result = command.ExecuteScalar();
                var tableExists = result != null && result.ToString() == tableName;

                if (!tableExists)
                {
                    Console.WriteLine($"Creating table: {tableName}");
                    _context.Database.ExecuteSqlRaw(createSql);
                    Console.WriteLine($"Table {tableName} created successfully");
                }
                else
                {
                    Console.WriteLine($"Table {tableName} already exists - skipping");
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating table {tableName}: {ex.Message}");
            }
        }

        private void SeedInitialData()
        {
            try
            {
                // Проверяем существование таблицы Products и заполняем если пустая
                if (TableExists("Products") && IsTableEmpty("Products"))
                {
                    Console.WriteLine("Seeding products data...");
                    SeedProducts();
                }

                // Проверяем существование таблицы Clients и заполняем если пустая
                if (TableExists("Clients") && IsTableEmpty("Clients"))
                {
                    Console.WriteLine("Seeding clients data...");
                    SeedClients();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }

        private bool TableExists(string tableName)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";

                var result = command.ExecuteScalar();
                var exists = result != null && result.ToString() == tableName;

                connection.Close();
                return exists;
            }
            catch
            {
                return false;
            }
        }

        private bool IsTableEmpty(string tableName)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";

                var count = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();
                return count == 0;
            }
            catch
            {
                return true; // Если ошибка - считаем таблицу пустой
            }
        }

        private void SeedProducts()
        {
            try
            {
                // Используем сырой SQL для вставки всех 9 продуктов
                var sql = @"
            INSERT INTO Products (ProductId, Name, Description, Price, ImageUrl, Category, PageUrl, count)
            VALUES 
            (1, 'Issey Miyake Le Sel D''issey Eau de Toilette', 'Туалетная вода | 50 мл', 5790, 'imagesCatalog/Issey Miyake Le Sel D''issey Eau de Toilette.jpg', 'Парфюмерия', 'catalog/Issey_Miyake_Le_Sel', 123),
            (2, 'Banderas The Icon Woman Eau de Parfum', 'Парфюмерная вода | 50 мл', 3191, 'imagesCatalog/Banderas-The-Icon-Woman-Eau-de-Parfum.jpg', 'Парфюмерия', 'catalog/Banderas_The_Icon_Woman', 123),
            (3, 'Love Generation Blushberry Contouring Palette', 'Компактные румяна и скульптор для лица | 1 Коричневый, персиковый', 527, 'imagesCatalog/Love Generation Blushberry Contouring Palette (2).jpg', 'Косметика', 'catalog/Love_Generation_Blushberry_Contouring', 123),
            (4, 'Emi Ultra Strong Nail Polish Gel Effect', 'Ультрастойкий лак для ногтей с гелевым эффектом | 1 Снежно-белый', 499, 'imagesCatalog/Emi Ultra Strong Nail Polish Gel Effect.jpg', 'Косметика', 'catalog/Emi_Ultra_Strong_Nail', 123),
            (5, 'Caudalie Vinohydra Sorbet Cream Set Xmas', 'Набор для увлажняющего ухода за кожей лица', 2145, 'imagesCatalog/Caudalie Vinohydra Sorbet Cream Set Xmas.jpg', 'Уход', 'catalog/Caudalie_Vinohydra_Sorbet_Cream', 123),
            (6, 'Alan Hadash Brazilian Murumuru Hair Сonditioner', 'Кондиционер для окрашенных и жестких волос | 200 мл', 719, 'imagesCatalog/Alan Hadash Brazilian Murumuru Hair Сonditioner.jpg', 'Уход', 'catalog/Alan_Hadash_Brazilian_Murumuru', 123),
            (7, 'Caudalie Vinopure Salicylic Spot Solution', 'Точечный крем против воспалений с салициловой кислотой и ниацинамидом', 1312, 'imagesCatalog/Caudalie Vinopure Salicylic Spot Solution.jpg', 'Уход', 'catalog/Caudalie Vinopure Salicylic Spot Solution', 21),
            (8, 'Banderas Her Secret Pink Absolu Eau De Parfum', 'Парфюмерная вода | 50 мл', 2826, 'imagesCatalog/Banderas Her Secret Pink Absolu Eau De Parfum.jpg', 'Парфюмерия', 'catalog/Banderas Her Secret Pink Absolu Eau De Parfum', 31),
            (9, 'Vivienne Sabo Premiere Grande Artistic Volume Mascara', 'Двойная тушь для ресниц с эффектом сценического объема', 649, 'imagesCatalog/Vivienne Sabo Premiere Grande Artistic Volume Mascara.jpg', 'Косметика', 'catalog/Vivienne Sabo Premiere Grande Artistic Volume Mascara', 33)";

                _context.Database.ExecuteSqlRaw(sql);
                Console.WriteLine("All 9 products data seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding products: {ex.Message}");
            }
        }

        private void SeedClients()
        {
            try
            {
                var sql = @"
                    INSERT INTO Clients (FirstName, LastName, MiddleName, Phone, Email, Login, Password)
                    VALUES ('Админ', 'Админов', 'Админович', '+79990001122', 'admin@example.com', 'admin', 'admin123')";

                _context.Database.ExecuteSqlRaw(sql);
                Console.WriteLine("Clients data seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding clients: {ex.Message}");
            }
        }
    }
}