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
using System.Data.Entity.Core;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para ConsultarProfesores.xaml
    /// </summary>
    public partial class ConsultarProfesores : Page
    {
        private readonly List<String> filters = new List<String> { "Ninguno", "Activo", "No activo" };
        private ObservableCollection<Profesor> profesorsCollection;
        private ObservableCollection<Profesor> profesorsCollectionFiltered;
        private Profesor profesorSelected = null;

        public ConsultarProfesores()
        {
            try
            {
                InitializeComponent();
                ComboBoxFilter.ItemsSource = filters;
                DataContext = this;
                GetProfesors();
                ComboBoxFilter.SelectedItem = "Activo";
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
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

        private void SpecificSearch(string searchText)
        {
            profesorsCollection.Clear();
            using (SCPPContext context = new SCPPContext())
            {
                var profesorsBySearch = context.Profesor.Where(
                    s => s.Numtrabajador.Contains(searchText) ||
                    s.Nombre.Contains(searchText) ||
                    s.Apellidomaterno.Contains(searchText) ||
                    s.Apellidopaterno.Contains(searchText) ||
                    s.Correopersonal.Contains(searchText));

                if (profesorsBySearch != null)
                {
                    foreach (Profesor profesor in profesorsBySearch)
                    {
                        if (profesor != null)
                            profesorsCollection.Add(profesor);
                    }
                }
                SetFilter();
            }
        }

        private void GetProfesors()
        {
            profesorsCollection = new ObservableCollection<Profesor>();
            using (SCPPContext context = new SCPPContext())
            {
                var profesorsList = context.Profesor.ToList();
                if (profesorsList != null)
                {
                    foreach (Profesor profesor in profesorsList)
                    {
                        if (profesor != null)
                            profesorsCollection.Add(profesor);
                    }
                }
            }
            ProfesorsList.ItemsSource = profesorsCollection;
            DataContext = profesorsCollection;
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarProfesor(profesorSelected));
            return;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            GetProfesors();
            var selectedFilter = ComboBoxFilter.SelectedItem;
            ComboBoxFilter.SelectedItem = "Ninguno";
            ComboBoxFilter.SelectedItem = selectedFilter;
            ManageButton.IsEnabled = false;
        }

        private void RegisterProfesorButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarProfesor());
            return;
        }

        private void SetFilter()
        {
            var filter = (String)ComboBoxFilter.SelectedItem;
            using (SCPPContext context = new SCPPContext())
            {
                profesorsCollectionFiltered = new ObservableCollection<Profesor>();
                switch (filter)
                {
                    case "Ninguno":
                        ProfesorsList.ItemsSource = profesorsCollection;
                        break;

                    case "Activo":
                        foreach (Profesor profesor in profesorsCollection)
                        {
                            if (profesor.Activo == 1)
                                profesorsCollectionFiltered.Add(profesor);
                        }
                        ProfesorsList.ItemsSource = profesorsCollectionFiltered;
                        break;

                    case "No activo":
                        foreach (Profesor profesor in profesorsCollection)
                        {
                            if (profesor.Activo != 1)
                                profesorsCollectionFiltered.Add(profesor);
                        }
                        ProfesorsList.ItemsSource = profesorsCollectionFiltered;
                        break;
                }
            }
            DataContext = this;
        }

        private void ProfesorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                profesorSelected = (Profesor)dataGrid.SelectedItems[0];
                if (profesorSelected != null)
                    ManageButton.IsEnabled = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void TextBoxSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string searchText = TextBoxSearch.Text;
            SpecificSearch(searchText);
        }

        public void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }
    }
}
