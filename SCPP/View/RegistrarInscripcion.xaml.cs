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
    /// Lógica de interacción para RegistrarInscripcion.xaml
    /// </summary>
    public partial class RegistrarInscripcion : Page
    {
        public RegistrarInscripcion()
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            using (SCPPContext context = new SCPPContext())
            {
                var student = context.Estudiante.Find(TextBoxEnrrollment.Text);
                if (student == null)
                {
                    if (VerificateFields())
                    {
                        var studentRegistered = RegisterNewStudent();

                        StudentRegisteredMessage(studentRegistered);
                    }
                }
                else
                    CustomMessageBox.ShowOK("Ya existe un estudiante registrado con la matrícula ingresada con nombre: " + student.Nombre, "Estudiante ya registrado", "Aceptar");
            }
        }

        private Estudiante RegisterNewStudent()
        {
            var student = new Estudiante
            {
                Matricula = TextBoxEnrrollment.Text,
                Nombre = TextBoxName.Text,
                Apellidopaterno = TextBoxLastName.Text,
                Apellidomaterno = TextBoxMothersLastName.Text,
                Telefono = TextBoxPhone.Text,
                Correopersonal = TextBoxEMail.Text,
                Promedio = Convert.ToDouble(TextBoxAverage.Text),
                Estado = "Preinscrito",
                Contraseña = Encrypt.GetSHA256(TextBoxPassword.Password)
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Estudiante.Add(student);
                context.SaveChanges();
            }
            return student;
        }

        private void StudentRegisteredMessage(Estudiante student)
        {
            MessageBoxResult confirmation = CustomMessageBox.ShowOK("Podrás ingresar al sistema una vez se valide tu inscripcion",
                "Registro exitoso",
                "Aceptar");
            if (confirmation == MessageBoxResult.OK)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }

        private bool ValidatePhone()
        {
            Regex rgx = new Regex(@"^\+?[\d- ]{9,}$");
            if (rgx.IsMatch(TextBoxPhone.Text))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de ingresar un numero de telefono correcto", "Error de formato de telefono", "Aceptar");
                return false;
            }
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
            return VerificateEmail() && VerificateMatricula() && VerificatePromedio() && ValidatePhone();
        }

        private bool VerificateMatricula()
        {
            Regex rgx = new Regex(@"^[S]\d{7}[a-zA-Z0-9]$");
            if (rgx.IsMatch(TextBoxEnrrollment.Text))
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
                average = Convert.ToDouble(TextBoxAverage.Text);
                IsDouble = true;
            }
            catch (Exception ex)
            {
            }

            Regex rgx = new Regex(@"^((\d+)((\.\d{1,2})?))$");
            if (rgx.IsMatch(TextBoxAverage.Text) && average <= 10 && IsDouble)
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