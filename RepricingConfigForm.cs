using System;
using System.Windows.Forms;
using WildberriesRepricer.Models;
using WildberriesRepricer.Services;
using WildberriesRepricer.Data;

namespace WildberriesRepricer.Forms
{
    public partial class RepricingConfigForm : Form
    {
        private RepricingConfig _config;
        private ComboBox _strategyComboBox;
        private TextBox _undercutPercentTextBox;
        private TextBox _undercutAmountTextBox;
        private TextBox _minMarginTextBox;
        private Button _executeButton;

        public RepricingConfigForm()
        {
            _config = new RepricingConfig();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Настройка Стратегии Репрайсинга";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            int yPos = 20;

            var strategyLabel = new Label { Text = "Стратегия:", Left = 20, Top = yPos, Width = 150 };
            _strategyComboBox = new ComboBox 
            { 
                Left = 180, 
                Top = yPos, 
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _strategyComboBox.Items.AddRange(new object[] 
            { 
                RepricingStrategy.MatchLowest,
                RepricingStrategy.UndercutByPercent,
                RepricingStrategy.UndercutByAmount,
                RepricingStrategy.MaintainMargin,
                RepricingStrategy.PremiumPricing
            });
            _strategyComboBox.SelectedIndex = 0;

            yPos += 40;
            var undercutPercentLabel = new Label { Text = "Понизить на %:", Left = 20, Top = yPos, Width = 150 };
            _undercutPercentTextBox = new TextBox { Left = 180, Top = yPos, Width = 100, Text = "5" };

            yPos += 40;
            var undercutAmountLabel = new Label { Text = "Понизить на руб.:", Left = 20, Top = yPos, Width = 150 };
            _undercutAmountTextBox = new TextBox { Left = 180, Top = yPos, Width = 100, Text = "50" };

            yPos += 40;
            var minMarginLabel = new Label { Text = "Мин. маржа %:", Left = 20, Top = yPos, Width = 150 };
            _minMarginTextBox = new TextBox { Left = 180, Top = yPos, Width = 100, Text = "15" };

            yPos += 60;
            _executeButton = new Button 
            { 
                Text = "Выполнить Репрайсинг", 
                Left = 150, 
                Top = yPos, 
                Width = 200, 
                Height = 35 
            };
            _executeButton.Click += OnExecuteClick;

            this.Controls.Add(strategyLabel);
            this.Controls.Add(_strategyComboBox);
            this.Controls.Add(undercutPercentLabel);
            this.Controls.Add(_undercutPercentTextBox);
            this.Controls.Add(undercutAmountLabel);
            this.Controls.Add(_undercutAmountTextBox);
            this.Controls.Add(minMarginLabel);
            this.Controls.Add(_minMarginTextBox);
            this.Controls.Add(_executeButton);
        }

        private void OnExecuteClick(object sender, EventArgs e)
        {
            _config.Strategy = (RepricingStrategy)_strategyComboBox.SelectedItem;
            _config.UndercutPercent = decimal.Parse(_undercutPercentTextBox.Text);
            _config.UndercutAmount = decimal.Parse(_undercutAmountTextBox.Text);
            _config.MinMarginPercent = decimal.Parse(_minMarginTextBox.Text);

            using (var context = new DatabaseContext())
            {
                var repricingService = new RepricingService(context);
                var results = repricingService.ExecuteBulkRepricing(_config);

                var message = $"Репрайсинг завершен!\n\nОбработано товаров: {results.Count}\n";
                int changed = 0;
                foreach (var result in results)
                {
                    if (result.PriceChanged) changed++;
                }
                message += $"Цены изменены: {changed}\n";
                message += $"Без изменений: {results.Count - changed}";

                MessageBox.Show(message, "Результаты Репрайсинга", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
