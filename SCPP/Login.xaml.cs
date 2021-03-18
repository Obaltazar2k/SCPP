using System.Windows;


namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private string _user, _password;
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButtonClicked (object sender, RoutedEventArgs e)
        {
            if (FieldsValidation())
            {
                MessageBox.Show("Vamos bien");
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
