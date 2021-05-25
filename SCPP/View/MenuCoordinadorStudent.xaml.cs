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
    /// Lógica de interacción para UserControlStudent.xaml
    /// </summary>
    public partial class MenuCoordinadorStudent : UserControl
    {
        public MenuCoordinadorStudent()
        {
            InitializeComponent();
        }

        private void AssignProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new AsignarProyectoEstudiante());
            return;
        }

        private void GetStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ConsultarEstudiantes());
            return;
        }

        private void ValidateEnrrollmentButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ValidarInscripcion());
            return;
        }

        private void GenerateAssignDocButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GenerarOficioAsignacion());
            return;
        }
    }
}
