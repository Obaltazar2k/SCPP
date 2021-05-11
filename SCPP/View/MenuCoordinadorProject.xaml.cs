using System.Windows;
using System.Windows.Controls;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuCoordinadorProject.xaml
    /// </summary>
    public partial class MenuCoordinadorProject : UserControl
    {
        public MenuCoordinadorProject()
        {
            InitializeComponent();
        }

        private void GetProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ConsultarProyectos());
            return;
        }

        private void RegisterProyectButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarProyecto());
            return;
        }
        
        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GenerarReporte());
            return;
        }    
    }
}
