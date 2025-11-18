using Microsoft.EntityFrameworkCore;
using WEBlaba2.context;
using WEBlaba2.models;

namespace WEBlaba2.Services
{
    public class ProductService
    {
        private readonly ProductContext _db;

        public ProductService(ProductContext context)
        {
            _db = context;

     
            EnsureProductsTableCreated();

            if (!_db.Products.Any())
            {
                InitializeData();
            }
        }

        private void EnsureProductsTableCreated()
        {
            try
            {
                var test = _db.Products.FirstOrDefault();
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 1)
            {
                // Создаём таблицу с правильным именем столбца
                _db.Database.ExecuteSqlRaw(@"
            CREATE TABLE Products (
                ProductId INTEGER PRIMARY KEY AUTOINCREMENT, -- ← ИЗМЕНИТЬ Id на ProductId
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Description TEXT,
                ImageUrl TEXT,
                Category TEXT,
                PageUrl TEXT,
                count INTEGER,
                CreatedDate TEXT DEFAULT (datetime('now'))
            )");
            }
        }

        private void InitializeData()
        {
            try
            {
                _db.Products.AddRange(
                    new Product
                    {
                        ProductId = 1,
                        Name = "Issey Miyake Le Sel D'issey Eau de Toilette",
                        Description = "Туалетная вода | 50 мл",
                        Price = 5790,
                        ImageUrl = "imagesCatalog/Issey Miyake Le Sel D'issey Eau de Toilette.jpg",
                        Category = "Парфюмерия",
                        PageUrl = "catalog/Issey_Miyake_Le_Sel",
                        count = 123
                    },
                    new Product
                    {
                        ProductId = 2,
                        Name = "Banderas The Icon Woman Eau de Parfum",
                        Description = "Парфюмерная вода | 50 мл",
                        Price = 3191,
                        ImageUrl = "imagesCatalog/Banderas-The-Icon-Woman-Eau-de-Parfum.jpg",
                        Category = "Парфюмерия",
                        PageUrl = "catalog/Banderas_The_Icon_Woman",
                        count = 123
                    },
                    new Product
                    {
                        ProductId = 3,
                        Name = "Love Generation Blushberry Contouring Palette",
                        Description = "Компактные румяна и скульптор для лица | 1 Коричневый, персиковый",
                        Price = 527,
                        ImageUrl = "imagesCatalog/Love Generation Blushberry Contouring Palette (2).jpg",
                        Category = "Косметика",
                        PageUrl = "catalog/Love_Generation_Blushberry_Contouring",
                        count = 123
                    },
                    new Product
                    {
                        ProductId = 4,
                        Name = "Emi Ultra Strong Nail Polish Gel Effect",
                        Description = "Ультрастойкий лак для ногтей с гелевым эффектом | 1 Снежно-белый",
                        Price = 499,
                        ImageUrl = "imagesCatalog/Emi Ultra Strong Nail Polish Gel Effect.jpg",
                        Category = "Косметика",
                        PageUrl = "catalog/Emi_Ultra_Strong_Nail",
                        count = 123
                    },
                    new Product
                    {
                        ProductId = 5,
                        Name = "Caudalie Vinohydra Sorbet Cream Set Xmas",
                        Description = "Набор для увлажняющего ухода за кожей лица",
                        Price = 2145,
                        ImageUrl = "imagesCatalog/Caudalie Vinohydra Sorbet Cream Set Xmas.jpg",
                        Category = "Уход",
                        PageUrl = "catalog/Caudalie_Vinohydra_Sorbet_Cream",
                        count = 123
                    },
                    new Product
                    {
                        ProductId = 6,
                        Name = "Alan Hadash Brazilian Murumuru Hair Сonditioner",
                        Description = "Кондиционер для окрашенных и жестких волос | 200 мл",
                        Price = 719,
                        ImageUrl = "imagesCatalog/Alan Hadash Brazilian Murumuru Hair Сonditioner.jpg",
                        Category = "Уход",
                        PageUrl = "catalog/Alan_Hadash_Brazilian_Murumuru",
                        count = 123
                    },
                    new Product
                    {
                        ProductId = 7,
                        Name = "Caudalie Vinopure Salicylic Spot Solution",
                        Description = "Точечный крем против воспалений с салициловой кислотой и ниацинамидом",
                        Price = 1312,
                        ImageUrl = "imagesCatalog/Caudalie Vinopure Salicylic Spot Solution.jpg",
                        Category = "Уход",
                        PageUrl = "catalog/Caudalie Vinopure Salicylic Spot Solution",
                        count = 21
                    },
                    new Product
                    {
                        ProductId = 8,
                        Name = "Banderas Her Secret Pink Absolu Eau De Parfum",
                        Description = "Парфюмерная вода | 50 мл",
                        Price = 2826,
                        ImageUrl = "imagesCatalog/Banderas Her Secret Pink Absolu Eau De Parfum.jpg",
                        Category = "Парфюмерия",
                        PageUrl = "catalog/Banderas Her Secret Pink Absolu Eau De Parfum",
                        count = 31
                    },
                    new Product
                    {
                        ProductId = 9,
                        Name = "Vivienne Sabo Premiere Grande Artistic Volume Mascara",
                        Description = "Двойная тушь для ресниц с эффектом сценического объема",
                        Price = 649,
                        ImageUrl = "imagesCatalog/Vivienne Sabo Premiere Grande Artistic Volume Mascara.jpg",
                        Category = "Косметика",
                        PageUrl = "catalog/Vivienne Sabo Premiere Grande Artistic Volume Mascara",
                        count = 33
                    }


                );

                _db.SaveChanges();
                Console.WriteLine("База данных создана и заполнена данными");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации данных: {ex.Message}");
            }
        }

        public async Task<List<Product>> SearchByNameAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<Product>();

            return await _db.Products
                .Where(p => p.Name.ToLower().Contains(productName.ToLower()))
                .OrderBy(p => p.Name)
                .Take(10)
                .ToListAsync();
        }

        public async Task<Product?> SearcOneByNameAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return null;

            // Используем поиск без учета регистра
            return await _db.Products.FirstOrDefaultAsync(product =>
                product.Name.ToLower() == productName.ToLower());
        }
        public List<Product> GetAllProducts()
        {
            return _db.Products.ToList();
        }
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _db.Products.FirstOrDefaultAsync(product => product.ProductId == id);
        }
    }

}
