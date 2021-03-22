using System;
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
        private int ALLOWEDATTEMTPS = 5;
        public IniciarSesion()
        {
            InitializeComponent();
        }

        private void LoginButton_Clicked(object sender, RoutedEventArgs e)
        {
            /*var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new AsignarProyectoEstudiante());
            return;*/
            
            try
            {
                if (FieldsValidation() && _lockedLogin == false)
                {
                    GetDataFromFields();
                    using (SCPPContext context = new SCPPContext())
                    {
                        var student = context.Estudiante.FirstOrDefault(u => u.Matricula == _user);
                        if ((student != null) && student.Contraseña == _password)
                        {
                            if (student.Estado.Equals("Preinscrito"))
                            {
                                MessageBox.Show("El coordinador aun no ha validado tu registro.");
                            }
                            else
                            {
                                var mainWindow = (MainWindow)Application.Current.MainWindow;
                                mainWindow?.ChangeView(new MenuEstudiante());
                                return;
                            }
                        }
                        else
                        {
                            var profesor = context.Profesor.FirstOrDefault(u => u.Rfc == _user);
                            if (profesor != null && profesor.Contraseña == _password)
                            {
                                var mainWindow = (MainWindow)Application.Current.MainWindow;
                                mainWindow?.ChangeView(new MenuProfesor());
                                return;
                            }
                            else
                            {
                                var coordinador = context.Coordinador.FirstOrDefault(u => u.Rfc == _user);
                                if (coordinador != null && coordinador.Contraseña == _password)
                                {
                                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                                    mainWindow?.ChangeView(new MenuCoordinador());
                                    return;
                                }
                                else
                                {
                                    MessageBox.Show("Usuario no encontrado. Por favor verifique que sus datos sean correctos.");
                                    UserTextBox.Clear();
                                    PasswordTextBox.Clear();
                                    _attempts++;
                                    if(_attempts == ALLOWEDATTEMTPS)
                                    {
                                        _lockedLogin = true;
                                        LoginButton.IsEnabled = false;
                                        MessageBox.Show("Ah sobre pasado el numero de intentos disponibles, intente mas tarde");
                                    }
                                }
                            }
                        }

                    }
                }
                        
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }                           
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
    }
}
