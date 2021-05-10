using SCPP.Utilities;
using System.Windows;
using System.Windows.Controls;
using SCPP.DataAcces;
using System.Linq;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuEstudiante.xaml
    /// </summary>
    public partial class MenuEstudiante : Page
    {
        Sesion userSesion;
        public MenuEstudiante()
        {
            InitializeComponent();
            userSesion = Sesion.GetSesion;
        }

        private void EscogerProyectoButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new EscogerProyecto());
            return;
        }

        private void SubirReporteButton_Click(object sender, RoutedEventArgs e)
        {
            string matricula = userSesion.Username;
            Inscripción inscripcion;
            using (SCPPContext context = new SCPPContext())
            {
                inscripcion = context.Inscripción.FirstOrDefault(u => u.Matriculaestudiante == matricula);
            }
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new EntregarReporte(inscripcion));
            return;
        }


    }
}