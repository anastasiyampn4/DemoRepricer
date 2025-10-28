public class Sale
{
    private static int _idCounter = 1;
    public int SaleId { get; set; }
    public string ProductArticle { get; set; } // Ссылка на артикул товара
    public DateTime SaleDate { get; set; }
    public int Quantity { get; set; }

    public Sale()
    {
        SaleId = _idCounter++;
    }
}
