using System.Windows;
using System.Windows.Controls;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para MenuEstudiante.xaml
    /// </summary>
    public partial class MenuEstudiante : Page
    {
        public MenuEstudiante()
        {
            InitializeComponent();
        }

        private void EscogerProyectoButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new EscogerProyecto());
            return;
        }
    }
}