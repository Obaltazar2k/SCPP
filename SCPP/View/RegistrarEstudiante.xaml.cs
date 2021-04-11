using MemoryGameService.Utilities;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using System.Data.Entity.Core;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para RegistrarEstudiante.xaml
    /// </summary>
    public partial class RegistrarEstudiante : Page
    {
        private readonly List<string> differentsStates = new List<string> { "Inscrito", "Preinscrito" };

        public RegistrarEstudiante()
        {
            InitializeComponent();
            ComboBoxStates.ItemsSource = differentsStates;
            ComboBoxStates.SelectedIndex = 0;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MenuCoordinador());
            return;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxContraseña.IsEnabled = false;
            TextBoxContraseña.Password = "contra" + TextBoxMatricula.Text + "seña";
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBoxContraseña.IsEnabled = true;
            TextBoxContraseña.Password = "";
        }

        private void NumbersTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerificateFields())
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        var student = context.Estudiante.Find(TextBoxMatricula.Text);
                        if (student == null)
                        {
                            var studentRegistered = RegisterNewStudent();
                            SendEmail();
                            StudentRegisteredMessage(studentRegistered);
                        }
                        else
                            CustomMessageBox.ShowOK("Ya existe un estudiante registrado con la matrícula ingresada con nombre: " + student.Nombre, "Estudiante ya registrado", "Aceptar");
                    }
                }
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                     "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private Estudiante RegisterNewStudent()
        {
            var student = new Estudiante
            {
                Matricula = TextBoxMatricula.Text,
                Nombre = TextBoxNombre.Text,
                Apellidopaterno = TextBoxApellidoPaterno.Text,
                Apellidomaterno = TextBoxApellidoMaterno.Text,
                Telefono = TextBoxTelefono.Text,
                Correopersonal = TextBoxCorreo.Text,
                Promedio = Convert.ToDouble(TextBoxPromedio.Text),
                Estado = ComboBoxStates.Text,
                Activo = 1,
                Contraseña = Encrypt.GetSHA256(TextBoxContraseña.Password)
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Estudiante.Add(student);
                context.SaveChanges();
            }
            return student;
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }

        private void SendEmail()
        {
            MailTemplate mt = new MailTemplate();
            mt.SetReceiver(TextBoxNombre.Text + " " + TextBoxApellidoPaterno.Text, TextBoxCorreo.Text);
            mt.SetMessage("Contraseña de de SCPP", "Estudiante con matricula " + TextBoxMatricula.Text + ", tu contraseña para el SCPP es: " + TextBoxContraseña.Password);
            try
            {
                mt.Send();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void StudentRegisteredMessage(Estudiante student)
        {
            MessageBoxResult confirmation = CustomMessageBox.ShowYesNoCancel("El registro se ha realizado con éxito", "Registro exitoso",
                "Asignar proyecto a estudiante",
                "Gestionar estudiante",
                "Finalizar");
            if (confirmation == MessageBoxResult.Yes)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new AsignarProyectoEstudiante(student));
                return;
            }
            if (confirmation == MessageBoxResult.No)
            {
                Console.WriteLine("Extiende CU Gestionar estudiante");
            }
            if (confirmation == MessageBoxResult.Cancel)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }

        private bool VerificateEmail()
        {
            string emailFormat = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(TextBoxCorreo.Text, emailFormat))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de introducir un correo valido.", "Error de formato de correo", "Aceptar");
                return false;
            }
        }

        private bool VerificateFields()
        {
            return VerificateEmail() && VerificateMatricula() && VerificatePromedio();
        }

        private bool VerificateMatricula()
        {
            Regex rgx = new Regex(@"^[S]\d{7}[a-zA-Z0-9]$");
            if (rgx.IsMatch(TextBoxMatricula.Text))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese que la matricula es una S seguida de 8 números", "Error de formato de matricula", "Aceptar");
                return false;
            }
        }

        private bool VerificatePromedio()
        {
            double average = 0;
            bool IsDouble = false;

            try
            {
                average = Convert.ToDouble(TextBoxPromedio.Text);
                IsDouble = true;
            }
            catch (Exception ex)
            {
            }

            Regex rgx = new Regex(@"^((\d+)((\.\d{1,2})?))$");
            if (rgx.IsMatch(TextBoxPromedio.Text) && average <= 10 && IsDouble)
            {
                return true;
            }
            else
            {
                CustomMessageBox.ShowOK("El promedio no tiene el formato correcto", "Error de formato de promedio", "Aceptar");
                return false;
            }
        }
    }
}