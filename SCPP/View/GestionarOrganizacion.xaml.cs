using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GestionarOrganizacion.xaml
    /// </summary>
    public partial class GestionarOrganizacion : Page
    {
        private readonly List<string> sectorsList = new List<string> { "Transporte", "Comunicaciones", "Comercial", "Turístico" , "Sanitario" ,
            "Educativo" , "Artes" , "Financiero" , "Administrativo", "Tecnológico" };
        private Organización actualOrganization;
        private ObservableCollection<Proyecto> projectsCollection;
        private bool isModifying = false;
        private string _user;

        public GestionarOrganizacion(Organización organization)
        {
            try
            {
                InitializeComponent();
                actualOrganization = organization;
                ComboBoxSector.ItemsSource = sectorsList;
                FillTextBoxes();
                GetSesion();
                GetProjects();
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (isModifying)
            {
                FillTextBoxes();
                ItsNotModifying();
            }
            else
            {
                ReturnToPreviousList(new object(), new RoutedEventArgs());
            }
        }

        private void DeleteOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar a la ORGANIZACIÓN " + actualOrganization.Nombre + "?", "Confirmación", "Si", "No");

                if (confirmation == MessageBoxResult.Yes)
                {
                    Organización organization;
                    using (SCPPContext context = new SCPPContext())
                    {
                        organization = context.Organización.FirstOrDefault(o => o.OrganizaciónID == actualOrganization.OrganizaciónID);
                        organization.Activo = 0;
                        context.SaveChanges();
                    }
                    actualOrganization = organization;
                    deleteDone = true;
                }
                else
                    return;

                if (deleteDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.ShowOK("El registro ha sido eliminado con éxito.", "Eliminación", "Finalizar");
                    ReturnToPreviousList(new object(), new RoutedEventArgs());
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void EditOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxSector.Text = TextBoxSector.Text;
            ItsModifying();
        }

        private void FillTextBoxes()
        {
            TextBoxName.Text = actualOrganization.Nombre;
            TextBoxEmail.Text = actualOrganization.Correo;
            TextBoxPhone.Text = actualOrganization.Telefono;
            TextBoxSector.Text = actualOrganization.Sector;
            TextBoxStreet.Text = actualOrganization.Calle;
            TextBoxNumext.Text = actualOrganization.Numext.ToString();
            TextBoxSuburb.Text = actualOrganization.Colonia;
            TextBoxPostcode.Text = actualOrganization.Codigopostal;
        }

        private void GetProjects()
        {
            projectsCollection = new ObservableCollection<Proyecto>();
            using (SCPPContext context = new SCPPContext())
            {
                var projectList = context.Proyecto.Where(p => p.OrganizaciónID == actualOrganization.OrganizaciónID);
                if (projectList != null)
                {
                    foreach (Proyecto project in projectList)
                    {
                        if (project != null)
                            projectsCollection.Add(project);
                    }
                }
            }
            ProjectsList.ItemsSource = projectsCollection;
            DataContext = projectsCollection;
        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
        }


        private void ItsModifying()
        {
            isModifying = true;
            SaveChangesButton.Visibility = Visibility.Visible;

            TextBoxName.IsReadOnly = false;
            TextBoxEmail.IsReadOnly = false;
            TextBoxPhone.IsReadOnly = false;
            TextBoxSector.Visibility = Visibility.Hidden;
            ComboBoxSector.Visibility = Visibility.Visible;
            TextBoxStreet.IsReadOnly = false;
            TextBoxNumext.IsReadOnly = false;
            TextBoxSuburb.IsReadOnly = false;
            TextBoxPostcode.IsReadOnly = false;
        }

        private void ItsNotModifying()
        {
            isModifying = false;
            SaveChangesButton.Visibility = Visibility.Hidden;

            TextBoxName.IsReadOnly = true;
            TextBoxEmail.IsReadOnly = true;
            TextBoxPhone.IsReadOnly = true;
            TextBoxSector.Visibility = Visibility.Visible;
            ComboBoxSector.Visibility = Visibility.Hidden;
            TextBoxStreet.IsReadOnly = true;
            TextBoxNumext.IsReadOnly = true;
            TextBoxSuburb.IsReadOnly = true;
            TextBoxPostcode.IsReadOnly = true;
        }

        private void OrganizationUpdatedMessage(object organizationUpdated)
        {
            CustomMessageBox.ShowOK("El registro se ha cambiado con éxito", "Cambio exitoso", "Finalizar");
        }

        private void ReturnToPreviousList(object v, RoutedEventArgs routedEventArgs)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerificateFields())
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        var organizationUpdated = UpdateOrganization();
                        OrganizationUpdatedMessage(organizationUpdated);
                    }
                    FillTextBoxes();
                    ItsNotModifying();
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void TextBoxPhone_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Tab)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private object UpdateOrganization()
        {
            Organización organization;
            using (SCPPContext context = new SCPPContext())
            {
                organization = context.Organización.FirstOrDefault(o => o.OrganizaciónID == actualOrganization.OrganizaciónID);

                organization.Calle = TextBoxStreet.Text;
                organization.Codigopostal = TextBoxPostcode.Text;
                organization.Colonia = TextBoxSuburb.Text;
                organization.Correo = TextBoxEmail.Text;
                organization.Nombre = TextBoxName.Text;
                organization.Numext = Int32.Parse(TextBoxNumext.Text);
                organization.Telefono = TextBoxPhone.Text;
                organization.Sector = ComboBoxSector.Text;

                context.SaveChanges();
            }
            actualOrganization = organization;
            return organization;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxEmail.Text)
                && FieldsVerificator.VerificatePhone(TextBoxPhone.Text)
                && FieldsVerificator.VerificatePostalCode(TextBoxPostcode.Text)
                && FieldsVerificator.VerificateNumext(TextBoxNumext.Text);
        }
    }
}
