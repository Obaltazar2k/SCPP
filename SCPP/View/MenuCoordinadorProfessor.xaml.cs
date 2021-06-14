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
    /// Lógica de interacción para MenuCoordinadorProfessor.xaml
    /// </summary>
    public partial class MenuCoordinadorProfessor : UserControl
    {
        public MenuCoordinadorProfessor()
        {
            InitializeComponent();
        }

        private void ConsultProfesorsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ConsultarProfesores());
            return;
        }

        private void AssignGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new AsignarProfesorGrupo());
            return;
        }

        private void DesassignGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new DesasignarProfesorGrupo());
            return;
        }

        private void RegisterProfesorButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarProfesor());
            return;
        }
    }
}
