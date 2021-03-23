using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCustomMessageBox;

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                CustomMessageBox.ShowOK("No hay entrada a la cual volver", "Error al navegar hacía atras", "Aceptar");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FieldsEmptyValidation())
                {
                    if (WrongFieldsValidation())
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
                            {
                                CustomMessageBox.ShowOK("Organización ya registrada", "Registro repetido", "Aceptar");
                            }
                        }
                    }
                    else
                    {
                        CustomMessageBox.ShowOK("Por favor corrobore los datos ingresados", "Campos erróneos", "Aceptar");
                    }
                }
                else
                {
                    CustomMessageBox.ShowOK("Por favor asegurese de no dejar campos vacíos", "Campos incompletos", "Aceptar");
                }
            }catch(Exception ex)
            {
                CustomMessageBox.ShowOK(ex.ToString(), "Error", "Aceptar");
            }
        }

        private bool FieldsEmptyValidation()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) || string.IsNullOrEmpty(EmailTextBox.Text) || 
                string.IsNullOrEmpty(PhoneTextBox.Text) || string.IsNullOrEmpty(StreetTextBox.Text) || 
                string.IsNullOrEmpty(SuburbTextBox.Text) || string.IsNullOrEmpty(NumextTextBox.Text) ||
                string.IsNullOrEmpty(PostcodeTextBox.Text))
            {
                return false;
            }
            return true;
        }

        private bool WrongFieldsValidation()
        {
            return EmailValidation() && PhoneValidation() && NumextValidation() && PostcodeValidation();
        }

        private bool EmailValidation()
        {
            try
            {
                MailAddress correo = new MailAddress(EmailTextBox.Text);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private bool PhoneValidation()
        {
            int phone;
            if (Int32.TryParse(PhoneTextBox.Text, out phone))
            {
                return true;
            }
            return false;
        }

        private bool NumextValidation()
        {
            int numext;
            if (Int32.TryParse(NumextTextBox.Text, out numext))
            {
                return true;
            }
            return false;
        }

        private bool PostcodeValidation()
        {
            int postcode;
            if (Int32.TryParse(PostcodeTextBox.Text, out postcode) && PostcodeTextBox.Text.Length==5)
            {
                return true;
            }
            return false;
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
                Telefono = PhoneTextBox.Text
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Organización.Add(organization);
                context.SaveChanges();
            }
            return organization;
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
    }
}
