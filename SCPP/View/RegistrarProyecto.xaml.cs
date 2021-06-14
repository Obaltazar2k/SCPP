using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para RegistrarProyecto.xaml
    /// </summary>
    public partial class RegistrarProyecto : Page
    {
        private readonly List<int> numOfStudents = new List<int> { 1, 2 };
        private readonly DateTime thisDay = DateTime.Today;
        private ObservableCollection<Organización> OrganizationsCollection { get; set; }
        private ObservableCollection<Responsableproyecto> ResponsablesCollection { get; set; }

        public RegistrarProyecto()
        {
            try
            {
                InitializeComponent();
                ComboBoxCapacidad.ItemsSource = numOfStudents;
                FillComboBoxOrganizations();
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ComboBoxOrganizacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var organization = (Organización)ComboBoxOrganizacion.SelectedItem;
            ResponsablesCollection = new ObservableCollection<Responsableproyecto>();
            using (SCPPContext context = new SCPPContext())
            {
                var responsablesList = context.Responsableproyecto.Where(r => r.OrganizaciónID == organization.OrganizaciónID);
                if (responsablesList != null)
                {
                    foreach (Responsableproyecto responsableProyecto in responsablesList)
                    {
                        if (responsableProyecto != null)
                            ResponsablesCollection.Add(responsableProyecto);
                    }
                }
            }
            ComboBoxResponsable.ItemsSource = ResponsablesCollection;
        }

        private void FillComboBoxOrganizations()
        {
            OrganizationsCollection = new ObservableCollection<Organización>();
            using (SCPPContext context = new SCPPContext())
            {
                var organizationsList = context.Organización.OrderByDescending(s => s.Nombre);
                if (organizationsList != null)
                {
                    foreach (Organización organizacion in organizationsList)
                    {
                        if (organizacion != null)
                            OrganizationsCollection.Add(organizacion);
                    }
                }
            }
            ComboBoxOrganizacion.ItemsSource = OrganizationsCollection;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SCPPContext context = new SCPPContext())
                {
                    var proyecto = context.Proyecto.FirstOrDefault(s => s.Nombre == TextBoxNombre.Text);
                    if (proyecto == null)
                    {
                        var proyectRegistered = RegisterNewProyect();
                        MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("El registro se ha realizado con éxito", "Registro exitoso", "Gestionar proyecto", "Finalizar" );
                        if (confirmation == MessageBoxResult.Yes)
                        {
                            var mainWindow = (MainWindow)Application.Current.MainWindow;
                            mainWindow?.ChangeView(new GestionarProyecto(proyecto));
                        }
                        else
                            CancelButton_Click(new object(), new RoutedEventArgs());
                    }
                    else
                        CustomMessageBox.ShowOK("Ya existe un proyecto registrado con el nombre ingresado con clave: " + proyecto.Clave, "Proyecto ya registrado", "Aceptar");
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private object RegisterNewProyect()
        {
            var organization = (Organización)ComboBoxOrganizacion.SelectedItem;
            var responsable = (Responsableproyecto)ComboBoxResponsable.SelectedItem;
            var proyect = new Proyecto
            {
                Actividades = TextBoxActividades.Text,
                Nombre = TextBoxNombre.Text,
                Descripcion = TextBoxDescripción.Text,
                Fecharegistro = thisDay,
                Noestudiantes = 1,
                OrganizaciónID = organization.OrganizaciónID,
                ResponsableproyectoID = responsable.ResponsableproyectoID,
                Activo = 1
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Proyecto.Add(proyect);
                context.SaveChanges();
            }
            return proyect;
        }
    }
}