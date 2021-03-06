﻿using MemoryGameService.Utilities;
using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para RegistrarEstudiante.xaml
    /// </summary>
    public partial class RegistrarEstudiante : Page
    {
        private readonly List<string> differentsStates = new List<string> { "Inscrito", "Preinscrito" };
        private readonly List<string> genderList = new List<string> { "Masculino", "Femenino" };

        public RegistrarEstudiante()
        {
            InitializeComponent();
            ComboBoxGender.ItemsSource = genderList;
            ComboBoxStates.ItemsSource = differentsStates;
            ComboBoxStates.SelectedIndex = 0;
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
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Tab)
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
                Restarter.RestarSCPP();
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
                Genero = ComboBoxGender.Text,
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
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new GestionarEstudiante(student));
                return;
            }
            if (confirmation == MessageBoxResult.Cancel)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxCorreo.Text)
                && FieldsVerificator.VerificateMatricula(TextBoxMatricula.Text)
                && FieldsVerificator.VerificatePromedio(TextBoxPromedio.Text)
                && FieldsVerificator.VerificateName(TextBoxNombre.Text)
                && FieldsVerificator.VerificateName(TextBoxApellidoPaterno.Text)
                && FieldsVerificator.VerificateName(TextBoxApellidoMaterno.Text)
                && FieldsVerificator.VerificatePhone(TextBoxTelefono.Text);
        }
    }
}