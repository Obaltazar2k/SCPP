using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para ConsultarEstudiantes.xaml
    /// </summary>
    public partial class ConsultarEstudiantes : Page
    {
        private readonly List<String> filters = new List<String> { "Ninguno", "Activo", "No activo" };
        private string _user;
        private bool isCordinator;
        private Profesor profesor;
        private ObservableCollection<Estudiante> studentsCollection;
        private ObservableCollection<Estudiante> studentsCollectionFiltered;
        private Estudiante studentSelected = null;

        public ConsultarEstudiantes()
        {
            try
            {
                InitializeComponent();
                ComboBoxFilter.ItemsSource = filters;
                DataContext = this;
                GetSesion();
                GetStudents();
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

        private void CordinatorSearch(string searchText)
        {
            studentsCollection.Clear();
            using (SCPPContext context = new SCPPContext())
            {
                var studentsBySearch = context.Estudiante.Where(
                    s => s.Matricula.Contains(searchText) ||
                    s.Nombre.Contains(searchText) ||
                    s.Apellidomaterno.Contains(searchText) ||
                    s.Apellidopaterno.Contains(searchText) ||
                    s.Correopersonal.Contains(searchText) ||
                    s.Telefono.Contains(searchText));

                if (studentsBySearch != null)
                {
                    foreach (Estudiante student in studentsBySearch)
                    {
                        if (student != null)
                            studentsCollection.Add(student);
                    }
                }
                SetFilter();
            }
        }

        private ObservableCollection<Estudiante> GetList(bool isCordinator, Profesor profesor)
        {
            studentsCollection.Clear();
            using (SCPPContext context = new SCPPContext())
            {
                IQueryable<Estudiante> studentsInDB = null;
                if (isCordinator)
                    studentsInDB = context.Estudiante;
                else
                {
                    studentsInDB = context.Grupo.Join(
                        context.Inscripción,
                        g => g.GrupoID,
                        i => i.GrupoID,
                        (g, i) => new { group = g, inscription = i })
                        .Join(
                        context.Estudiante,
                        j => j.inscription.Matriculaestudiante,
                        s => s.Matricula,
                        (j, s) => new { join = j, student = s })
                        .Where(q => q.join.group.Rfcprofesor == profesor.Numtrabajador)
                        .Select(q => q.student);
                }

                if (studentsInDB != null)
                {
                    foreach (Estudiante student in studentsInDB)
                    {
                        if (student != null)
                            studentsCollection.Add(student);
                    }
                }
                return studentsCollection;
            }
        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
        }

        private void GetStudents()
        {
            studentsCollection = new ObservableCollection<Estudiante>();
            using (SCPPContext context = new SCPPContext())
            {
                profesor = context.Profesor.FirstOrDefault(c => c.Numtrabajador == _user);
                if (profesor != null)
                {
                    isCordinator = false;
                    RegisterStudentButton.Visibility = Visibility.Hidden;
                }
                else
                    isCordinator = true;
                studentsCollection = GetList(isCordinator, profesor);
            }
            StudentList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarEstudiante(studentSelected));
            return;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                GetStudents();
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

        private void ProfesorSearch(string searchText)
        {
            studentsCollection.Clear();
            using (SCPPContext context = new SCPPContext())
            {
                var studentsBySearch = context.Grupo.Join(
                    context.Inscripción,
                    g => g.GrupoID,
                    i => i.GrupoID,
                    (g, i) => new { group = g, inscription = i })
                    .Join(
                    context.Estudiante,
                    j => j.inscription.Matriculaestudiante,
                    s => s.Matricula,
                    (j, s) => new { join = j, student = s })
                    .Where(q => q.join.group.Rfcprofesor == profesor.Numtrabajador)
                    .Select(q => q.student).Where(
                    s => s.Matricula.Contains(searchText) ||
                    s.Nombre.Contains(searchText) ||
                    s.Apellidomaterno.Contains(searchText) ||
                    s.Apellidopaterno.Contains(searchText) ||
                    s.Correopersonal.Contains(searchText) ||
                    s.Telefono.Contains(searchText));

                if (studentsBySearch != null)
                {
                    foreach (Estudiante student in studentsBySearch)
                    {
                        if (student != null)
                            studentsCollection.Add(student);
                    }
                }
                SetFilter();
            }
        }

        private void RegisterStudentButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarEstudiante());
            return;
        }

        private void SetFilter()
        {
            try
            {
                var filter = ComboBoxFilter.SelectedItem;
                using (SCPPContext context = new SCPPContext())
                {
                    studentsCollectionFiltered = new ObservableCollection<Estudiante>();
                    switch (filter)
                    {
                        case "Ninguno":
                            StudentList.ItemsSource = studentsCollection;
                            break;

                        case "Activo":
                            foreach (Estudiante student in studentsCollection)
                            {
                                if (student.Activo == 1)
                                    studentsCollectionFiltered.Add(student);
                            }
                            StudentList.ItemsSource = studentsCollectionFiltered;
                            break;

                        case "No activo":
                            foreach (Estudiante student in studentsCollection)
                            {
                                if (student.Activo != 1)
                                    studentsCollectionFiltered.Add(student);
                            }
                            StudentList.ItemsSource = studentsCollectionFiltered;
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

        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                studentSelected = (Estudiante)dataGrid.SelectedItems[0];
                if (studentSelected != null)
                    ManageButton.IsEnabled = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void TextBoxSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                string searchText = TextBoxSearch.Text;
                if (isCordinator)
                    CordinatorSearch(searchText);
                else
                    ProfesorSearch(searchText);
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }
    }
}