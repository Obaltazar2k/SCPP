﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SCPP.Utilities;


namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class IniciarSesion : Page
    {
        private string _user, _password;
        private bool _lockedLogin = false;
        private int _attempts = 0;
        private static int ALLOWEDATTEMTPS = 5;
        public IniciarSesion()
        {
            InitializeComponent();
        }

        private void LoginButton_Clicked(object sender, RoutedEventArgs e)
        {           
            try
            {
                if (FieldsValidation() && _lockedLogin == false)
                {
                    GetDataFromFields();
                    var student = IsStudent();
                    if(student != null)
                    {
                        if (StudentIsValidated(student))
                        {
                            SetSesion(student.Matricula, student.Correopersonal);
                            var mainWindow = (MainWindow)Application.Current.MainWindow;
                            mainWindow?.ChangeView(new MenuEstudiante());
                            return;
                        }else
                            MessageBox.Show("El coordinador aun no ha validado tu registro.");
                    }else if(IsProfesor())
                    {
                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                        mainWindow?.ChangeView(new MenuProfesor());
                        return;
                    }else if (IsCoordinator())
                    {
                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                        mainWindow?.ChangeView(new MenuCoordinador());
                        return;
                    }
                    else
                    {
                        FailedAttempt();
                    }                    
                }
                        
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }                           
        }

        private void RegisterButton_Clicked(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarInscripcion());
            return;
        }

        private void GetDataFromFields()
        {
            _user = UserTextBox.Text;
            _password = Encrypt.GetSHA256(PasswordTextBox.Password);
        }

        private bool FieldsValidation()
        {
            if (string.IsNullOrEmpty(UserTextBox.Text) || string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                MessageBox.Show("Campos incompletos. Por favor asegurese de no dejar campos vacíos.");
                return false;
            }
            return true;
        }

        private Estudiante IsStudent()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var student = context.Estudiante.FirstOrDefault(u => u.Matricula == _user);
                if ((student != null) && student.Contraseña == _password)
                {
                    return student;
                }
                else
                    return null;
            }
        }

        private bool IsProfesor()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var profesor = context.Profesor.FirstOrDefault(u => u.Rfc == _user);
                if (profesor != null && profesor.Contraseña == _password)
                {
                    SetSesion(profesor.Rfc, profesor.Correopersonal);
                    return true;
                }
                else
                    return false;
            }
        }

        private bool IsCoordinator()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var coordinador = context.Coordinador.FirstOrDefault(u => u.Rfc == _user);
                if (coordinador != null && coordinador.Contraseña == _password)
                {
                    SetSesion(coordinador.Rfc, coordinador.Correopersonal);
                    return true;
                }
                else
                    return false;
            }
        }

        private bool StudentIsValidated(Estudiante student)
        {
            if (student.Estado.Equals("Preinscrito"))
                return false;
            
            else
                return true;
        }

        private void FailedAttempt()
        {
            MessageBox.Show("Usuario no encontrado. Por favor verifique que sus datos sean correctos.");
            UserTextBox.Clear();
            PasswordTextBox.Clear();
            _attempts++;
            if (_attempts == ALLOWEDATTEMTPS)
            {
                _lockedLogin = true;
                LoginButton.IsEnabled = false;
                MessageBox.Show("Ah sobre pasado el numero de intentos disponibles, intente mas tarde");
            }
        }

        private void SetSesion(string user, string email)
        {
            Sesion userSesion = Sesion.GetSesion;
            userSesion.Username = user;
            userSesion.Email = email;
        }
    }
}
