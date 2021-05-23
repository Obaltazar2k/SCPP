using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para GestionarProfesor.xaml
    /// </summary>
    public partial class GestionarProfesor : Page
    {
        private Profesor actualProfesor;
        private ObservableCollection<Grupo> groupsCollection;
        private bool isModifying = false;
        private readonly List<string> differentsStates = new List<string> { "Activo", "Inactivo" };

        public GestionarProfesor()
        {
            InitializeComponent();
            ComboBoxState.ItemsSource = differentsStates;           
        }

        public GestionarProfesor(Profesor profesor)
        {
            try
            {
                InitializeComponent();
                ComboBoxState.ItemsSource = differentsStates;
                actualProfesor = profesor;
                FillTextBoxes();
                GetGroups();               
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

        private void DeleteProfesorButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar al PROFESOR "
                    + actualProfesor.Nombre + " " + actualProfesor.Apellidopaterno + " con numero de trabajador:  " + actualProfesor.Numtrabajador
                    + "?", "Confirmación", "Si", "No");

                if (confirmation == MessageBoxResult.Yes)
                {
                    Profesor profesor;
                    using (SCPPContext context = new SCPPContext())
                    {
                        profesor = context.Profesor.FirstOrDefault(s => s.Numtrabajador == actualProfesor.Numtrabajador);
                        profesor.Activo = 0;
                        context.SaveChanges();
                    }
                    actualProfesor = profesor;
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
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private void EditProfesorButton_Click(object sender, RoutedEventArgs e)
        {
            ItsModifying();
        }

        private void FillTextBoxes()
        {
            TextBoxNumeroTrabajador.Text = actualProfesor.Numtrabajador;
            TextBoxName.Text = actualProfesor.Nombre;
            TextBoxApellidoPaterno.Text = actualProfesor.Apellidopaterno;
            TextBoxApellidoMaterno.Text = actualProfesor.Apellidomaterno;
            TextBoxEmail.Text = actualProfesor.Correopersonal;
            TextBoxPhone.Text = actualProfesor.Telefono;
            if(actualProfesor.Activo == 1)
            {
                ComboBoxState.SelectedIndex = 0;
                TextBoxStatus.Text = "Activo";
            }
            else
            {
                ComboBoxState.SelectedIndex = 1;
                TextBoxStatus.Text = "Inactivo";
            }           
        }

        private void GetGroups()
        {
            groupsCollection = new ObservableCollection<Grupo>();
            using (SCPPContext context = new SCPPContext())
            {
                var groupsList = context.Grupo.Where(p => p.Rfcprofesor.Equals(actualProfesor.Numtrabajador));
                if (groupsList != null)
                {
                    foreach (Grupo grupo in groupsList)
                    {
                        if (grupo != null)
                            groupsCollection.Add(grupo);
                    }
                }
            }
            GroupsList.ItemsSource = groupsCollection;
            DataContext = groupsCollection;
        }

        private void ItsModifying()
        {
            isModifying = true;
            SaveChangesButton.Visibility = Visibility.Visible;

            TextBoxName.IsReadOnly = false;
            TextBoxApellidoPaterno.IsReadOnly = false;
            TextBoxApellidoMaterno.IsReadOnly = false;
            TextBoxEmail.IsReadOnly = false;
            TextBoxPhone.IsReadOnly = false;
            ComboBoxState.IsReadOnly = false;
            TextBoxStatus.Visibility = Visibility.Hidden;
            ComboBoxState.Visibility = Visibility.Visible;
        }

        private void ItsNotModifying()
        {
            isModifying = false;
            SaveChangesButton.Visibility = Visibility.Hidden;

            TextBoxName.IsReadOnly = true;
            TextBoxApellidoPaterno.IsReadOnly = true;
            TextBoxApellidoMaterno.IsReadOnly = true;
            TextBoxEmail.IsReadOnly = true;
            TextBoxPhone.IsReadOnly = true;
            ComboBoxState.IsReadOnly = true;
            TextBoxStatus.Visibility = Visibility.Visible;
            TextBoxStatus.Text = ComboBoxState.Text;
            ComboBoxState.Visibility = Visibility.Hidden;
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
            try
            {
                if (VerificateFields())
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        var profesroUpdated = UpdateProfesor();
                        ProfesortUpdatedMessage(profesroUpdated);
                    }
                    ItsNotModifying();
                }
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }           
        }

        private void ProfesortUpdatedMessage(object profesorUpdated)
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

        private object UpdateProfesor()
        {
            Profesor profesor;
            using (SCPPContext context = new SCPPContext())
            {
                profesor = context.Profesor.FirstOrDefault(s => s.Numtrabajador == actualProfesor.Numtrabajador);

                profesor.Nombre = TextBoxName.Text;
                profesor.Apellidopaterno = TextBoxApellidoPaterno.Text;
                profesor.Apellidomaterno = TextBoxApellidoMaterno.Text;
                profesor.Telefono = TextBoxPhone.Text;
                profesor.Correopersonal = TextBoxEmail.Text;
                if (ComboBoxState.Text.Equals("Activo"))
                {
                    profesor.Activo = 1;
                }
                else
                {
                    profesor.Activo = 0;
                }
                

                context.SaveChanges();
            }
            actualProfesor = profesor;
            return profesor;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxEmail.Text)
                && FieldsVerificator.VerificatePhone(TextBoxPhone.Text)
                && FieldsVerificator.VerificateName(TextBoxName.Text)
                && FieldsVerificator.VerificateName(TextBoxApellidoPaterno.Text)
                && FieldsVerificator.VerificateName(TextBoxApellidoPaterno.Text);
        }

        public void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }
    }
}
