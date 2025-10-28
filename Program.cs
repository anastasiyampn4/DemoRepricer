using System;
using System.Linq;
using WildberriesRepricer.Data;
using WildberriesRepricer.Models;
using WildberriesRepricer.Services;

namespace WildberriesRepricer
{
    class Program
    {
        static DatabaseContext context = null!;
        static ProductRepository productRepo = null!;
        static CompetitorRepository competitorRepo = null!;
        static RepricingService repricingService = null!;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            context = new DatabaseContext();
            context.InitializeDatabase();

            var sampleDataService = new SampleDataService(context);
            sampleDataService.InitializeSampleData();

            productRepo = new ProductRepository(context);
            competitorRepo = new CompetitorRepository(context);
            repricingService = new RepricingService(context);

            ShowWelcome();
            RunMainMenu();

            context.Dispose();
        }

        static void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   WILDBERRIES РЕПРАЙСЕР - СИСТЕМА УПРАВЛЕНИЯ ЦЕНАМИ        ║");
            Console.WriteLine("║          Интернет-магазин кофе на маркетплейсе              ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");
        }

        static void RunMainMenu()
        {
            bool exit = false;
            
            while (!exit)
            {
                Console.WriteLine("\n═══════════════ ГЛАВНОЕ МЕНЮ ═══════════════════");
                Console.WriteLine("1. Показать каталог товаров");
                Console.WriteLine("2. Анализ конкурентов");
                Console.WriteLine("3. Применить стратегию: Установить минимальную цену");
                Console.WriteLine("4. Применить стратегию: Понизить на 5%");
                Console.WriteLine("5. Применить стратегию: Понизить на сумму");
                Console.WriteLine("6. Применить стратегию: Поддержать маржу 25%");
                Console.WriteLine("7. Применить стратегию: Премиум цена +10%");
                Console.WriteLine("8. Показать аналитику цен");
                Console.WriteLine("9. История изменения цен");
                Console.WriteLine("0. Выход");
                Console.WriteLine("═════════════════════════════════════════════════");
                Console.Write("\nВыберите действие (0-9): ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayProductCatalog();
                        break;
                    case "2":
                        DisplayCompetitorAnalysis();
                        break;
                    case "3":
                        ApplyMatchLowestStrategy();
                        break;
                    case "4":
                        ApplyUndercutByPercentStrategy();
                        break;
                    case "5":
                        ApplyUndercutByAmountStrategy();
                        break;
                    case "6":
                        ApplyMaintainMarginStrategy();
                        break;
                    case "7":
                        ApplyPremiumPricingStrategy();
                        break;
                    case "8":
                        DisplayPricingAnalytics();
                        break;
                    case "9":
                        DisplayPriceHistory();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("\n✅ Спасибо за использование системы! До свидания!");
                        break;
                    default:
                        Console.WriteLine("\n❌ Неверный выбор. Попробуйте снова.");
                        break;
                }

                if (!exit)
                {
                    Console.Write("\nНажмите Enter для продолжения...");
                    Console.ReadLine();
                }
            }
        }

        static void DisplayProductCatalog()
        {
            Console.WriteLine("\n📦 КАТАЛОГ ТОВАРОВ\n");
            
            var products = productRepo.GetAll();
            Console.WriteLine($"{"ID",-5} {"SKU",-18} {"Наименование",-35} {"Цена",-12} {"Маржа",-10} {"Остаток"}");
            Console.WriteLine(new string('-', 95));

            foreach (var product in products)
            {
                var margin = product.CalculateMargin();
                Console.WriteLine($"{product.Id,-5} {product.SKU,-18} {TruncateString(product.Name, 35),-35} " +
                                $"{product.CurrentPrice,10:N2}₽ {margin,8:N1}% {product.StockQuantity,8}");
            }
        }

        static void DisplayCompetitorAnalysis()
        {
            Console.WriteLine("\n🏪 АНАЛИЗ КОНКУРЕНТОВ\n");
            var products = productRepo.GetAll();
            
            foreach (var product in products)
            {
                var competitors = competitorRepo.GetByProductId(product.Id);
                if (!competitors.Any()) continue;
                
                var lowestPrice = competitors.Min(c => c.Price);
                var avgPrice = competitors.Average(c => c.Price);
                
                Console.WriteLine($"\n{product.Name}");
                Console.WriteLine($"  Наша цена: {product.CurrentPrice:N2}₽");
                Console.WriteLine($"  Мин. цена конкурентов: {lowestPrice:N2}₽");
                Console.WriteLine($"  Средняя цена конкурентов: {avgPrice:N2}₽");
                Console.WriteLine($"  Разница с мин.: {(product.CurrentPrice - lowestPrice):N2}₽ ({((product.CurrentPrice - lowestPrice) / lowestPrice * 100):N1}%)");
                Console.WriteLine($"  Конкурентов: {competitors.Count}");
            }
        }

        static void ApplyMatchLowestStrategy()
        {
            Console.WriteLine("\n🎯 СТРАТЕГИЯ: Установить цену равной минимальной у конкурентов\n");
            
            var config = new RepricingConfig
            {
                Strategy = RepricingStrategy.MatchLowest
            };

            var results = repricingService.ExecuteBulkRepricing(config);
            DisplayBulkResults(results);
        }

        static void ApplyUndercutByPercentStrategy()
        {
            Console.WriteLine("\n📉 СТРАТЕГИЯ: Понизить цену на 5% от минимальной конкурента\n");
            
            var config = new RepricingConfig
            {
                Strategy = RepricingStrategy.UndercutByPercent,
                UndercutPercent = 5.0m
            };

            var results = repricingService.ExecuteBulkRepricing(config);
            DisplayBulkResults(results);
        }

        static void ApplyUndercutByAmountStrategy()
        {
            Console.WriteLine("\n💵 СТРАТЕГИЯ: Понизить цену на 50₽ от минимальной конкурента\n");
            
            var config = new RepricingConfig
            {
                Strategy = RepricingStrategy.UndercutByAmount,
                UndercutAmount = 50.0m
            };

            var results = repricingService.ExecuteBulkRepricing(config);
            DisplayBulkResults(results);
        }

        static void ApplyMaintainMarginStrategy()
        {
            Console.WriteLine("\n💰 СТРАТЕГИЯ: Поддержание минимальной маржи 25%\n");
            
            var config = new RepricingConfig
            {
                Strategy = RepricingStrategy.MaintainMargin,
                MinMarginPercent = 25.0m
            };

            var results = repricingService.ExecuteBulkRepricing(config);
            DisplayBulkResults(results);
        }

        static void ApplyPremiumPricingStrategy()
        {
            Console.WriteLine("\n✨ СТРАТЕГИЯ: Премиум цена +10% от минимальной конкурента\n");
            
            var config = new RepricingConfig
            {
                Strategy = RepricingStrategy.PremiumPricing,
                PremiumPercent = 10.0m
            };

            var results = repricingService.ExecuteBulkRepricing(config);
            DisplayBulkResults(results);
        }

        static void DisplayBulkResults(System.Collections.Generic.List<RepricingResult> results)
        {
            int changed = 0;
            int unchanged = 0;

            foreach (var result in results)
            {
                Console.WriteLine($"Товар: {result.ProductName}");
                Console.WriteLine($"  Старая цена: {result.OldPrice:N2}₽");
                Console.WriteLine($"  Новая цена:  {result.NewPrice:N2}₽");
                
                if (result.PriceChanged)
                {
                    Console.WriteLine($"  Изменение:   {result.GetPriceChange():N2}₽ ({result.GetPriceChangePercent():N1}%)");
                    Console.WriteLine($"  Статус: ✓ Цена обновлена");
                    changed++;
                }
                else
                {
                    Console.WriteLine($"  Статус: ○ Без изменений");
                    unchanged++;
                }
                
                Console.WriteLine($"  {result.Message}\n");
            }

            Console.WriteLine($"\n📊 Итого: Обновлено {changed}, Без изменений {unchanged}");
        }

        static void DisplayPricingAnalytics()
        {
            Console.WriteLine("\n" + new string('═', 70));
            Console.WriteLine("📊 АНАЛИТИКА ЦЕН");
            Console.WriteLine(new string('═', 70) + "\n");

            var products = productRepo.GetAll();
            
            var totalRevenue = products.Sum(p => p.CurrentPrice * p.StockQuantity);
            var totalCost = products.Sum(p => p.CostPrice * p.StockQuantity);
            var totalProfit = totalRevenue - totalCost;
            var avgMargin = products.Average(p => p.CalculateMargin());

            Console.WriteLine($"Всего товаров:         {products.Count}");
            Console.WriteLine($"Общая стоимость:       {totalRevenue:N2}₽");
            Console.WriteLine($"Себестоимость:         {totalCost:N2}₽");
            Console.WriteLine($"Потенциальная прибыль: {totalProfit:N2}₽");
            Console.WriteLine($"Средняя маржа:         {avgMargin:N1}%");
            
            Console.WriteLine($"\nДиапазон цен:          {products.Min(p => p.CurrentPrice):N2}₽ - {products.Max(p => p.CurrentPrice):N2}₽");
            Console.WriteLine($"Общий остаток:         {products.Sum(p => p.StockQuantity)} единиц");
        }

        static void DisplayPriceHistory()
        {
            Console.WriteLine("\n📜 ИСТОРИЯ ИЗМЕНЕНИЯ ЦЕН\n");
            
            var historyRepo = new PriceHistoryRepository(context);
            var history = historyRepo.GetAll().OrderByDescending(h => h.ChangeDate).Take(20).ToList();

            if (!history.Any())
            {
                Console.WriteLine("История изменений пуста.");
                return;
            }

            Console.WriteLine($"{"Дата/Время",-20} {"Товар",-30} {"Старая цена",-12} {"Новая цена",-12} {"Стратегия"}");
            Console.WriteLine(new string('-', 100));

            foreach (var entry in history)
            {
                var product = productRepo.GetById(entry.ProductId);
                var productName = product != null ? (TruncateString(product.Name, 30) ?? "Неизвестно") : "Неизвестно";
                
                Console.WriteLine($"{entry.ChangeDate:yyyy-MM-dd HH:mm:ss}  {productName,-30} " +
                                $"{entry.OldPrice,10:N2}₽  {entry.NewPrice,10:N2}₽  {entry.Strategy}");
            }

            Console.WriteLine($"\nОтображено последних {history.Count} записей.");
        }

        static string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.Length <= maxLength ? str : str.Substring(0, maxLength - 3) + "...";
        }
    }
}
