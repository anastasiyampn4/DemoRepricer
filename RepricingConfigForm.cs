using System.Globalization;

public static class DataManager
{
    // Коллекции для хранения данных в памяти
    public static List<Product> Products { get; set; } = new List<Product>();
    public static List<Sale> Sales { get; set; } = new List<Sale>();
    public static List<Competitor> Competitors { get; set; } = new List<Competitor>();

    // Пути к файлам
    private static readonly string ProductsFile = "products.csv";
    private static readonly string SalesFile = "sales.csv";
    private static readonly string CompetitorsFile = "competitors.csv";

    // === СОХРАНЕНИЕ ДАННЫХ ===
    public static void SaveAllData()
    {
        SaveProducts();
        SaveSales();
        SaveCompetitors();
    }

    public static void SaveProducts()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(ProductsFile))
            {
                // Заголовок CSV
                writer.WriteLine("Article;Name;CostPrice;Price;Stock;MinPrice;MaxPrice");
                foreach (var product in Products)
                {
                    // Форматируем с учетом культуры (для корректного десятичного разделителя)
                    var line = $"{product.Article};{product.Name};{product.CostPrice.ToString(CultureInfo.InvariantCulture)};{product.Price.ToString(CultureInfo.InvariantCulture)};{product.Stock};{product.MinPrice.ToString(CultureInfo.InvariantCulture)};{product.MaxPrice.ToString(CultureInfo.InvariantCulture)}";
                    writer.WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void SaveSales()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(SalesFile))
            {
                writer.WriteLine("SaleId;ProductArticle;SaleDate;Quantity");
                foreach (var sale in Sales)
                {
                    var line = $"{sale.SaleId};{sale.ProductArticle};{sale.SaleDate:yyyy-MM-dd HH:mm:ss};{sale.Quantity}";
                    writer.WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении продаж: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void SaveCompetitors()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(CompetitorsFile))
            {
                writer.WriteLine("Name;CompetitorArticle;LastPrice");
                foreach (var comp in Competitors)
                {
                    var line = $"{comp.Name};{comp.CompetitorArticle};{comp.LastPrice.ToString(CultureInfo.InvariantCulture)}";
                    writer.WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении данных конкурентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // === ЗАГРУЗКА ДАННЫХ ===
    public static void LoadAllData()
    {
        LoadProducts();
        LoadSales();
        LoadCompetitors();
    }

    public static void LoadProducts()
    {
        Products.Clear();
        if (!File.Exists(ProductsFile)) return;

        try
        {
            var lines = File.ReadAllLines(ProductsFile);
            // Пропускаем заголовок
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length == 7)
                {
                    var product = new Product
                    {
                        Article = parts[0],
                        Name = parts[1],
                        CostPrice = decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                        Price = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                        Stock = int.Parse(parts[4]),
                        MinPrice = decimal.Parse(parts[5], CultureInfo.InvariantCulture),
                        MaxPrice = decimal.Parse(parts[6], CultureInfo.InvariantCulture)
                    };
                    Products.Add(product);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void LoadSales()
    {
        Sales.Clear();
        if (!File.Exists(SalesFile)) return;

        try
        {
            var lines = File.ReadAllLines(SalesFile);
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length == 4)
                {
                    var sale = new Sale
                    {
                        SaleId = int.Parse(parts[0]),
                        ProductArticle = parts[1],
                        SaleDate = DateTime.ParseExact(parts[2], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        Quantity = int.Parse(parts[3])
                    };
                    // Обновляем счетчик ID, чтобы новые продажи имели уникальный ID
                    Sale._idCounter = Math.Max(Sale._idCounter, sale.SaleId + 1);
                    Sales.Add(sale);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке продаж: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void LoadCompetitors()
    {
        Competitors.Clear();
        if (!File.Exists(CompetitorsFile)) return;

        try
        {
            var lines = File.ReadAllLines(CompetitorsFile);
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length == 3)
                {
                    var comp = new Competitor
                    {
                        Name = parts[0],
                        CompetitorArticle = parts[1],
                        LastPrice = decimal.Parse(parts[2], CultureInfo.InvariantCulture)
                    };
                    Competitors.Add(comp);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке данных конкурентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
