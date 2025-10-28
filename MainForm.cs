using System;
using System.Windows.Forms;
using WildberriesRepricer.Data;
using WildberriesRepricer.Services;

namespace WildberriesRepricer.Forms
{
    public partial class MainForm : Form
    {
        private DatabaseContext _context;
        private ProductRepository _productRepo;
        private RepricingService _repricingService;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            _context = new DatabaseContext();
            _context.InitializeDatabase();
            _productRepo = new ProductRepository(_context);
            _repricingService = new RepricingService(_context);
        }

        private void InitializeComponent()
        {
            this.Text = "Wildberries Репрайсер - Управление Ценами";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            var menuStrip = new MenuStrip();
            
            var productsMenu = new ToolStripMenuItem("Товары");
            productsMenu.DropDownItems.Add("Список товаров", null, OnProductsListClick);
            productsMenu.DropDownItems.Add("Добавить товар", null, OnAddProductClick);
            
            var repricingMenu = new ToolStripMenuItem("Репрайсинг");
            repricingMenu.DropDownItems.Add("Запустить репрайсинг", null, OnRunRepricingClick);
            repricingMenu.DropDownItems.Add("Настройки стратегии", null, OnRepricingSettingsClick);
            
            var analyticsMenu = new ToolStripMenuItem("Аналитика");
            analyticsMenu.DropDownItems.Add("История цен", null, OnPriceHistoryClick);
            analyticsMenu.DropDownItems.Add("Отчеты о продажах", null, OnSalesReportsClick);
            
            menuStrip.Items.Add(productsMenu);
            menuStrip.Items.Add(repricingMenu);
            menuStrip.Items.Add(analyticsMenu);
            
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            var statusStrip = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("Готов к работе");
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        private void OnProductsListClick(object sender, EventArgs e)
        {
            MessageBox.Show("Открытие списка товаров...", "Информация");
        }

        private void OnAddProductClick(object sender, EventArgs e)
        {
            MessageBox.Show("Добавление нового товара...", "Информация");
        }

        private void OnRunRepricingClick(object sender, EventArgs e)
        {
            MessageBox.Show("Запуск репрайсинга...", "Информация");
        }

        private void OnRepricingSettingsClick(object sender, EventArgs e)
        {
            MessageBox.Show("Настройки стратегии репрайсинга...", "Информация");
        }

        private void OnPriceHistoryClick(object sender, EventArgs e)
        {
            MessageBox.Show("История изменения цен...", "Информация");
        }

        private void OnSalesReportsClick(object sender, EventArgs e)
        {
            MessageBox.Show("Отчеты о продажах...", "Информация");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
