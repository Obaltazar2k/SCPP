using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCustomMessageBox;
using SCPP.DataAcces;

namespace SCPP
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
            InitializeComponent();
            GetProfesors();
            _period = Period.GetPeriod();
            GetGroups();
        }

        private void AssignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (profesorSelected == null || groupSelected == null)
                return;

            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea asignar al PROFESOR "
                + profesorSelected.Nombre + " " + profesorSelected.Apellidopaterno + " " + profesorSelected.Apellidopaterno + " con Rfc " + profesorSelected.Rfc
                + " al GRUPO " + groupSelected.Nrc + "?", "Confirmación", "Si", "No");
            var assignDone = false;
            if (confirmation == MessageBoxResult.Yes)
            {
                Grupo grupoAsignado = new Grupo();
                grupoAsignado.Bloque = groupSelected.Bloque;
                grupoAsignado.Cupo = groupSelected.Cupo;
                grupoAsignado.Nrc = groupSelected.Nrc;
                grupoAsignado.Seccion = groupSelected.Seccion;
                grupoAsignado.Rfcprofesor = profesorSelected.Rfc;
                grupoAsignado.Periodo = _period;
                using (SCPPContext context = new SCPPContext())
                {
                    context.Grupo.Add(grupoAsignado);
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MenuCoordinador());
            return;
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
                var groupList = context.Grupo.Where(p => p.Rfcprofesor == null && p.Periodo == null);
                var groupAssginedList = context.Grupo.Where(g => g.Periodo != null && g.Periodo.Equals(_period));

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
                    }
                    else
                    {
                        foreach (Grupo grupo in groupList)
                        {
                            if (grupo != null)
                            {
                                groupsCollection.Add(grupo);
                            }
                        }
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
    }
}