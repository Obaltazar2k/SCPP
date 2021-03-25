using SCPP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCustomMessageBox;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para AsignarEstudianteProyecto.xaml
    /// </summary>
    public partial class AsignarProyectoEstudiante : Page
    {
        private readonly DateTime thisDay = DateTime.Today;
        private string periodo;
        private ObservableCollection<Proyecto> proyectsCollection;
        private Proyecto proyectSelected = null;
        private ObservableCollection<Estudiante> studentsCollection;
        private Estudiante studentSelected = null;

        public AsignarProyectoEstudiante()
        {
            InitializeComponent();
            AddInformationToLabels();
            GetStudents();
            GetProyects();
        }

        public AsignarProyectoEstudiante(Estudiante student)
        {
            InitializeComponent();
            AddInformationToLabels();
            GetStudents();
            GetProyects();

            var studentSelection = (from i in studentsCollection
                                    where i.Matricula == student.Matricula
                                    select i).FirstOrDefault();
            if (studentSelection != null)
                StudentsList.SelectedItem = studentSelection;
        }

        private void AddInformationToLabels()
        {
            LabelFecha.Content += thisDay.ToString("d");
            periodo = Period.GetPeriod();
            LabelPeriodo.Content += periodo;
        }

        private void AssignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (proyectSelected == null && studentSelected == null)
                return;

            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea asignar al ESTUDIANTE "
                + studentSelected.Nombre + " " + studentSelected.Apellidopaterno + " con matricula " + studentSelected.Matricula
                + " al PROYECTO " + proyectSelected.Nombre + "?", "Confirmación", "Si", "No");
            var assignDone = false;
            if (confirmation == MessageBoxResult.Yes)
            {
                Inscripción newInscription = CreateInscription(studentSelected.Matricula, proyectSelected.Clave);
                Expediente newExpedient = CreateExpedient(newInscription.InscripciónID);
                assignDone = true;
            }
            else
                return;

            if (assignDone == true)
            {
                MessageBoxResult result = CustomMessageBox.ShowYesNo("La asignación ha sido realizada con éxito.", "Asignación", "Generar oficio de asignación", "Finalizar");
                if (result == MessageBoxResult.Yes)
                {
                    //extend CU Generar oficio de asignación (newInscription.InscripciónID);
                }
                else
                {
                    CancelButton_Click(new object(), new RoutedEventArgs());
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CheckSelecctions()
        {
            if ((studentSelected != null) && (proyectSelected != null))
                AssignButton.IsEnabled = true;
        }

        private Expediente CreateExpedient(int inscripciónID)
        {
            var expedient = new Expediente
            {
                Fechafinpp = null,
                Fechainiciopp = thisDay,
                Horasacumuladas = 0,
                Numreportesentregados = 0,
                InscripciónID = inscripciónID
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Expediente.Add(expedient);
                context.SaveChanges();
            }
            return expedient;
        }

        private Inscripción CreateInscription(string matricula, int clave)
        {
            var inscription = new Inscripción
            {
                Estatus = "Cursando",
                Fecha = thisDay,
                Periodo = periodo,
                Tipo = "A tiempo",
                Matriculaestudiante = matricula,
                Claveproyecto = clave
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Inscripción.Add(inscription);
                context.SaveChanges();
            }
            return inscription;
        }

        private void GetProyects()
        {
            proyectsCollection = new ObservableCollection<Proyecto>();
            using (SCPPContext context = new SCPPContext())
            {
                var proyectsList = context.Proyecto.Where(p => p.Inscripción.Count == 0);
                if (proyectsList != null)
                {
                    foreach (Proyecto proyect in proyectsList)
                    {
                        if (proyect != null)
                            proyectsCollection.Add(proyect);
                    }
                }
            }
            ProyectsList.ItemsSource = proyectsCollection;
            DataContext = proyectsCollection;
        }

        private void GetStudents()
        {
            studentsCollection = new ObservableCollection<Estudiante>();
            using (SCPPContext context = new SCPPContext())
            {
                var studentsListWithLessThan2Periods = context.Estudiante.Where(s => s.Inscripción.Count < 2);

                var studentsListAlreadyAttending =
                    context.Estudiante.Join(
                        context.Inscripción,
                        s => s.Matricula,
                        i => i.Matriculaestudiante,
                        (s, i) => new { student = s, inscription = i })
                    .Where(s2 => s2.inscription.Periodo == periodo)
                    .Select(s2 => s2.student);

                var studentsList = studentsListWithLessThan2Periods.Except(studentsListAlreadyAttending);

                if (studentsList != null)
                {
                    foreach (Estudiante student in studentsList)
                    {
                        if (student != null)
                            studentsCollection.Add(student);
                    }
                }
            }
            StudentsList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }

        private void ProyectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            proyectSelected = (Proyecto)dataGrid.SelectedItems[0];
            CheckSelecctions();
        }

        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            studentSelected = (Estudiante)dataGrid.SelectedItems[0];

            selection1.Text = "";
            selection2.Text = "";
            selection3.Text = "";

            LabelStudent.Content = "Selecciones de proyecto: " + studentSelected.Nombre + " " + studentSelected.Apellidopaterno;

            using (SCPPContext context = new SCPPContext())
            {
                var proyectSelections = context.Selecciónproyecto.Where(p => p.Matriculaestudiante == studentSelected.Matricula);
                var count = 1;
                foreach (Selecciónproyecto selection in proyectSelections)
                {
                    switch (count)
                    {
                        case 1:
                            selection1.Text = selection.Proyecto.Nombre;
                            break;

                        case 2:
                            selection2.Text = selection.Proyecto.Nombre;
                            break;

                        case 3:
                            selection3.Text = selection.Proyecto.Nombre;
                            break;
                    }
                    count++;
                }
            }
            CheckSelecctions();
        }
    }
}