﻿using MemoryGameService.Utilities;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCustomMessageBox;

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
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxContraseña.IsEnabled = false;
            TextBoxContraseña.Text = "contra" + TextBoxMatricula.Text + "seña";
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBoxContraseña.IsEnabled = true;
            TextBoxContraseña.Text = "";
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
            using (SCPPContext context = new SCPPContext())
            {
                var student = context.Estudiante.Find(TextBoxMatricula.Text);
                if (student == null)
                {
                    if (VerificateFields())
                    {
                        var studentRegistered = RegisterNewStudent();
                        SendEmail();
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
                Matricula = TextBoxMatricula.Text,
                Nombre = TextBoxNombre.Text,
                Apellidopaterno = TextBoxApellidoPaterno.Text,
                Apellidomaterno = TextBoxApellidoMaterno.Text,
                Telefono = TextBoxTelefono.Text,
                Correopersonal = TextBoxCorreo.Text,
                Promedio = Convert.ToDouble(TextBoxPromedio.Text),
                Estado = ComboBoxStates.Text,
                Contraseña = Encrypt.GetSHA256(TextBoxContraseña.Text)
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
            mt.SetMessage("Contraseña de de SCPP", "Estudiante con matricula " + TextBoxMatricula.Text + ", tu contraseña para el SCPP es: " + TextBoxContraseña.Text);
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
            try
            {
                var promedio = Convert.ToDouble(TextBoxPromedio.Text);
                return true;
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowOK("El promedio no tiene el formato correcto", "Error de formato de matricula", "Aceptar");
                MessageBox.Show(ex.ToString());
            }
            return false;
        }
    }
}