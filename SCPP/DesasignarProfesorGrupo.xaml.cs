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

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para Desasignar_profesor_a_grupo.xaml
    /// </summary>
    public partial class DesasignarProfesorGrupo : Page
    {
        public DesasignarProfesorGrupo()
        {
            InitializeComponent();
        }

        private void DesassignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MenuCoordinador());
            return;
        }
    }
}
