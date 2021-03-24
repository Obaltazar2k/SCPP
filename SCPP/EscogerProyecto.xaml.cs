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
using SCPP.Utilities;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para EscogerProyecto.xaml
    /// </summary>
    public partial class EscogerProyecto : Page
    {
        private Proyecto proyectSelected = null;

        public EscogerProyecto()
        {
            InitializeComponent();
            GetProyects();
        }

        private void ProyectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChoosenProyectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetProyects()
        {
            List<Proyecto> proyectsCollection = new List<Proyecto>();
            using (SCPPContext context = new SCPPContext())
            {
                var proyectsList = context.Proyecto.Where(p => p.Noestudiantes <= 2);
                if (proyectsList != null)
                {
                    foreach (Proyecto proyecto in proyectsList)
                    {
                        if (proyecto != null)
                            proyectsCollection.Add(proyecto);
                    }
                }
            }
            ProyectsList.ItemsSource = proyectsCollection;
        }
    }
}
