using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para VerReportes.xaml
    /// </summary>
    public partial class VerReportes : Page
    {
        private readonly Estudiante student;
        private Reporte reportSelected;
        private Inscripción inscription;
        private Expediente expediente;
        private ObservableCollection<Reporte> reportsCollection;
        public VerReportes(Estudiante student)
        {
            InitializeComponent();
            this.student = student;
            reportsCollection = new ObservableCollection<Reporte>();
            GetStudentActualInscription();
            GetStudentActualExpediente();
            GetReports();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void GetStudentActualInscription()
        {
            using (SCPPContext context = new SCPPContext())
            {
                inscription = context.Inscripción.FirstOrDefault(i => i.Matriculaestudiante == student.Matricula && i.Estatus.Equals("Cursando"));
            }
        }

        private void GetStudentActualExpediente()
        {
            using(SCPPContext context = new SCPPContext())
            {
                expediente = context.Expediente.FirstOrDefault(i => i.InscripciónID == inscription.InscripciónID);
            }
        }

        private void GetReports()
        {

            using(SCPPContext context = new SCPPContext())
            {
                var reportsList = context.Reporte.Where(r => r.Archivo.ExpedienteID == expediente.ExpedienteID).Include(r => r.Archivo);
                if(reportsList != null)
                {
                    foreach(Reporte report in reportsList)
                    {
                        if(report != null)
                            reportsCollection.Add(report);
                    }
                }
            }

            ReportsList.ItemsSource = reportsCollection;
            DataContext = this;
        }

        private void GradeReportButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new CalificarReporte(reportSelected));
            return;
        }
        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                reportsCollection.Clear();
                GetStudentActualInscription();
                GetStudentActualExpediente();
                GetReports();
                GradeReportButton.IsEnabled = false;
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }

        }

        private void ReportsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                reportSelected = (Reporte)dataGrid.SelectedItems[0];
                if(reportSelected != null)
                {
                    GradeReportButton.IsEnabled = true;
                }
            }
            catch(ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }
    }
}
