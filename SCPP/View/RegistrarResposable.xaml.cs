using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
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

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para RegistrarResposable.xaml
    /// </summary>
    public partial class RegistrarResposable : Page
    {
        public RegistrarResposable()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    var foundResposable = context.Responsableproyecto.Where(s => s.Correopersonal.Equals(TextBoxEMail.Text)).FirstOrDefault();
                    if(foundResposable == null)
                    {
                        if(VerificateFields())
                        {
                            var resposableRegistered = RegisterNewResposable();
                            ResposableRegisteredMessage(resposableRegistered);
                        }
                    }
                    else
                        CustomMessageBox.ShowOK("Ya existe un resposable registrado con el correo ingresado: " +
                            foundResposable.Nombre, "Resposable ya registrado", "Aceptar");
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }

        }

        private void ResposableRegisteredMessage(Responsableproyecto resposable)
        {
            MessageBoxResult selection = CustomMessageBox.ShowYesNo("El registro se ha realizado con exito", "Registro exitoso",
                "Gestionar",
                "Aceptar");
            if(selection == MessageBoxResult.Yes)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new GestionarResposable(resposable));
                return;
            }
            if(selection == MessageBoxResult.No)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }

        private Responsableproyecto RegisterNewResposable()
        {
            Responsableproyecto responsable = new Responsableproyecto
            {
                Nombre = TextBoxName.Text,
                Apellidopaterno = TextBoxLastName.Text,
                Apellidomaterno = TextBoxMothersLastName.Text,
                Correopersonal = TextBoxEMail.Text,
                Telefono = TextBoxPhone.Text,
                Activo = 1
            };

            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    context.Responsableproyecto.Add(responsable);
                    context.SaveChanges();
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }

            return responsable;
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxEMail.Text)
                && FieldsVerificator.VerificatePhone(TextBoxPhone.Text)
                && FieldsVerificator.VerificateName(TextBoxName.Text)
                && FieldsVerificator.VerificateName(TextBoxMothersLastName.Text)
                && FieldsVerificator.VerificateName(TextBoxLastName.Text);
        }
    }
}
