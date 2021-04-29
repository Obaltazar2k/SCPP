using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GestionarEstudiante.xaml
    /// </summary>
    public partial class GestionarEstudiante : Page
    {
        private Estudiante actualStudent;
        private ObservableCollection<Inscripción> inscriptionsCollection;
        private Inscripción inscriptionSelected;
        private bool isModifying = false;
        private string _user;

        public GestionarEstudiante(Estudiante student)
        {
            InitializeComponent();
            actualStudent = student;
            FillTextBoxes();
            GetSesion();
            GetInscriptions();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (isModifying)
            {
                FillTextBoxes();
                ItsNotModifying();
            }
            else
            {
                ReturnToPreviousList(new object(), new RoutedEventArgs());
            }
        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar al ESTUDIANTE "
                    + actualStudent.Nombre + " " + actualStudent.Apellidopaterno + " con matricula " + actualStudent.Matricula
                    + "?", "Confirmación", "Si", "No");

                if (confirmation == MessageBoxResult.Yes)
                {
                    Estudiante student;
                    using (SCPPContext context = new SCPPContext())
                    {
                        student = context.Estudiante.FirstOrDefault(s => s.Matricula == actualStudent.Matricula);
                        student.Activo = 0;
                        context.SaveChanges();
                    }
                    actualStudent = student;
                    deleteDone = true;
                }
                else
                    return;

                if (deleteDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.ShowOK("El registro ha sido eliminado con éxito.", "Eliminación", "Finalizar");
                    ReturnToPreviousList(new object(), new RoutedEventArgs());
                }
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                     "Fallo en conexión con la base de datos", "Aceptar");
                //ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            ItsModifying();
        }

        private void FillTextBoxes()
        {
            TextBoxMatricula.Text = actualStudent.Matricula;
            TextBoxName.Text = actualStudent.Nombre;
            TextBoxApellidoPaterno.Text = actualStudent.Apellidopaterno;
            TextBoxApellidoMaterno.Text = actualStudent.Apellidomaterno;
            TextBoxEmail.Text = actualStudent.Correopersonal;
            TextBoxPhone.Text = actualStudent.Telefono;
            TextBoxStatus.Text = actualStudent.Estado;
        }

        private void GetExpedientButton_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarExpediente(inscriptionSelected));
            return;
        }

        private void GetInscriptions()
        {
            inscriptionsCollection = new ObservableCollection<Inscripción>();
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionsList = context.Inscripción.Where(i => i.Matriculaestudiante == actualStudent.Matricula)
                    .Include(i => i.Proyecto)
                    .Include(i => i.Proyecto.Organización)
                    .Include(i => i.Expediente);
                if (inscriptionsList != null)
                {
                    foreach (Inscripción inscription in inscriptionsList)
                    {
                        if (inscription != null)
                            inscriptionsCollection.Add(inscription);
                    }
                }

                var coordinator = context.Coordinador.FirstOrDefault(c => c.Numtrabajador == _user);
                if (coordinator != null)
                    DeleteStudentButton.Visibility = Visibility.Visible;
            }

            ProyectColumn.Binding = new Binding("Proyecto.Nombre");
            OrganizationColumn.Binding = new Binding("Proyecto.Organización.Nombre");

            InscriptionList.ItemsSource = inscriptionsCollection;
            DataContext = inscriptionsCollection;
        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
        }

        private void InscriptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                inscriptionSelected = (Inscripción)dataGrid.SelectedItems[0];
                if (inscriptionSelected != null)
                    GetExpedientButton.IsEnabled = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void ItsModifying()
        {
            isModifying = true;
            GetExpedientButton.Visibility = Visibility.Hidden;
            SaveChangesButton.Visibility = Visibility.Visible;
            InscriptionList.Visibility = Visibility.Hidden;

            // TextBoxMatricula.IsReadOnly = false; La matricula no debería modificarse
            TextBoxName.IsReadOnly = false;
            TextBoxApellidoPaterno.IsReadOnly = false;
            TextBoxApellidoMaterno.IsReadOnly = false;
            TextBoxEmail.IsReadOnly = false;
            TextBoxPhone.IsReadOnly = false;
            TextBoxStatus.IsReadOnly = false;
        }

        private void ItsNotModifying()
        {
            isModifying = false;
            GetExpedientButton.Visibility = Visibility.Visible;
            SaveChangesButton.Visibility = Visibility.Hidden;
            InscriptionList.Visibility = Visibility.Visible;

            InscriptionList.IsEnabled = true;
            InscriptionList.Focusable = true;
            InscriptionList.IsReadOnly = false;

            // TextBoxMatricula.IsReadOnly = true; La matricula no debería modificarse
            TextBoxName.IsReadOnly = true;
            TextBoxApellidoPaterno.IsReadOnly = true;
            TextBoxApellidoMaterno.IsReadOnly = true;
            TextBoxEmail.IsReadOnly = true;
            TextBoxPhone.IsReadOnly = true;
            TextBoxStatus.IsReadOnly = true;
        }

        private void ReturnToPreviousList(object v, RoutedEventArgs routedEventArgs)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (VerificateFields())
            {
                using (SCPPContext context = new SCPPContext())
                {
                    var studentUptdated = UpdateStudent();
                    StudentUpdatedMessage(studentUptdated);
                }
                ItsNotModifying();
            }
        }

        private void StudentUpdatedMessage(object studentUptdated)
        {
            CustomMessageBox.ShowOK("El registro se ha cambiado con éxito", "Cambio exitoso", "Finalizar");
        }

        private void TextBoxPhone_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Tab)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private object UpdateStudent()
        {
            Estudiante student;
            using (SCPPContext context = new SCPPContext())
            {
                student = context.Estudiante.FirstOrDefault(s => s.Matricula == actualStudent.Matricula);

                student.Nombre = TextBoxName.Text;
                student.Apellidopaterno = TextBoxApellidoPaterno.Text;
                student.Apellidomaterno = TextBoxApellidoMaterno.Text;
                student.Telefono = TextBoxPhone.Text;
                student.Correopersonal = TextBoxEmail.Text;
                student.Estado = TextBoxStatus.Text;

                context.SaveChanges();
            }
            actualStudent = student;
            return student;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxEmail.Text)
                && FieldsVerificator.VerificatePhone(TextBoxPhone.Text);
        }
    }
}