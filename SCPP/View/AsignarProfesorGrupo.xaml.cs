using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using System.Data.Entity.Core;
using SCPP.View;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para AsignarProfesorGrupo.xaml
    /// </summary>
    public partial class AsignarProfesorGrupo : Page
    {
        private string _period;
        private Grupo groupSelected = null;
        private Profesor profesorSelected = null;
        
        public AsignarProfesorGrupo()
        {
            try
            {
                InitializeComponent();
                GetProfesors();
                _period = Period.GetPeriod();
                GetGroups();
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                Loaded += ReturnToLogin;
            }           
        }

        private void AssignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (profesorSelected == null || groupSelected == null)
                    return;

                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea asignar al PROFESOR "
                    + profesorSelected.Nombre + " " + profesorSelected.Apellidopaterno + " " + profesorSelected.Apellidomaterno + " con Numero de trabajador: " + profesorSelected.Numtrabajador
                    + " al GRUPO " + groupSelected.Nrc + "?", "Confirmación", "Si", "No");
                var assignDone = false;
                if (confirmation == MessageBoxResult.Yes)
                {
                    Grupo grupo;
                    using (SCPPContext context = new SCPPContext())
                    {
                        grupo = context.Grupo.FirstOrDefault(s => s.GrupoID == groupSelected.GrupoID);
                        grupo.Rfcprofesor = profesorSelected.Numtrabajador;
                        grupo.Estado = "Asignado";
                        context.SaveChanges();
                    }

                    assignDone = true;
                }
                else
                    return;

                if (assignDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.Show("La asignación ha sido realizada con éxito.");
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow?.ChangeView(new MenuCoordinador());
                    return;
                }
            }
            catch(EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CheckSelecctions()
        {
            if ((profesorSelected != null) && (groupSelected != null))
                AssignButton.IsEnabled = true;
        }

        private void GetGroups()
        {
            List<Grupo> groupsCollection = new List<Grupo>();
            using (SCPPContext context = new SCPPContext())
            {
                var groupList = context.Grupo.Where(p => p.Rfcprofesor == null && p.Periodo == _period && p.Estado.Equals("Disponible"));
                /*var groupAssginedList = context.Grupo.Where(g => g.Periodo != null && g.Periodo.Equals(_period));

                if (groupList != null)
                {
                    if (groupAssginedList != null)
                    {
                        foreach (Grupo grupo in groupList)
                        {
                            bool alreadyAssigned = false;
                            foreach (Grupo grupoAsignado in groupAssginedList)
                            {
                                Console.WriteLine(grupoAsignado.Nrc);
                                if (grupo != null && (grupo.Nrc.Equals(grupoAsignado.Nrc)))
                                {
                                    alreadyAssigned = true;
                                }
                            }

                            if (alreadyAssigned == false)
                            {
                                groupsCollection.Add(grupo);
                            }
                        }
                    }*/
                    foreach (Grupo grupo in groupList)
                    {
                        if (grupo != null)
                        {
                            groupsCollection.Add(grupo);
                        }
                    }               
            }
            GroupsList.ItemsSource = groupsCollection;
        }

        private void GetProfesors()
        {
            List<Profesor> profesorsCollection = new List<Profesor>();
            using (SCPPContext context = new SCPPContext())
            {
                var profesorsList = context.Profesor.ToList();
                if (profesorsList != null)
                {
                    foreach (Profesor profesor in profesorsList)
                    {
                        if (profesor != null)
                            profesorsCollection.Add(profesor);
                    }
                }
            }
            ProfesorsList.ItemsSource = profesorsCollection;
        }
        private void GroupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            groupSelected = (Grupo)dataGrid.SelectedItems[0];
            CheckSelecctions();
        }

        private void ProfesorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            profesorSelected = (Profesor)dataGrid.SelectedItems[0];
            CheckSelecctions();
        }

        public void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }
    }
}