﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCustomMessageBox;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para AsignarProfesorGrupo.xaml
    /// </summary>
    public partial class AsignarProfesorGrupo : Page
    {
        private readonly DateTime _thisDay = DateTime.Today;
        private string _periodo;
        private Profesor profesorSelected = null;
        private Grupo groupSelected = null;

        public AsignarProfesorGrupo()
        {
            InitializeComponent();
            GetProfesors();
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
                grupoAsignado.Periodo = GetPeriod();
                grupoAsignado.GrupoID = 5;
                using(SCPPContext context = new SCPPContext())
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
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MenuCoordinador());
            return;
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

        private void GetGroups()
        {
            List<Grupo> groupsCollection = new List<Grupo>();
            using (SCPPContext context = new SCPPContext())
            {
                var groupList = context.Grupo.Where(p => p.Rfcprofesor == null).Where(p => p.Periodo == null);
                if (groupList != null)
                {
                    foreach (Grupo grupo in groupList)
                    {
                        if (grupo != null)
                            groupsCollection.Add(grupo);
                    }
                }
            }
            GroupsList.ItemsSource = groupsCollection;
        }

        private void CheckSelecctions()
        {
            if ((profesorSelected != null) && (groupSelected != null))
                AssignButton.IsEnabled = true;
        }

        private void ProfesorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            profesorSelected = (Profesor)dataGrid.SelectedItems[0];
            CheckSelecctions();
        }

        private void GroupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            groupSelected = (Grupo)dataGrid.SelectedItems[0];
            CheckSelecctions();
        }

        private string GetPeriod()
        {
            var year = _thisDay.Year;
            var month = _thisDay.Month;
            string startMonth;
            string endtMonth;
            string startYear;
            string endYear;
            if (month < 7)
            {
                startMonth = "FEB";
                startYear = year.ToString();
                endtMonth = "JUL";
                endYear = startYear;
            }
            else
            {
                startMonth = "AGO";
                startYear = year.ToString();
                endtMonth = "ENE";
                endYear = _thisDay.AddYears(1).ToString();
            }
            return startMonth + startYear + "-" + endtMonth + endYear;
        }
    }
}