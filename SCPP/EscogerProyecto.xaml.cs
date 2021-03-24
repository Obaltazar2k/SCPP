using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<Proyecto> selectedProjects = new ObservableCollection<Proyecto>();
        ObservableCollection<Proyecto> choosenProjects = new ObservableCollection<Proyecto>();

        public EscogerProyecto()
        {
            InitializeComponent();
            GetProyects();
        }

        private void ProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddButton.IsEnabled = true;
            RemoveButton.IsEnabled = false;
            selectedProjects.Clear();
            foreach(var project in ProjectsList.SelectedItems)
            {
                selectedProjects.Add((Proyecto)project);
            }
        }

        private void ChoosenProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveButton.IsEnabled = true;
            AddButton.IsEnabled = false;
            choosenProjects.Clear();
            foreach(var project in ChoosenProjectsList.SelectedItems)
            {
                choosenProjects.Add((Proyecto)project);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int numProjectsSelected = selectedProjects.Count();
            if (numProjectsSelected <= 3)
            {
                ChoosenProjectsList.ItemsSource = selectedProjects;
                DataContext = selectedProjects;
            }
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
                var proyectsList = context.Proyecto.Where(p => p.Inscripción.Count <= 2);
                if (proyectsList != null)
                {
                    foreach (Proyecto proyecto in proyectsList)
                    {
                        if (proyecto != null)
                            proyectsCollection.Add(proyecto);
                    }
                }
            }
            ProjectsList.ItemsSource = proyectsCollection;
        }
    }
}
