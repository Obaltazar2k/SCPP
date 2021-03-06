﻿using MemoryGameService.Utilities;
using SCPP.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using System.Data.Entity.Core;


namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para RegistrarProfesor.xaml
    /// </summary>
    public partial class RegistrarProfesor : Page
    {
        private string password;
        public RegistrarProfesor()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string generatedPassword = GeneratePassword();

            TextBoxPassword.IsEnabled = false;
            TextBoxPassword.Password = password = generatedPassword + TextBoxWorkerNumber.Text;
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

            for(int i = 0; i <= 2; i++)
            {
                var value = random.Next(0, 9);
                password += value;
            }

            return password;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    var foundProfessor = context.Profesor.Find(TextBoxWorkerNumber.Text);
                    if(foundProfessor == null)
                    {
                        if(VerificateFields())
                        {
                            var professorRegistered = RegisterNewProfessor();
                            SendEmail();
                            ProfessorRegisteredMessage(professorRegistered);
                        }
                    }
                    else
                        CustomMessageBox.ShowOK("Ya existe un profesor registrado con la matrícula ingresada con nombre: " +
                            foundProfessor.Nombre, "Profesor ya registrado", "Aceptar");
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }

        }

        private Profesor RegisterNewProfessor()
        {
            Profesor professor = new Profesor
            {
                Nombre = TextBoxName.Text,
                Apellidopaterno = TextBoxLastName.Text,
                Apellidomaterno = TextBoxMothersLastName.Text,
                Numtrabajador = TextBoxWorkerNumber.Text,
                Correopersonal = TextBoxEMail.Text,
                Telefono = TextBoxPhone.Text,
                Contraseña = Encrypt.GetSHA256(TextBoxPassword.Password),
                Activo = 1
            };

            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    context.Profesor.Add(professor);
                    context.SaveChanges();
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }

            return professor;
        }

        private void SendEmail()
        {
            MailTemplate mt = new MailTemplate();
            mt.SetReceiver(TextBoxName.Text + " " + TextBoxLastName.Text, TextBoxEMail.Text);
            mt.SetMessage("Contraseña de SCPP", "Profesor con el numero de trabajador: " + TextBoxWorkerNumber.Text + ", tu contraseña para el SCPP es: " + password);
            try
            {
                mt.Send();
            }
            catch(Exception)
            {

            }
        }

        private void ProfessorRegisteredMessage(Profesor teacher)
        {
            MessageBoxResult selection = CustomMessageBox.ShowYesNo("El registro se ha realizado con exito", "Registro exitoso",
                "Gestionar",
                "Aceptar");
            if(selection == MessageBoxResult.Yes)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new GestionarProfesor(teacher));
                return;
            }
            if(selection == MessageBoxResult.No)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxEMail.Text)
                && FieldsVerificator.VerificatePhone(TextBoxPhone.Text)
                && FieldsVerificator.VerificateName(TextBoxName.Text)
                && FieldsVerificator.VerificateName(TextBoxMothersLastName.Text)
                && FieldsVerificator.VerificateName(TextBoxLastName.Text);
        }
    }
}