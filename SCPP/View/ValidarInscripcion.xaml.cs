using SCPP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using System.Data.Entity.Core;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para ValidarInscripcion.xaml
    /// </summary>
    public partial class ValidarInscripcion : Page
    {
        private readonly DateTime thisDay = DateTime.Today;
        private string periodo;
        private ObservableCollection<Estudiante> selectedStudents = new ObservableCollection<Estudiante>();
        private ObservableCollection<Estudiante> studentsCollection;

        public ValidarInscripcion()
        {
            try
            {
                InitializeComponent();
                AddInformationToLabels();
                GetStudents();

            }
            catch(EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                Loaded += ReturnToLogin;
            }

        }

        private void AddInformationToLabels()
        {
            LabelFecha.Content += thisDay.ToString("d");
            periodo = Period.GetPeriod();
            LabelPeriodo.Content += periodo;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ChangeStudentStatus()
        {
            ObservableCollection<Estudiante> studentList = new ObservableCollection<Estudiante>();

            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    foreach(var student in selectedStudents)
                    {
                        var foundStudent = context.Estudiante.Where(s => s.Matricula.Equals(student.Matricula)).
                            FirstOrDefault();

                        if(foundStudent != null)
                        {
                            foundStudent.Estado = "Inscrito";
                            context.SaveChanges();
                        }

                        studentList.Add(student);
                    }
                }

                foreach(var student in studentList)
                {
                    studentsCollection.Remove(student);
                }

                ConfirmedRegistrationMessage();
            }
            catch(EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");

                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if(CheckSelections())
            {
                ChangeStudentStatus();
            }
            else
            {
                CustomMessageBox.ShowOK("Selecciona por lo menos un estudiante", "No has seleccionado ningun estudiante", "Aceptar");
            }
        }

        private bool CheckSelections()
        {
            if(selectedStudents.Count == 0)
            {
                return false;
            }

            return true;
        }

        private void ConfirmedRegistrationMessage()
        {
            MessageBoxResult selection = CustomMessageBox.ShowOK("Preinscripciones realizadas con exito",
                "Preinscripciones confirmadas",
                "Aceptar");
            if(selection == MessageBoxResult.OK)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }

        private void GetStudents()
        {
            studentsCollection = new ObservableCollection<Estudiante>();

            using(SCPPContext context = new SCPPContext())
            {
                var unregisteredStudentsList = context.Estudiante.Where(s => s.Estado == "Preinscrito");

                if(unregisteredStudentsList != null)
                {
                    foreach(Estudiante student in unregisteredStudentsList)
                    {
                        if(student != null)
                            studentsCollection.Add(student);
                    }
                }
            }

            StudentsList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }

        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStudents.Clear();

            foreach(var student in StudentsList.SelectedItems)
            {
                selectedStudents.Add((Estudiante)student);
            }
        }

    }
}