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
    /// Lógica de interacción para CalificarReporte.xaml
    /// </summary>
    public partial class CalificarReporte : Page
    {
        public CalificarReporte()
        {
            InitializeComponent();
            //Cuando recuperes el archivo cambia la propiedad de content con el nombre del archivo
            FileButton.Content = "Archivo.txt";
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
