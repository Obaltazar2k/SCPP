using System;
using System.Linq;
using System.Windows;
using SCPP.Utilities;


namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class IniciarSesion : Window
    {
        private string _user, _password;
        public IniciarSesion()
        {
            InitializeComponent();
        }

        private void LoginButtonClicked (object sender, RoutedEventArgs e)
        {
            try
            {
                if (FieldsValidation())
                {
                    GetDataFromFields();
                    using(SCPPContext context = new SCPPContext())
                    {
                        if ((_user.Substring(0, 1)).Equals("S"))
                        {
                            var student = context.Estudiante.FirstOrDefault(u => u.Matricula == _user);
                            if((student != null))
                            {
                                MessageBox.Show(student.Nombre + " " + student.Apellidopaterno + " " + student.Apellidomaterno);
                            }
                            else
                            {
                                MessageBox.Show("Usuario no encontrado. Por favor verifique que sus datos sean correctos.");
                            }                            
                        }
                        else
                        {
                            var profesor = context.Profesor.FirstOrDefault(u => u.Rfc == _user);
                            if ((profesor != null))
                            {
                                MessageBox.Show(profesor.Nombre + " " + profesor.Apellidopaterno + " " + profesor.Apellidomaterno);
                            }
                            else
                            {
                                MessageBox.Show("Usuario no encontrado. Por favor verifique que sus datos sean correctos.");
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
            _password = PasswordTextBox.Password;
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
