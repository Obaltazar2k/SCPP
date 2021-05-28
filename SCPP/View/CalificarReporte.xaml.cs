using Microsoft.Win32;
using SCPP.DataAcces;
using SCPP.Utilities;
using Syroot.Windows.IO;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para CalificarReporte.xaml
    /// </summary>
    public partial class CalificarReporte : Page
    {
        private Estudiante student;
        private Reporte actualReporte;
        private Inscripción inscription;
        private Expediente expediente;
        private ObservableCollection<Reporte> reportsCollection;

        public CalificarReporte(Estudiante student)
        {
            InitializeComponent();
            this.student = student;
            reportsCollection = new ObservableCollection<Reporte>();
            GetStudentActualInscription();
            GetStudentActualExpediente();
            GetReports();
            
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                if(actualReporte != null)
                {
                    string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path;
                    string folder = downloadsPath + "\\PracticasProfesionales\\";
                    string fullFilePath = folder + actualReporte.Archivo.Titulo;

                    if(!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    if(File.Exists(fullFilePath))
                    {
                        int count = 1;

                        string fileNameOnly = Path.GetFileNameWithoutExtension(fullFilePath);
                        string extension = Path.GetExtension(fullFilePath);
                        string path = Path.GetDirectoryName(fullFilePath);
                        string newFullPath = fullFilePath;

                        while(File.Exists(newFullPath))
                        {
                            string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                            newFullPath = Path.Combine(path, tempFileName + extension);
                        }
                        fullFilePath = newFullPath;
                    }
                    File.WriteAllBytes(fullFilePath, actualReporte.Archivo.Archivo1);
                }
                CustomMessageBox.ShowOK("Archivo descargado con éxito.", "Éxito.", "Aceptar");
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }
  
        private void FillTextBoxes(Reporte actualReporte)
        {
            KindTextBox.Text = actualReporte.Tiporeporte;
            HoursTextBox.Text = actualReporte.Horasreportadas.ToString();
            DateTextBox.Text = actualReporte.Archivo.Fechaentrega.ToString();
            FileButton.Content = actualReporte.Archivo.Titulo;
        }

        private void GradeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(VerificateFields())
                {
                    using(SCPPContext context = new SCPPContext())
                    {
                        var studentUptdated = UpdateReport();
                        CalificacionRegisteredMessage();
                    }
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private object UpdateReport()
        {
            Reporte reporte;
            using(SCPPContext context = new SCPPContext())
            {
                reporte = context.Reporte.FirstOrDefault(s => s.ReporteID == actualReporte.ReporteID);
                reporte.Calificacion = Convert.ToDouble(ScoreTextBox.Text);
                reporte.Comentario = TextBoxComments.Text;
                context.SaveChanges();
            }
            return reporte;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateGrade(ScoreTextBox.Text);
               
        }

        private void CalificacionRegisteredMessage()
        {
            MessageBoxResult selection = CustomMessageBox.ShowOK("Calificacion registrada con exito", "Calificacion registrada",
                "Aceptar");

            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ReportsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                actualReporte = (Reporte)dataGrid.SelectedItems[0];
                FillTextBoxes(actualReporte);
            }
            catch(ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void GetStudentActualInscription()
        {
            using(SCPPContext context = new SCPPContext())
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
    }
}
