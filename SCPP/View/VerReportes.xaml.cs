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
        private ObservableCollection<Reporte> reportsCollection;
        public VerReportes(Estudiante student)
        {
            InitializeComponent();
            this.student = student;
            GetStudentActualInscription();
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

        private void GetReports()
        {
            /*reportsCollection = new ObservableCollection<Reporte>();
            var expedienteID = inscription.Expediente.First().ExpedienteID;
            using (SCPPContext context = new SCPPContext())
            {
                var reportsList = context.Reporte.Where(r => r.Archivo.ExpedienteID == expedienteID).Include(r => r.Archivo);
                if (reportsList != null)
                {
                    foreach (Reporte report in reportsList)
                    {
                        if (report != null)
                            reportsCollection.Add(report);
                    }
                }
            }

            ReportsList.ItemsSource = reportsCollection;
            DataContext = this;*/
        }

        private void GradeReportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReportsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
