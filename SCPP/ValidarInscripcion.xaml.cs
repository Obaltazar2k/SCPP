using SCPP.Utilities;
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
using WPFCustomMessageBox;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para ValidarInscripcion.xaml
    /// </summary>
    public partial class ValidarInscripcion : Page
    {
        private readonly DateTime thisDay = DateTime.Today;
        private ObservableCollection<Estudiante> studentsCollection;
        ObservableCollection<Estudiante> selectedStudents = new ObservableCollection<Estudiante>();

        private string periodo;


        public ValidarInscripcion()
        {
            InitializeComponent();
            GetStudents();
            AddInformationToLabels();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeStudentStatus();
            ConfirmedRegistrationMessage();
        }

        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStudents.Clear();

            foreach(var student in StudentsList.SelectedItems)
            {
                selectedStudents.Add((Estudiante)student);
            }
        }

        private void GetStudents()
        {
            studentsCollection = new ObservableCollection<Estudiante>();

            try
            {
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
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            StudentsList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }

        private void AddInformationToLabels()
        {
            LabelFecha.Content += thisDay.ToString("d");
            periodo = Period.GetPeriod();
            LabelPeriodo.Content += periodo;
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

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
    }
}
