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
    /// Lógica de interacción para GestionarProyecto.xaml
    /// </summary>
    public partial class GestionarProyecto : Page
    {
        private readonly List<int> numOfStudents = new List<int> { 1, 2 };
        private Proyecto actualProject;
        private ObservableCollection<Organización> organizationsCollection;
        private ObservableCollection<Responsableproyecto> responsablesCollection;
        private ObservableCollection<Estudiante> studentsCollection;
        private bool isModifying = false;
        private Responsableproyecto actualResponsableProyecto;

        public GestionarProyecto(Proyecto project)
        {
            try
            {
                InitializeComponent();
                actualProject = project;
                ComboBoxCapacidad.ItemsSource = numOfStudents;
                FillComboBoxOrganizations();
                FillTextBoxes();
                GetStudents();
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

        private void ComboBoxOrganization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var organization = (Organización)ComboBoxOrganization.SelectedItem;
                responsablesCollection = new ObservableCollection<Responsableproyecto>();
                using (SCPPContext context = new SCPPContext())
                {
                    var responsablesList = context.Responsableproyecto.Where(r => r.OrganizaciónID == organization.OrganizaciónID);
                    if (responsablesList != null)
                    {
                        foreach (Responsableproyecto responsableProyecto in responsablesList)
                        {
                            if (responsableProyecto != null)
                                responsablesCollection.Add(responsableProyecto);
                        }
                    }
                }
                ComboBoxResponsable.ItemsSource = responsablesCollection;
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void DeleteOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar el PROYECTO " + actualProject.Nombre + "?", "Confirmación", "Si", "No");

                if (confirmation == MessageBoxResult.Yes)
                {
                    Proyecto project;
                    using (SCPPContext context = new SCPPContext())
                    {
                        project = context.Proyecto.FirstOrDefault(p => p.Clave == actualProject.Clave);
                        project.Activo = 0;
                        context.SaveChanges();
                    }
                    actualProject = project;
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
            ComboBoxCapacidad.Text = TextBoxCapacidad.Text;
            ComboBoxResponsable.Text = TextBoxResponsable.Text;
            ComboBoxOrganization.Text = TextBoxOrganization.Text;
            ItsModifying();
        }

        private void FillComboBoxOrganizations()
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
            ComboBoxOrganization.ItemsSource = organizationsCollection;
        }

        private void FillTextBoxes()
        {
            TextBoxName.Text = actualProject.Nombre;
            TextBoxDescription.Text = actualProject.Descripcion;
            TextBoxCapacidad.Text = actualProject.Noestudiantes.ToString();
            TextBoxActividades.Text = actualProject.Actividades;
            TextBoxResponsable.Text = GetProjectResponsable();
            TextBoxOrganization.Text = GetProjectOrganization();
        }

        private string GetProjectResponsable()
        {
            string nombre = null;
            Responsableproyecto responsable;
            using (SCPPContext context = new SCPPContext())
            {
                responsable = context.Responsableproyecto.FirstOrDefault(r => r.ResponsableproyectoID == actualProject.ResponsableproyectoID);
                if (responsable != null)
                {
                    nombre = responsable.Nombre + " " + responsable.Apellidopaterno + " " + responsable.Apellidomaterno;
                    actualResponsableProyecto = responsable;
                }
            }
            return nombre;
        }

        private string GetProjectOrganization()
        {
            string nombre = null;
            Organización organization;
            using (SCPPContext context = new SCPPContext())
            {
                organization = context.Organización.FirstOrDefault(o => o.OrganizaciónID == actualProject.OrganizaciónID);
                if (organization != null)
                {
                    nombre = organization.Nombre;
                }
            }
            return nombre;
        }

        private void GetStudents()
        {
            studentsCollection = new ObservableCollection<Estudiante>();
            using (SCPPContext context = new SCPPContext())
            {
                var students = context.Estudiante.Join(
                    context.Inscripción,
                    e => e.Matricula,
                    i => i.Matriculaestudiante,
                    (e, i) => new { e, i })
                    .Where(x => x.i.Claveproyecto == actualProject.Clave)
                    .Select(x => x.e);
                if (students != null)
                {
                    foreach (Estudiante estudiante in students)
                    {
                        if (estudiante != null)
                            studentsCollection.Add(estudiante);
                    }
                }
            }
            StudentsList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }

        private void ItsModifying()
        {
            isModifying = true;
            SaveChangesButton.Visibility = Visibility.Visible;

            TextBoxName.IsReadOnly = false;
            TextBoxDescription.IsReadOnly = false;
            TextBoxCapacidad.Visibility = Visibility.Hidden;
            ComboBoxCapacidad.Visibility = Visibility.Visible;
            TextBoxActividades.IsReadOnly = false;
            TextBoxResponsable.Visibility = Visibility.Hidden;
            ComboBoxResponsable.Visibility = Visibility.Visible;
            TextBoxOrganization.Visibility = Visibility.Hidden;
            ComboBoxOrganization.Visibility = Visibility.Visible;
        }

        private void ItsNotModifying()
        {
            isModifying = false;
            SaveChangesButton.Visibility = Visibility.Hidden;

            TextBoxName.IsReadOnly = true;
            TextBoxDescription.IsReadOnly = true;
            TextBoxCapacidad.Visibility = Visibility.Visible;
            ComboBoxCapacidad.Visibility = Visibility.Hidden;
            TextBoxActividades.IsReadOnly = true;
            TextBoxResponsable.Visibility = Visibility.Visible;
            ComboBoxResponsable.Visibility = Visibility.Hidden;
            TextBoxOrganization.Visibility = Visibility.Visible;
            ComboBoxOrganization.Visibility = Visibility.Hidden;
        }

        private void ProjectUpdatedMessage(object projectUpdated)
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
                using (SCPPContext context = new SCPPContext())
                {
                    var projectUpdated = UpdateProject();
                    ProjectUpdatedMessage(projectUpdated);
                }
                FillTextBoxes();
                ItsNotModifying();
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private object UpdateProject()
        {
            Proyecto project;
            using (SCPPContext context = new SCPPContext())
            {
                var organization = (Organización)ComboBoxOrganization.SelectedItem;
                Responsableproyecto responsable;
                if (ComboBoxResponsable.SelectedItem == null)
                {
                    responsable = actualResponsableProyecto;
                }
                else
                    responsable = (Responsableproyecto)ComboBoxResponsable.SelectedItem;

                project = context.Proyecto.FirstOrDefault(p => p.Clave == actualProject.Clave);

                project.Actividades = TextBoxActividades.Text;
                project.Descripcion = TextBoxDescription.Text;
                project.Noestudiantes = Int32.Parse(ComboBoxCapacidad.Text);
                project.Nombre = TextBoxName.Text;
                project.OrganizaciónID = organization.OrganizaciónID;
                project.ResponsableproyectoID = responsable.ResponsableproyectoID;

                context.SaveChanges();
            }
            actualProject = project;
            return project;
        }
    }
}
