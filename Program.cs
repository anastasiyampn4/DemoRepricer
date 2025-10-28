public class Product
{
    public string Article { get; set; } // Уникальный артикул WB
    public string Name { get; set; }
    public decimal CostPrice { get; set; } // Себестоимость
    public decimal Price { get; set; } // Текущая цена продажи
    public int Stock { get; set; } // Остаток на складе
    public decimal MinPrice { get; set; } // Минимальная цена (для репрайсинга)
    public decimal MaxPrice { get; set; } // Максимальная цена (для репрайсинга)

    // Метод для расчета рекомендуемой цены (заглушка, имитирующая анализ конкурентов)
    public decimal CalculateRecommendedPrice()
    {
        Random rnd = new Random();
        // Например, рекомендуем цену на 5-15% выше себестоимости, но в рамках коридора.
        decimal baseRecommendation = CostPrice * (1 + (decimal)(rnd.Next(5, 16) / 100.0));
        baseRecommendation = Math.Max(MinPrice, baseRecommendation);
        baseRecommendation = Math.Min(MaxPrice, baseRecommendation);
        return Math.Round(baseRecommendation, 2);
    }

    // Расчет рентабельности
    public decimal CalculateProfitability()
    {
        if (CostPrice == 0) return 0;
        return Math.Round((Price - CostPrice) / CostPrice * 100, 2);
    }
}
