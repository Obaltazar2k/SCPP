using SCPP.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SCPP.DataAcces;
using System.Data.Entity.Core;
using WPFCustomMessageBox;
using System.Text.RegularExpressions;
using MaterialDesignThemes.Wpf;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class IniciarSesion : Page
    {
        private static int ALLOWEDATTEMTPS = 5;
        private int _attempts = 0;
        private bool _lockedLogin = false;
        private string _user, _password;
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        public IniciarSesion()
        {
            ITheme theme = _paletteHelper.GetTheme();
            theme.SetSecondaryColor(System.Windows.Media.Color.FromRgb(4, 156, 51)); //verde
            theme.SetPrimaryColor(System.Windows.Media.Color.FromRgb(4, 83, 156)); //azul
            _paletteHelper.SetTheme(theme);

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
                    if (student != null)
                    {
                        if (StudentIsValidated(student))
                        {
                            SetSesion(student.Matricula, student.Correopersonal, "Student");
                            var mainWindow = (MainWindow)Application.Current.MainWindow;
                            mainWindow?.ChangeView(new MenuEstudiante());
                            return;
                        }
                        else
                            CustomMessageBox.Show("El coordinador aun no ha validado tu registro.");
                    }
                    else if (IsProfesor())
                    {
                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                        mainWindow?.ChangeView(new MenuProfesor());
                        return;
                    }
                    else if (IsCoordinator())
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
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                   "Fallo en conexión con la base de datos", "Aceptar");
                Restarter.RestarSCPP();
            }
        }

        private void RegisterButton_Clicked(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarInscripcion());
            return;
        }
        private void SetSesion(string user, string email, string kind)
        {
            Sesion userSesion = Sesion.GetSesion;
            userSesion.Username = user;
            userSesion.Email = email;
            userSesion.Kind = kind;
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
            CustomMessageBox.Show("Usuario no encontrado. Por favor verifique que sus datos sean correctos.");
            UserTextBox.Clear();
            PasswordTextBox.Clear();
            _attempts++;
            if (_attempts == ALLOWEDATTEMTPS)
            {
                _lockedLogin = true;
                LoginButton.IsEnabled = false;
                CustomMessageBox.Show("Ah sobre pasado el numero de intentos disponibles, intente mas tarde");
            }
        }

        private bool FieldsValidation()
        {
            if (!string.IsNullOrEmpty(UserTextBox.Text) && !string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                Regex rgx = new Regex(@"^[a-zA-Z0-9]+$");
                if (rgx.IsMatch(UserTextBox.Text))
                    return true;
                else
                {
                    CustomMessageBox.Show("Campos erróneos. Por favor asegurese de introducir datos alfanumericos en usuario.");
                    UserTextBox.Clear();
                    PasswordTextBox.Clear();
                    return false;
                }
            }
            else
            {
                CustomMessageBox.Show("Campos incompletos. Por favor asegurese de no dejar campos vacíos.");
                return false;
            }
        }

        private void GetDataFromFields()
        {
            _user = UserTextBox.Text;
            _password = Encrypt.GetSHA256(PasswordTextBox.Password);
            Console.WriteLine(_password);
        }

        private bool IsCoordinator()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var coordinador = context.Coordinador.FirstOrDefault(u => u.Numtrabajador == _user);
                if (coordinador != null && coordinador.Contraseña == _password)
                {
                    SetSesion(coordinador.Numtrabajador, coordinador.Correopersonal, "Coordinator");
                    return true;
                }
                else
                    return false;
            }
        }

        private bool IsProfesor()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var profesor = context.Profesor.FirstOrDefault(u => u.Numtrabajador == _user);
                if (profesor != null && profesor.Contraseña == _password)
                {
                    SetSesion(profesor.Numtrabajador, profesor.Correopersonal, "Profesor");
                    return true;
                }
                else
                    return false;
            }
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
    }
}