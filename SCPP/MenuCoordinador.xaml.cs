using System.Windows;
using System.Windows.Controls;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para MenuCoordinador.xaml
    /// </summary>
    public partial class MenuCoordinador : Page
    {
        public MenuCoordinador()
        {
            InitializeComponent();
        }

        private void AssignProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new AsignarProyectoEstudiante());
            return;
        }
    }   
}
