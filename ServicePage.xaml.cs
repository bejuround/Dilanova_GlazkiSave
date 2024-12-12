using System;
using System.Collections.Generic;
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

namespace Dilanova_GlazkiSave
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {

        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;

        public ServicePage()
        {
            InitializeComponent();
            var currentServices = Dilanova_ГлазкиSaveEntities.GetContext().Agent.ToList(); 

            AgentListView.ItemsSource = currentServices;

            ComboType.SelectedIndex = 0;
            ComboAgentType.SelectedIndex = 0;
            UpdateAgent();
        }

      private void UpdateAgent()
        {
            var currentAgent = Dilanova_ГлазкиSaveEntities.GetContext().Agent.ToList();

            currentAgent = currentAgent.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())
            || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower())
            || p.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "").Contains(TBoxSearch.Text)).ToList();

            AgentListView.ItemsSource = currentAgent.ToList();

            if (ComboAgentType.SelectedIndex == 1)
            {
                currentAgent = currentAgent.Where(p => p.AgentType.Title == "ЗАО").ToList();
            }
            if (ComboAgentType.SelectedIndex == 2)
            {
                currentAgent = currentAgent.Where(p => p.AgentType.Title == "МКК").ToList();
            }
            if (ComboAgentType.SelectedIndex == 3)
            {
                currentAgent = currentAgent.Where(p => p.AgentType.Title == "МФО").ToList();
            }
            if (ComboAgentType.SelectedIndex == 4)
            {
                currentAgent = currentAgent.Where(p => p.AgentType.Title == "ОАО").ToList();
            }
            if (ComboAgentType.SelectedIndex == 5)
            {
                currentAgent = currentAgent.Where(p => p.AgentType.Title == "ООО").ToList();
            }
            if (ComboAgentType.SelectedIndex == 6)
            {
                currentAgent = currentAgent.Where(p => p.AgentType.Title == "ПАО").ToList();
            }

            AgentListView.ItemsSource = currentAgent.ToList();

            if (ComboType.SelectedIndex == 1)
            {
                currentAgent = currentAgent.OrderBy(p => p.Title).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentAgent = currentAgent.OrderByDescending(p => p.Title).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentAgent = currentAgent.OrderBy(p => p.Priority).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentAgent = currentAgent.OrderByDescending(p => p.Priority).ToList();
            }


            AgentListView.ItemsSource = currentAgent;

            TableList = currentAgent;
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void ComboAgentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }
    }
}
