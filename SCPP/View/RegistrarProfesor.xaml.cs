using MemoryGameService.Utilities;
using SCPP.Utilities;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para RegistrarProfesor.xaml
    /// </summary>
    public partial class RegistrarProfesor : Page
    {
        private string encryptedPassword;
        private string password;
        public RegistrarProfesor()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string generatedPassword = GeneratePassword();

            TextBoxPassword.IsEnabled = false;
            TextBoxPassword.Password = password = generatedPassword + TextBoxRFC.Text;
            encryptedPassword = Encrypt.GetSHA256(password);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            password = null;
            TextBoxPassword.IsEnabled = true;
            TextBoxPassword.Password = null;
        }

        private string GeneratePassword()
        {
            string password = "";
            var seed = Environment.TickCount;
            var random = new Random(seed);

            for (int i = 0; i <= 2; i++)
            {
                var value = random.Next(0, 9);
                password += value;
            }

            return password;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            using (SCPPContext context = new SCPPContext())
            {
                var foundProfessor = context.Profesor.Find(TextBoxRFC.Text);
                if (foundProfessor == null)
                {
                    if (VerificateFields())
                    {
                        var professorRegistered = RegisterNewTeacher();
                        SendEmail();
                        TeacherRegisteredMessage(professorRegistered);
                    }
                }
                else
                    CustomMessageBox.ShowOK("Ya existe un profesor registrado con la matrícula ingresada con nombre: " +
                        foundProfessor.Nombre, "Profesor ya registrado", "Aceptar");
            }
        }

        private Profesor RegisterNewTeacher()
        {
            Profesor professor = new Profesor();

            professor.Nombre = TextBoxName.Text;
            professor.Apellidopaterno = TextBoxLastName.Text;
            professor.Apellidomaterno = TextBoxMothersLastName.Text;
            professor.Rfc = TextBoxRFC.Text;
            professor.Correopersonal = TextBoxEMail.Text;
            professor.Contraseña = encryptedPassword;

            using (SCPPContext context = new SCPPContext())
            {
                context.Profesor.Add(professor);
                context.SaveChanges();
            }
            return professor;
        }

        private void SendEmail()
        {
            MailTemplate mt = new MailTemplate();
            mt.SetReceiver(TextBoxName.Text + " " + TextBoxLastName.Text, TextBoxEMail.Text);
            mt.SetMessage("Contraseña de SCPP", "Profesor con RFC: " + TextBoxRFC.Text + ", tu contraseña para el SCPP es: " + password);
            try
            {
                mt.Send();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void TeacherRegisteredMessage(Profesor teacher)
        {
            MessageBoxResult selection = CustomMessageBox.ShowYesNo("El registro se ha realizado con exito", "Registro exitoso",
                "Gestionar",
                "Aceptar");
            if (selection == MessageBoxResult.Yes)
            {
                Console.WriteLine("Extiende el CU de administrar Profesor");
            }
            if (selection == MessageBoxResult.No)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }
        private bool ValidateFullFields()
        {
            if (string.IsNullOrEmpty(TextBoxName.Text) || string.IsNullOrEmpty(TextBoxLastName.Text) || string.IsNullOrEmpty(TextBoxMothersLastName.Text)
                || string.IsNullOrEmpty(TextBoxEMail.Text) || string.IsNullOrEmpty(TextBoxRFC.Text)
                || string.IsNullOrEmpty(TextBoxPassword.Password))
            {
                MessageBox.Show("Campos incompletos. Por favor asegurese de no dejar campos vacíos.");
                return false;
            }

            return true;
        }

        private bool VerificateEmail()
        {
            string emailFormat = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(TextBoxEMail.Text, emailFormat))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de introducir un correo valido.", "Error de formato de correo", "Aceptar");
                return false;
            }
        }

        private bool VerificateFields()
        {
            if (!ValidateFullFields())
            {
                return false;
            }

            if (!VerificateEmail())
            {
                return false;
            }

            if (!VerificateRFC())
            {
                return false;
            }

            return true;
        }

        private bool VerificateRFC()
        {
            Regex rgx = new Regex(@"^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([A-Z\d]{3})?$");
            if (rgx.IsMatch(TextBoxRFC.Text))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de ingresar un RFC Valido", "Error de formato de RFC", "Aceptar");
                return false;
            }
        }
    }
}