using SCPP.DataAcces;
using SCPP.Utilities;
using System;
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
    /// Lógica de interacción para GestionarGrupo.xaml
    /// </summary>
    public partial class GestionarGrupo : Page
    {
        private ObservableCollection<Estudiante> studentsCollection;
        private string _user;
        private Grupo actualGroup;
        private bool isModifying = false;
        private bool isCoordinator = false;
        private Estudiante studentSelected = null;


        public GestionarGrupo(Grupo grupo)
        {
            InitializeComponent();
            actualGroup = grupo;
            studentsCollection = new ObservableCollection<Estudiante>();
            FillTextBoxes();
            GetSesion();
            IsCoordinator();
            GetStudents();
        }


        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;

        }

        private void IsCoordinator()
        {

            using(SCPPContext context = new SCPPContext())
            {
                var profesor = context.Profesor.FirstOrDefault(c => c.Numtrabajador.Equals(_user));

                if(profesor != null)
                {
                    isCoordinator = false;
                    ManageButtonProfesor();
                }
                else
                {
                    isCoordinator = true;
                    DeleteGroupButton.Visibility = Visibility.Visible;
                    ManageButtonsCoordinator();
                }

            }
        }

        private void ManageButtonsCoordinator()
        {
            DeleteStudentButton.Visibility = Visibility.Hidden;
        }

        private void ManageButtonProfesor()
        {
            EditGroupButton.Visibility = Visibility.Hidden;
            DeleteStudentButton.Visibility = Visibility.Visible;
            DeleteStudentButton.IsEnabled = false;
        }

        private void GetStudents()
        {
            using(SCPPContext context = new SCPPContext())
            {
                IQueryable<Estudiante> studentsInDB = null;

                var grupo = context.Grupo.Where(a => a.Nrc == actualGroup.Nrc).FirstOrDefault();

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
                    .Where(q => q.join.inscription.GrupoID == grupo.GrupoID)
                    .Select(q => q.student);


                if(studentsInDB != null)
                {
                    foreach(Estudiante student in studentsInDB)
                    {
                        if(student != null)
                            studentsCollection.Add(student);
                    }
                }

                StudentList.ItemsSource = studentsCollection;
                DataContext = studentsCollection;
            }
        }

        private void FillTextBoxes()
        {
            TextBoxNRC.Text = actualGroup.Nrc.ToString();
            TextBoxBloque.Text = actualGroup.Bloque;
            TextBoxCupo.Text = actualGroup.Cupo.ToString();
            TextBoxPeriodo.Text = actualGroup.Periodo;
            TextBoxSeccion.Text = actualGroup.Seccion;

        }

        private void ItsModifying()
        {
            isModifying = true;
            SaveChangesButton.Visibility = Visibility.Visible;

            TextBoxBloque.IsReadOnly = false;
            TextBoxCupo.IsReadOnly = false;
            TextBoxSeccion.IsReadOnly = false;

        }

        private void ItsNotModifying()
        {
            isModifying = false;
            SaveChangesButton.Visibility = Visibility.Hidden;

            TextBoxNRC.IsReadOnly = true;
            TextBoxBloque.IsReadOnly = true;
            TextBoxCupo.IsReadOnly = true;
            TextBoxSeccion.IsReadOnly = true;
        }

        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            ItsModifying();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if(VerificateFields())
            {
                using(SCPPContext context = new SCPPContext())
                {
                    var studentUptdated = UpdateGroup();
                    GroupUpdatedMessage(studentUptdated);
                }
                ItsNotModifying();
            }
        }

        private void GroupUpdatedMessage(object studentUptdated)
        {
            CustomMessageBox.ShowOK("El registro se ha cambiado con éxito", "Cambio exitoso", "Finalizar");
        }

        private object UpdateGroup()
        {
            Grupo group;
            using(SCPPContext context = new SCPPContext())
            {
                group = context.Grupo.FirstOrDefault(s => s.Nrc == actualGroup.Nrc);

                group.Bloque = TextBoxBloque.Text;
                group.Cupo = int.Parse(TextBoxCupo.Text);
                group.Seccion = TextBoxSeccion.Text;

                context.SaveChanges();
            }

            actualGroup = group;
            return group;

        }

        private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar el GRUPO "
                    + " con NRC " + actualGroup.Nrc
                    + "?", "Confirmación", "Si", "No");

                if(confirmation == MessageBoxResult.Yes)
                {
                    Grupo group;
                    using(SCPPContext context = new SCPPContext())
                    {
                        group = context.Grupo.FirstOrDefault(s => s.Nrc == actualGroup.Nrc);
                        context.Grupo.Remove(group);
                        context.SaveChanges();
                    }
                    actualGroup = group;
                    deleteDone = true;
                }
                else
                    return;

                if(deleteDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.ShowOK("El registro ha sido eliminado con éxito.", "Eliminación", "Finalizar");
                    ReturnToPreviousList(new object(), new RoutedEventArgs());
                }
            }
            catch(EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                     "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private void ReturnToPreviousList(object v, RoutedEventArgs routedEventArgs)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(isModifying)
            {
                FillTextBoxes();
                ItsNotModifying();
            }
            else
            {
                ReturnToPreviousList(new object(), new RoutedEventArgs());
            }
        }

        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                studentSelected = (Estudiante)dataGrid.SelectedItems[0];
                if(studentSelected != null)
                    DeleteStudentButton.IsEnabled = true;
            }
            catch(ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateCupo(TextBoxCupo.Text);

        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            bool unasingDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea desasignar al alumno con nombre: "
                    + studentSelected.Nombre
                    + " del grupo?", "Confirmación", "Si", "No");

                if(confirmation == MessageBoxResult.Yes)
                {
                    UnasingStudent();
                    unasingDone = true;
                }
                else
                    return;

                if(unasingDone)
                {
                    MessageBoxResult result = CustomMessageBox.ShowOK("El estudiante se ha quitado del grupo.", "Alumno desasignado", "Finalizar");
                    ReturnToPreviousList(new object(), new RoutedEventArgs());
                }
            }
            catch(EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                     "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private void UnasingStudent()
        {
            try
            {
                using(SCPPContext context = new SCPPContext())
                {

                    var group = context.Grupo.Where(a => a.Nrc == actualGroup.Nrc).FirstOrDefault();

                    Inscripción inscripcion = context.Inscripción.Where(i => i.GrupoID == group.GrupoID
                    && i.Matriculaestudiante == studentSelected.Matricula).FirstOrDefault();

                    inscripcion.GrupoID = null;
                    context.SaveChanges();

                    studentsCollection.Clear();
                    GetStudents();
                }
            }
            catch(EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }
    }
}
