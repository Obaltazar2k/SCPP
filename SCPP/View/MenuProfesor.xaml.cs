using SCPP.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuProfesor.xaml
    /// </summary>
    public partial class MenuProfesor : Page
    {
        public MenuProfesor()
        {
            InitializeComponent();
        }

        private void AsociarEstudianteGrupoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new AsociarEstudianteGrupo());
            return;
        }

        private void GetStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ConsultarEstudiantes());
            return;
        }

        private void ConsultarGruposButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ConsultarGrupos());
            return;
        }

        private void CloseSesionButton_Clicked(object sender, RoutedEventArgs e)
        {
            Sesion.CloseSesion();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }
    }
}