using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para RegistrarInscripcion.xaml
    /// </summary>
    public partial class RegistrarInscripcion : Page
    {
        private readonly List<string> genderList = new List<string> { "Masculino", "Femenino" };

        public RegistrarInscripcion()
        {
            InitializeComponent();
            ComboBoxGender.ItemsSource = genderList;
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
            try
            {
                if (VerificateFields())
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        var student = context.Estudiante.Find(TextBoxEnrrollment.Text);
                        if (student == null)
                        {
                            var studentRegistered = RegisterNewStudent();
                            StudentRegisteredMessage(studentRegistered);
                        }
                        else
                            CustomMessageBox.ShowOK("Ya existe un estudiante registrado con la matrícula ingresada con nombre: " + student.Nombre, "Estudiante ya registrado", "Aceptar");
                    }
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
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
                Genero = ComboBoxGender.Text,
                Activo = 1,
                Contraseña = Encrypt.GetSHA256(TextBoxPassword.Password)
            };

            try
            {
                using (SCPPContext context = new SCPPContext())
                {
                    context.Estudiante.Add(student);
                    context.SaveChanges();
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }

            return student;
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
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

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxEMail.Text)
                && FieldsVerificator.VerificateMatricula(TextBoxEnrrollment.Text)
                && FieldsVerificator.VerificatePromedio(TextBoxAverage.Text)
                && FieldsVerificator.VerificatePhone(TextBoxPhone.Text)
                && FieldsVerificator.VerificateName(TextBoxName.Text)
                && FieldsVerificator.VerificateName(TextBoxLastName.Text)
                && FieldsVerificator.VerificateName(TextBoxMothersLastName.Text);
        }
    }
}