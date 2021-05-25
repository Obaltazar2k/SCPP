using System.Windows;
using System.Windows.Controls;

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
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ConsultarOrganizaciones());
            return;
        }

        private void RegisterOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarOrganizacion());
            return;
        }

        private void RegisterResponsableButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarResposable());
            return;
        }
    }
}
