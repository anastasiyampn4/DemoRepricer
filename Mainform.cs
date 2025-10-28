using System.ComponentModel;
using System.Linq;

public partial class MainForm : Form
{
    private BindingList<Product> _bindingProducts;

    public MainForm()
    {
        InitializeComponent();
        Load += MainForm_Load;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        // Загружаем данные из файлов при запуске
        DataManager.LoadAllData();
        SetupDataGridView();
        RefreshDashboard();
    }

    private void SetupDataGridView()
    {
        // Привязываем список товаров к DataGridView
        _bindingProducts = new BindingList<Product>(DataManager.Products);
        dataGridViewProducts.DataSource = _bindingProducts;

        // Настраиваем столбцы (можно сделать в дизайнере)
        dataGridViewProducts.AutoGenerateColumns = false;
        dataGridViewProducts.Columns.Clear();

        dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Article", HeaderText = "Артикул" });
        dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Наименование" });
        dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CostPrice", HeaderText = "Себест-ть" });
        dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Цена" });
        dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Stock", HeaderText = "Остаток" });
        dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CalculateProfitability", HeaderText = "Рентаб-ть (%)" });
    }

    // === ОБРАБОТЧИКИ СОБЫТИЙ ДЛЯ КНОПОК ===

    // Добавление нового товара
    private void buttonAddProduct_Click(object sender, EventArgs e)
    {
        // Простое добавление через диалог. Можно сделать отдельную форму.
        string article = Microsoft.VisualBasic.Interaction.InputBox("Введите артикул:", "Новый товар");
        if (!string.IsNullOrWhiteSpace(article))
        {
            // Проверяем, нет ли уже такого артикула
            if (DataManager.Products.Any(p => p.Article == article))
            {
                MessageBox.Show("Товар с таким артикулом уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = Microsoft.VisualBasic.Interaction.InputBox("Введите наименование:", "Новый товар");
            if (!string.IsNullOrWhiteSpace(name))
            {
                var newProduct = new Product
                {
                    Article = article,
                    Name = name,
                    CostPrice = 500, // Значения по умолчанию
                    Price = 1000,
                    Stock = 10,
                    MinPrice = 800,
                    MaxPrice = 1200
                };
                _bindingProducts.Add(newProduct);
                DataManager.SaveProducts(); // Сохраняем изменения
                RefreshDashboard();
            }
        }
    }

    // Имитация продажи (для тестирования аналитики)
    private void buttonSimulateSale_Click(object sender, EventArgs e)
    {
        if (dataGridViewProducts.CurrentRow?.DataBoundItem is Product selectedProduct)
        {
            if (selectedProduct.Stock > 0)
            {
                var sale = new Sale
                {
                    ProductArticle = selectedProduct.Article,
                    SaleDate = DateTime.Now,
                    Quantity = 1
                };
                DataManager.Sales.Add(sale);
                selectedProduct.Stock--; // Уменьшаем остаток
                _bindingProducts.ResetBindings(); // Обновляем DataGridView
                DataManager.SaveSales();
                DataManager.SaveProducts();
                RefreshDashboard();
            }
            else
            {
                MessageBox.Show("Товара нет в наличии!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        else
        {
            MessageBox.Show("Выберите товар для продажи.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Запуск "мониторинга цен" (имитация)
    private void buttonRunPriceMonitor_Click(object sender, EventArgs e)
    {
        listBoxLog.Items.Clear();
        listBoxLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] Запуск мониторинга цен...");

        // Имитируем анализ и даем рекомендации для каждого товара
        foreach (var product in DataManager.Products)
        {
            decimal recommendedPrice = product.CalculateRecommendedPrice();
            listBoxLog.Items.Add($"Товар {product.Name}: Тек. цена = {product.Price}р. | Реком. цена = {recommendedPrice}р.");

            // Пример логики: если текущая цена сильно отличается от рекомендованной, предлагаем изменить
            decimal difference = Math.Abs(product.Price - recommendedPrice);
            if (difference > 50) // Порог в 50 рублей
            {
                listBoxLog.Items.Add($"--> ВНИМАНИЕ: Рекомендуется изменить цену на {product.Name}!");
            }
        }
        listBoxLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] Мониторинг завершен.");
        listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1; // Прокрутка к последнему элементу
    }

    // Аналитика с использованием LINQ (Раздел 3.2.2)
    private void buttonShowAnalytics_Click(object sender, EventArgs e)
    {
        // 1. Самый продаваемый товар
        var topSellingProduct = DataManager.Sales
            .GroupBy(s => s.ProductArticle)
            .Select(g => new { Article = g.Key, TotalSold = g.Sum(s => s.Quantity) })
            .OrderByDescending(x => x.TotalSold)
            .FirstOrDefault();

        // 2. Общая выручка
        decimal totalRevenue = (from sale in DataManager.Sales
                                join product in DataManager.Products on sale.ProductArticle equals product.Article
                                select sale.Quantity * product.Price).Sum();

        // 3. Товары с низкой рентабельностью (< 10%)
        var lowProfitProducts = DataManager.Products.Where(p => p.CalculateProfitability() < 10).ToList();

        // 4. Товары, которых мало на складе (< 5 шт)
        var lowStockProducts = DataManager.Products.Where(p => p.Stock < 5).ToList();

        // Формируем отчет
        string report = "--- ОТЧЕТ ПО АНАЛИТИКЕ ---\n";
        report += $"Общая выручка: {totalRevenue}р.\n";
        report += topSellingProduct != null ? $"Самый продаваемый товар (арт. {topSellingProduct.Article}): {topSellingProduct.TotalSold} шт.\n" : "Продаж нет.\n";
        report += $"Товары с низкой рентабельностью (<10%): {lowProfitProducts.Count} шт.\n";
        report += $"Товары с низким остатком (<5 шт.): {lowStockProducts.Count} шт.\n";

        report += "\nСПИСОК ТОВАРОВ С НИЗКИМ ОСТАТКОМ:\n";
        foreach (var product in lowStockProducts)
        {
            report += $"- {product.Name} (Остаток: {product.Stock} шт.)\n";
        }

        MessageBox.Show(report, "Результаты аналитики", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Обновление дашборда (ключевые метрики)
    private void RefreshDashboard()
    {
        labelTotalProducts.Text = DataManager.Products.Count.ToString();
        labelTotalSales.Text = DataManager.Sales.Sum(s => s.Quantity).ToString();
        labelOutOfStock.Text = DataManager.Products.Count(p => p.Stock == 0).ToString();

        // Средняя рентабельность
        decimal avgProfitability = DataManager.Products.Count > 0 ?
            DataManager.Products.Average(p => p.CalculateProfitability()) : 0;
        labelAvgProfitability.Text = $"{avgProfitability:F2}%";
    }

    // Сохранение всех данных при закрытии формы
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        DataManager.SaveAllData();
    }
}
