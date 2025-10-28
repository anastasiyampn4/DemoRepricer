using System;
using System.Windows.Forms;
using WildberriesRepricer.Data;
using WildberriesRepricer.Models;

namespace WildberriesRepricer.Forms
{
    public partial class ProductManagementForm : Form
    {
        private DatabaseContext _context;
        private ProductRepository _productRepo;
        private DataGridView _productGrid;

        public ProductManagementForm()
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _productRepo = new ProductRepository(_context);
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление Товарами";
            this.Size = new System.Drawing.Size(1000, 600);

            _productGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            var btnAdd = new Button { Text = "Добавить", Left = 10, Top = 10, Width = 100 };
            var btnEdit = new Button { Text = "Редактировать", Left = 120, Top = 10, Width = 100 };
            var btnDelete = new Button { Text = "Удалить", Left = 230, Top = 10, Width = 100 };
            var btnRefresh = new Button { Text = "Обновить", Left = 340, Top = 10, Width = 100 };

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnRefresh);

            this.Controls.Add(_productGrid);
            this.Controls.Add(buttonPanel);
        }

        private void LoadProducts()
        {
            var products = _productRepo.GetAll();
            _productGrid.DataSource = products;
        }
    }
}
