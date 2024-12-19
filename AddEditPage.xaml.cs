using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Dilanova_GlazkiSave
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgent = new Agent();
        private ProductSale _currentProductSale = new ProductSale();

        public AddEditPage(Agent SelectedAgent)
        {
            InitializeComponent();

            ComboType.SelectedIndex = 0;

            if (SelectedAgent != null)
            {
                _currentAgent = SelectedAgent;
                ComboType.SelectedIndex = _currentAgent.AgentTypeID - 1;
                HistoryOfRealisationListView.Visibility = Visibility.Visible;
            
            }
            else
            {
                HistoryOfRealisationListView.Visibility = Visibility.Hidden;
            }

            var allProducts = Dilanova_ГлазкиSaveEntities.GetContext().Product.ToList();

            var currentProductSales = Dilanova_ГлазкиSaveEntities.GetContext().ProductSale.ToList();
            currentProductSales = currentProductSales.Where(p => p.AgentID == _currentAgent.ID).ToList();

            HistoryOfRealisationListView.ItemsSource = currentProductSales;

            ComboProductSaleTitle.ItemsSource = allProducts;


            DataContext = _currentProductSale;


            DataContext = _currentAgent;
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentAgent.Logo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");

            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (_currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentAgent.Phone.Replace("(", "").Replace("-", "").Replace("+", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentAgent.AgentTypeID = ComboType.SelectedIndex + 1;

            if (_currentAgent.ID == 0)
                Dilanova_ГлазкиSaveEntities.GetContext().Agent.Add(_currentAgent);
            try
            {
                Dilanova_ГлазкиSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var _currentAgent = (sender as Button).DataContext as Agent;

            var currentProductSale = Dilanova_ГлазкиSaveEntities.GetContext().ProductSale.ToList();
            currentProductSale = currentProductSale.Where(p => p.AgentID == _currentAgent.ID).ToList();

            var currentAgentPriorityHistory = Dilanova_ГлазкиSaveEntities.GetContext().AgentPriorityHistory.ToList();
            var currentShop = Dilanova_ГлазкиSaveEntities.GetContext().Shop.ToList();
            currentAgentPriorityHistory = currentAgentPriorityHistory.Where(p => p.AgentID == _currentAgent.ID).ToList();
            currentShop = currentShop.Where(p => p.AgentID == _currentAgent.ID).ToList();


            if (currentProductSale.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существует история реализации продуктов");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Dilanova_ГлазкиSaveEntities.GetContext().Agent.Remove(_currentAgent);

                        if (currentAgentPriorityHistory.Count != 0)
                        {
                            for (int i = 0; currentAgentPriorityHistory.Count == i; i++)
                                Dilanova_ГлазкиSaveEntities.GetContext().AgentPriorityHistory.Remove(currentAgentPriorityHistory[i]);
                        }
                        if (currentShop.Count != 0)
                        {
                            for (int i = 0; currentShop.Count == i; i++)
                                Dilanova_ГлазкиSaveEntities.GetContext().Shop.Remove(currentShop[i]);
                        }
                        Dilanova_ГлазкиSaveEntities.GetContext().SaveChanges();

                        MessageBox.Show("Информация удалена!");
                        Manager.MainFrame.GoBack();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void AddProductSale_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (ComboProductSaleTitle.SelectedItem == null)
                errors.AppendLine("Укажите продукт");
            if (string.IsNullOrWhiteSpace(ProductCountTB.Text))
                errors.AppendLine("Укажите количество продуктов");
            bool isProductCountDigits = true;
            for (int i = 0; i < ProductCountTB.Text.Length; i++)
            {
                if (ProductCountTB.Text[i] < '0' || ProductCountTB.Text[i] > '9')
                {
                    isProductCountDigits = false;
                }
            }
            if (!isProductCountDigits)
                errors.AppendLine("Укажите численное положительное продуктов");

            if (string.IsNullOrWhiteSpace(SaleDateDatePicker.Text))
                errors.AppendLine("Укажите дату продажи");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            _currentProductSale.AgentID = _currentAgent.ID;
            _currentProductSale.ProductID = ComboProductSaleTitle.SelectedIndex + 1;
            _currentProductSale.ProductCount = Convert.ToInt32(ProductCountTB.Text);
            _currentProductSale.SaleDate = Convert.ToDateTime(SaleDateDatePicker.Text);
            if (_currentProductSale.ID == 0)
                Dilanova_ГлазкиSaveEntities.GetContext().ProductSale.Add(_currentProductSale);



            try
            {
                Dilanova_ГлазкиSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteProductSale_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (ProductSale history in HistoryOfRealisationListView.SelectedItems)
                    {
                        Dilanova_ГлазкиSaveEntities.GetContext().ProductSale.Remove(history);

                    }
                    Dilanova_ГлазкиSaveEntities.GetContext().SaveChanges();

                    MessageBox.Show("Информация удалена!");
                    Manager.MainFrame.GoBack();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
