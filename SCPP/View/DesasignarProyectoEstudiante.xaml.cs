using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para DesasignarProyectoEstudiante.xaml
    /// </summary>
    public partial class DesasignarProyectoEstudiante : Page
    {
        private readonly DateTime thisDay = DateTime.Today;
        private string periodo;
        private ObservableCollection<Inscripción> inscriptionsCollection;
        private Inscripción inscriptionSelected = null;

        public DesasignarProyectoEstudiante()
        {
            InitializeComponent();
            AddInformationToLabels();
            GetInscriptions(periodo);
        }

        private void GetInscriptions(string periodo)
        {
            inscriptionsCollection = new ObservableCollection<Inscripción>();
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptions = context.Inscripción.Include(i => i.Estudiante).Include(i => i.Proyecto).Where(i => i.Periodo.Equals(periodo));

                if (inscriptions != null)
                {
                    foreach (Inscripción inscription in inscriptions)
                    {
                        if (inscription != null)
                            inscriptionsCollection.Add(inscription);
                    }
                }
            }
            InscriptionsList.ItemsSource = inscriptionsCollection;
            DataContext = inscriptionsCollection;
        }

        private void AddInformationToLabels()
        {
            LabelFecha.Content += thisDay.ToString("d");
            periodo = Period.GetPeriod();
            LabelPeriodo.Content += periodo;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void UnassignButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (inscriptionSelected == null)
                    return;

                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea desasignar al ESTUDIANTE "
                    + inscriptionSelected.Estudiante.Nombre + " " + inscriptionSelected.Estudiante.Apellidopaterno + " con matricula " + inscriptionSelected.Estudiante.Matricula
                    + " del PROYECTO " + inscriptionSelected.Proyecto.Nombre + "?", "Confirmación", "Si", "No");
                var unassignDone = false;
                if (confirmation == MessageBoxResult.Yes)
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        if (inscriptionSelected.Grupo == null)
                        {
                            var expediente = context.Expediente.Include(ex => ex.Archivo).Where(ex => ex.Inscripción.InscripciónID.Equals(inscriptionSelected.InscripciónID)).First();
                            if (expediente.Archivo.Count.Equals(0))
                            {
                                context.Expediente.Remove(context.Expediente.First(ex => ex.InscripciónID.Equals(inscriptionSelected.InscripciónID)));
                                context.Inscripción.Remove(context.Inscripción.First(i => i.InscripciónID.Equals(inscriptionSelected.InscripciónID)));
                                context.SaveChanges();
                                inscriptionsCollection.Remove(inscriptionSelected);
                            }
                            else
                            {
                                MessageBoxResult result = CustomMessageBox.ShowOK("Hay archivos en el expediente de la inscripción del estudiante por lo que es imposible desasignar al proyecto, trate de nuevo despues de eliminar los archivos y reportes en el expediente.", "Estudiante asignado a un grupo", "Aceptar");
                                return;
                            }
                        } 
                        else
                        {
                            MessageBoxResult result = CustomMessageBox.ShowOK("La inscripción del estudiante se encuantra asignada a un grupo por lo que es imposible desasignar al proyecto, trate de nuevo despues de desasignar al estudiante al grupo.", "Estudiante asignado a un grupo", "Aceptar");
                            return;
                        }
                    }
                    unassignDone = true;
                }
                else
                    return;

                if (unassignDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.ShowYesNo("La desasignación ha sido realizada con éxito.", "Desasignación", "Desasignar otro estudiante", "Finalizar");
                    if (result == MessageBoxResult.Yes)
                    {
                        //extend CU Generar oficio de asignación
                    }
                    else
                    {
                        CancelButton_Click(new object(), new RoutedEventArgs());
                    }
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void InscriptionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                inscriptionSelected = (Inscripción)dataGrid.SelectedItems[0];
                CheckSelecctions();
            }
            catch(ArgumentOutOfRangeException)
            {
                inscriptionSelected = null;
            }
        }

        private void CheckSelecctions()
        {
            if (inscriptionSelected != null)
                UnassignButton.IsEnabled = true;
        }
    }
}
