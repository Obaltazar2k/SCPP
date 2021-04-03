﻿using SCPP.Utilities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCustomMessageBox;
using SCPP.DataAcces;
using System.Data.Entity.Core;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para Desasignar_profesor_a_grupo.xaml
    /// </summary>
    public partial class DesasignarProfesorGrupo : Page
    {
        private string _period;
        private Grupo groupSelected = null;
        private Profesor profesorSelected = null;
        
        public DesasignarProfesorGrupo()
        {
            try
            {
                InitializeComponent();
                GetProfesors();
                _period = Period.GetPeriod();
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                Loaded += ReturnToLogin;
            }           
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MenuCoordinador());
            return;
        }

        private void CheckSelecctions()
        {
            if ((profesorSelected != null) && (groupSelected != null))
                DesassignButton.IsEnabled = true;
            else
                DesassignButton.IsEnabled = false;
        }

        private void DesassignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (profesorSelected == null || groupSelected == null)
                    return;

                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea desasignar al PROFESOR "
                    + profesorSelected.Nombre + " " + profesorSelected.Apellidopaterno + " " + profesorSelected.Apellidopaterno + " con Rfc " + profesorSelected.Rfc
                    + " del GRUPO " + groupSelected.Nrc + "?", "Confirmación", "Si", "No");
                var assignDone = false;
                if (confirmation == MessageBoxResult.Yes)
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        context.Grupo.Attach(groupSelected);
                        context.Entry(groupSelected).State = EntityState.Deleted;
                        context.SaveChanges();
                        assignDone = true;
                    }
                }

                if (assignDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.Show("La desasignación ha sido realizada con éxito.");
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow?.ChangeView(new MenuCoordinador());
                    return;
                }
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }
        private void GetGroups()
        {
            List<Grupo> groupsCollection = new List<Grupo>();
            using (SCPPContext context = new SCPPContext())
            {
                var groupsList = context.Grupo.Where(p => p.Rfcprofesor.Equals(profesorSelected.Rfc) && p.Periodo.Equals(_period));
                if (groupsList != null)
                {
                    foreach (Grupo grupo in groupsList)
                    {
                        if (grupo != null)
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
                var profesorsList = context.Profesor.Where(p => p.Grupo.Count >= 1);
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

            if (groupSelected == null)
            {
                groupSelected = (Grupo)dataGrid.SelectedItems[0];
            }
            CheckSelecctions();
        }

        private void ProfesorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (groupSelected != null)
                {
                    GroupsList.SelectedIndex = -1;
                    groupSelected = null;
                }

                DataGrid dataGrid = sender as DataGrid;
                profesorSelected = (Profesor)dataGrid.SelectedItems[0];
                CheckSelecctions();

                GetGroups();
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
                ReturnToLogin(new object(), new RoutedEventArgs());
            }
        }

        public void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }
    }
}