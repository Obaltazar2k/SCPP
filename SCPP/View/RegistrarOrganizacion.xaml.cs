using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para RegistrarOrganizacion.xaml
    /// </summary>
    public partial class RegistrarOrganizacion : Page
    {
        public RegistrarOrganizacion()
        {
            InitializeComponent();
        }

        public void OrganizationRegisteredMessage(object organization)
        {
            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("El registro se ha realizado con éxito", "Registro exitoso",
                "Gestionar organizacion",
                "Finalizar");
            if (confirmation == MessageBoxResult.Yes)
            {
                throw new NotImplementedException();
            }
            else
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }

        public object RegisterNewOrganization()
        {
            var organization = new Organización
            {
                Calle = StreetTextBox.Text,
                Codigopostal = PostcodeTextBox.Text,
                Colonia = SuburbTextBox.Text,
                Correo = EmailTextBox.Text,
                Nombre = NameTextBox.Text,
                Numext = Int32.Parse(NumextTextBox.Text),
                Telefono = PhoneTextBox.Text,
                Activo = 1
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Organización.Add(organization);
                context.SaveChanges();
            }
            return organization;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MenuCoordinador());
            return;
        }

        private bool EmailValidation()
        {
            string emailFormat = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(EmailTextBox.Text, emailFormat))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de introducir un correo valido.", "Error de formato de correo", "Aceptar");
                return false;
            }
        }

        private void NumbersTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private bool NumextValidation()
        {
            int numext;
            if (Int32.TryParse(NumextTextBox.Text, out numext))
            {
                return true;
            }
            else
            {
                CustomMessageBox.ShowOK("Asegurese de introducir un número exterior valido.", "Error de formato de número exterior", "Aceptar");
                return false;
            }
        }

        private bool PostcodeValidation()
        {
            int postcode;
            if (Int32.TryParse(PostcodeTextBox.Text, out postcode) && PostcodeTextBox.Text.Length == 5)
            {
                return true;
            }
            else
            {
                CustomMessageBox.ShowOK("Asegurese de introducir un código postal valido.", "Error de formato de código postal", "Aceptar");
                return false;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerificateFields())
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        var organization = context.Organización.FirstOrDefault(s => s.Nombre == NameTextBox.Text);
                        if (organization == null)
                        {
                            var organizationRegistered = RegisterNewOrganization();
                            OrganizationRegisteredMessage(organizationRegistered);
                        }
                        else
                            CustomMessageBox.ShowOK("Organización ya registrada", "Registro repetido", "Aceptar");    
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowOK(ex.ToString(), "Error", "Aceptar");
            }
        }
        private bool VerificateFields()
        {
            return EmailValidation() && NumextValidation() && PostcodeValidation();
        }
    }
}