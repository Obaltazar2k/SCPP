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

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuCoordinadorOrganization.xaml
    /// </summary>
    public partial class MenuCoordinadorOrganization : UserControl
    {
        public MenuCoordinadorOrganization()
        {
            InitializeComponent();
        }

        private void GetOrganizationsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RegisterOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarOrganizacion());
            return;
        }
    }
}
