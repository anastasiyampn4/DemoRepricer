public class Competitor
{
    public string Name { get; set; }
    public string CompetitorArticle { get; set; }
    public decimal LastPrice { get; set; } // Последняя известная цена конкурента
    // ProductArticle можно добавить для связи с вашим товаром
}
