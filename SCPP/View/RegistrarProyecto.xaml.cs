using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using System.Data.Entity.Core;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para RegistrarProyecto.xaml
    /// </summary>
    public partial class RegistrarProyecto : Page
    {
        private readonly List<int> numOfStudents = new List<int> { 1, 2 };
        private readonly DateTime thisDay = DateTime.Today;
        
        public RegistrarProyecto()
        {
            try
            {
                InitializeComponent();
                ComboBoxCapacidad.ItemsSource = numOfStudents;
                DataContext = this;
                FillComboBox();
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                        "Fallo en conexión con la base de datos", "Aceptar");
                Loaded += ReturnToLogin;
            }
        }

        private ObservableCollection<Organización> organizationsCollection { get; set; }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void FillComboBox()
        {
            organizationsCollection = new ObservableCollection<Organización>();
            using (SCPPContext context = new SCPPContext())
            {
                var organizationsList = context.Organización.OrderByDescending(s => s.Nombre);

                if (organizationsList != null)
                {
                    foreach (Organización organizacion in organizationsList)
                    {
                        if (organizacion != null)
                            organizationsCollection.Add(organizacion);
                    }
                }
            }
            ComboBoxOrganizacion.ItemsSource = organizationsCollection;
            DataContext = organizationsCollection;
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
                        StudentRegisteredMessage(proyectRegistered);
                    }
                    else
                        CustomMessageBox.ShowOK("Ya existe un proyecto registrado con el nombre ingresado con clave: " + proyecto.Clave, "Proyecto ya registrado", "Aceptar");
                }
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                     "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        private object RegisterNewProyect()
        {
            var organization = (Organización)ComboBoxOrganizacion.SelectedItem;
            var proyect = new Proyecto
            {
                Actividades = TextBoxActividades.Text,
                Nombre = TextBoxNombre.Text,
                Descripcion = TextBoxDescripción.Text,
                Fecharegistro = thisDay,
                Noestudiantes = 1,
                OrganizaciónID = organization.OrganizaciónID,
                Resbonsablenombre = TextBoxResponsable.Text
            };
            using (SCPPContext context = new SCPPContext())
            {
                context.Proyecto.Add(proyect);
                context.SaveChanges();
            }
            return proyect;
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }

        private void StudentRegisteredMessage(object proyectRegistered)
        {
            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("El registro se ha realizado con éxito", "Registro exitoso",
                "Gestionar proyecto",
                "Finalizar"
                );
            if (confirmation == MessageBoxResult.Yes)
            {
                //extend CU Gestionar proyecto
            }
            if (confirmation == MessageBoxResult.No)
            {
                CancelButton_Click(new object(), new RoutedEventArgs());
            }
        }
    }
}