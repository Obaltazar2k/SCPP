using System;
using SCPP.DataAcces;
using SCPP.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using System.Windows.Input;
using System.Data.Entity.Core;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para ConsultarProyectos.xaml
    /// </summary>
    public partial class ConsultarProyectos : Page
    {
        private readonly List<String> filters = new List<String> { "Ninguno", "Activo", "No activo" };
        private ObservableCollection<Proyecto> projectsCollection;
        private ObservableCollection<Proyecto> projectsCollectionFiltered;
        private Proyecto projectSelected = null;

        public ConsultarProyectos()
        {
            try
            {
                InitializeComponent();
                ComboBoxFilter.ItemsSource = filters;
                DataContext = this;
                GetProjects();
                ComboBoxFilter.SelectedItem = "Activo";
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
        }

        private void GetProjects()
        {
            projectsCollection = new ObservableCollection<Proyecto>();
            using (SCPPContext context = new SCPPContext())
            {
                var projectList = context.Proyecto.ToList();
                if (projectList != null)
                {
                    foreach (Proyecto project in projectList)
                    {
                        if (project != null)
                            projectsCollection.Add(project);
                    }
                }
            }
            ProjectList.ItemsSource = projectsCollection;
            DataContext = projectsCollection;
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarProyecto(projectSelected));
            return;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                GetProjects();
                var selectedFilter = ComboBoxFilter.SelectedItem;
                ComboBoxFilter.SelectedItem = "Ninguno";
                ComboBoxFilter.SelectedItem = selectedFilter;
                ManageButton.IsEnabled = false;
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }


        private void ProjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                projectSelected = (Proyecto)dataGrid.SelectedItems[0];
                if (projectSelected != null)
                    ManageButton.IsEnabled = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void RegisterProjectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarProyecto());
            return;
        }

        private void SetFilter()
        {
            try
            {
                var filter = (String)ComboBoxFilter.SelectedItem;
                using (SCPPContext context = new SCPPContext())
                {
                    projectsCollectionFiltered = new ObservableCollection<Proyecto>();
                    switch (filter)
                    {
                        case "Ninguno":
                            ProjectList.ItemsSource = projectsCollection;
                            break;

                        case "Activo":
                            foreach (Proyecto project in projectsCollection)
                            {
                                if (project.Activo == 1)
                                    projectsCollectionFiltered.Add(project);
                            }
                            ProjectList.ItemsSource = projectsCollectionFiltered;
                            break;

                        case "No activo":
                            foreach (Proyecto project in projectsCollection)
                            {
                                if (project.Activo != 1)
                                    projectsCollectionFiltered.Add(project);
                            }
                            ProjectList.ItemsSource = projectsCollectionFiltered;
                            break;
                    }
                }
                DataContext = this;
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void SpecificSearch(string searchText)
        {
            projectsCollection.Clear();
            using (SCPPContext context = new SCPPContext())
            {
                var projectBySearch = context.Proyecto.Where(
                    p => p.Actividades.Contains(searchText) ||
                    p.Descripcion.Contains(searchText) ||
                    p.Nombre.Contains(searchText));

                if (projectBySearch != null)
                {
                    foreach (Proyecto project in projectBySearch)
                    {
                        if (project != null)
                            projectsCollection.Add(project);
                    }
                }
                SetFilter();
            }
        }

        private void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string searchText = TextBoxSearch.Text;
                SpecificSearch(searchText);
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }
    }
}
