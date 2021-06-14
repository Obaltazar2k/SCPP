using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
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
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para ConsultarGrupos.xaml
    /// </summary>
    public partial class ConsultarGrupos : Page
    {
        private readonly List<String> filters = new List<String> { "Ninguno", "Disponible", "Asignado" };
        private string _user;
        private ObservableCollection<Grupo> groupsCollection;
        private ObservableCollection<Grupo> groupsCollectionFiltered;
        private Grupo groupSelected = null;
        private string _period;
        private bool isCoordinator = false;

        public ConsultarGrupos()
        {
            try
            {
                InitializeComponent();
                ComboBoxFilter.ItemsSource = filters;
                DataContext = this;
                _period = Period.GetPeriod();
                _user = "";
                DataContext = this;
                groupsCollection = new ObservableCollection<Grupo>();
                GetSesion();
                GetGroups();
                ComboBoxFilter.SelectedItem = "Ninguno";
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }
            

        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
        }

        private void ProfessorSearch(string searchText)
        {
            groupsCollection.Clear();
            using(SCPPContext context = new SCPPContext())
            {
                var groupsBySearch = context.Grupo.Where(s => (s.Nrc.ToString().Contains(searchText) ||
                    s.Cupo.ToString().Contains(searchText) ||
                    s.Bloque.Contains(searchText) ||
                    s.Seccion.Contains(searchText) ||
                    s.Periodo.Contains(searchText))
                    && s.Rfcprofesor.Equals(_user));

                if(groupsBySearch != null)
                {
                    foreach(Grupo group in groupsBySearch)
                    {
                        if(group != null)
                            groupsCollection.Add(group);
                    }
                }
            }
            SetFilter();
        }

        private void CoordinatorSearch(string searchText)
        {
            groupsCollection.Clear();
            using(SCPPContext context = new SCPPContext())
            {
                var groupsBySearch = context.Grupo.Where(s => (s.Nrc.ToString().Contains(searchText) ||
                    s.Cupo.ToString().Contains(searchText) ||
                    s.Bloque.Contains(searchText) ||
                    s.Seccion.Contains(searchText) ||
                    s.Periodo.Contains(searchText))
                    );

                if(groupsBySearch != null)
                {
                    foreach(Grupo group in groupsBySearch)
                    {
                        if(group != null)
                            groupsCollection.Add(group);
                    }
                }
            }
            SetFilter();
        }

        public ObservableCollection<Grupo> GetList(bool isCoordinator, Profesor profesor)
        {
            groupsCollection.Clear();

            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    IQueryable<Grupo> groupsInBD = null;

                    if(isCoordinator)
                    {
                        groupsInBD = context.Grupo;
                    }
                    else
                    {
                        groupsInBD = context.Grupo.Where(p => p.Rfcprofesor.Equals(_user) && p.Periodo.Equals(_period));

                    }
                    if(groupsInBD != null)
                    {
                        foreach(Grupo grupo in groupsInBD)
                        {
                            if(grupo != null)
                            {
                                groupsCollection.Add(grupo);
                            }
                        }
                    }
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }
            
            return groupsCollection;
        }

        private void GetGroups()
        {
            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    var profesor = context.Profesor.FirstOrDefault(c => c.Numtrabajador.Equals(_user));

                    if(profesor != null)
                    {
                        isCoordinator = false;
                    }
                    else
                    {
                        isCoordinator = true;
                    }
                    groupsCollection = GetList(isCoordinator, profesor);
                }

                GroupList.ItemsSource = groupsCollection;
                DataContext = groupsCollection;
            }
            catch(Exception)
            {
                Restarter.RestarSCPP();
            }
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchText = TextBoxSearch.Text;
                if(isCoordinator)
                {
                    CoordinatorSearch(searchText);
                }
                else
                {
                    ProfessorSearch(searchText);
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarGrupo(groupSelected));
            return;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                groupsCollection.Clear();
                GetGroups();
                var selectedFilter = ComboBoxFilter.SelectedItem;
                ComboBoxFilter.SelectedItem = "Ninguno";
                ComboBoxFilter.SelectedItem = selectedFilter;
                ManageButton.IsEnabled = false;
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }
            
        }

        private void GroupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                groupSelected = (Grupo)dataGrid.SelectedItems[0];
                if(groupSelected != null)
                    ManageButton.IsEnabled = true;
            }
            catch(ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
        }

        public void SetFilter()
        {
            var filter = (String)ComboBoxFilter.SelectedItem;

            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    groupsCollectionFiltered = new ObservableCollection<Grupo>();
                    switch(filter)
                    {
                        case "Ninguno":
                            GroupList.ItemsSource = groupsCollection;
                            break;

                        case "Disponible":
                            foreach(Grupo grupo in groupsCollection)
                            {
                                if(grupo.Estado.Equals("Disponible"))
                                    groupsCollectionFiltered.Add(grupo);
                            }
                            GroupList.ItemsSource = groupsCollectionFiltered;
                            break;

                        case "Asignado":
                            foreach(Grupo grupo in groupsCollection)
                            {
                                if(grupo.Estado.Equals("Asignado"))
                                    groupsCollectionFiltered.Add(grupo);
                            }
                            GroupList.ItemsSource = groupsCollectionFiltered;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            DataContext = this;
        }
    }
}