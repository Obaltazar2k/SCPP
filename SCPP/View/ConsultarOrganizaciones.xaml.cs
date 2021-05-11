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


namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para ConsultarOrganizaciones.xaml
    /// </summary>
    public partial class ConsultarOrganizaciones : Page
    {
        private readonly List<String> filters = new List<String> { "Ninguno", "Activo", "No activo" };
        private ObservableCollection<Organización> organizationCollection;
        private ObservableCollection<Organización> organizationCollectionFiltered;
        private Organización organizationSelected = null;

        public ConsultarOrganizaciones()
        {
            InitializeComponent();
            ComboBoxFilter.ItemsSource = filters;
            DataContext = this;
            GetOrganizations();
            ComboBoxFilter.SelectedItem = "Activo";
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

        private void GetOrganizations()
        {
            organizationCollection = new ObservableCollection<Organización>();
            using (SCPPContext context = new SCPPContext())
            {
                var organizationList = context.Organización.ToList();
                if (organizationList != null)
                {
                    foreach (Organización organization in organizationList)
                    {
                        if (organization != null)
                            organizationCollection.Add(organization);
                    }
                }
            }
            OrganizationList.ItemsSource = organizationCollection;
            DataContext = organizationCollection;
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarOrganizacion(organizationSelected));
            return;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            GetOrganizations();
            var selectedFilter = ComboBoxFilter.SelectedItem;
            ComboBoxFilter.SelectedItem = "Ninguno";
            ComboBoxFilter.SelectedItem = selectedFilter;
            ManageButton.IsEnabled = false;
        }

        private void OrganizationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                organizationSelected = (Organización)dataGrid.SelectedItems[0];
                if (organizationSelected != null)
                    ManageButton.IsEnabled = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void RegisterOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarOrganizacion());
            return;
        }

        private void SetFilter()
        {
            var filter = (String)ComboBoxFilter.SelectedItem;
            using (SCPPContext context = new SCPPContext())
            {
                organizationCollectionFiltered = new ObservableCollection<Organización>();
                switch (filter)
                {
                    case "Ninguno":
                        OrganizationList.ItemsSource = organizationCollection;
                        break;

                    case "Activo":
                        foreach (Organización organization in organizationCollection)
                        {
                            if (organization.Activo == 1)
                                organizationCollectionFiltered.Add(organization);
                        }
                        OrganizationList.ItemsSource = organizationCollectionFiltered;
                        break;

                    case "No activo":
                        foreach (Organización organization in organizationCollection)
                        {
                            if (organization.Activo != 1)
                                organizationCollectionFiltered.Add(organization);
                        }
                        OrganizationList.ItemsSource = organizationCollectionFiltered;
                        break;
                }
            }
            DataContext = this;
        }

        private void SpecificSearch(string searchText)
        {
            organizationCollection.Clear();
            using (SCPPContext context = new SCPPContext())
            {
                var organizationBySearch = context.Organización.Where(
                    o => o.Nombre.Contains(searchText) ||
                    o.Correo.Contains(searchText) ||
                    o.Telefono.Contains(searchText) ||
                    o.Sector.Contains(searchText) ||
                    o.Calle.Contains(searchText));

                if (organizationBySearch != null)
                {
                    foreach (Organización organization in organizationBySearch)
                    {
                        if (organization != null)
                            organizationCollection.Add(organization);
                    }
                }
                SetFilter();
            }
        }

        private void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            string searchText = TextBoxSearch.Text;
            SpecificSearch(searchText);

        }
    }
}
