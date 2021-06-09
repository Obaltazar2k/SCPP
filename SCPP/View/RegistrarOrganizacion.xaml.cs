using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using SCPP.Utilities;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Collections.Generic;
using System.Data.Entity.Core;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para RegistrarOrganizacion.xaml
    /// </summary>
    public partial class RegistrarOrganizacion : Page
    {
        private readonly List<string> sectorsList = new List<string> { "Transporte", "Comunicaciones", "Comercial", "Turístico" , "Sanitario" , 
            "Educativo" , "Artes" , "Financiero" , "Administrativo", "Tecnológico" };

        public RegistrarOrganizacion()
        {
            InitializeComponent();
            ComboBoxSector.ItemsSource = sectorsList;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void NumbersTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        public void OrganizationRegisteredMessage(Organización organizationRegistered)
        {
            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("El registro se ha realizado con éxito", "Registro exitoso",
                "Gestionar organizacion",
                "Finalizar");
            if (confirmation == MessageBoxResult.Yes)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new GestionarOrganizacion(organizationRegistered));
                return;
            }
            else
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
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
                            Organización organizationRegistered =(Organización) RegisterNewOrganization();
                            OrganizationRegisteredMessage(organizationRegistered);
                        }
                        else
                            CustomMessageBox.ShowOK("Organización ya registrada", "Registro repetido", "Aceptar");    
                    }
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();            
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
                Activo = 1,
                Sector = ComboBoxSector.Text
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Organización.Add(organization);
                context.SaveChanges();
            }
            return organization;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(EmailTextBox.Text)
                && FieldsVerificator.VerificatePhone(PhoneTextBox.Text)
                && FieldsVerificator.VerificatePostalCode(PostcodeTextBox.Text)
                && FieldsVerificator.VerificateNumext(NumextTextBox.Text);
        }
    }
}