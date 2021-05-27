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
        Reporte actualReporte;
        public CalificarReporte(Reporte reporte)
        {
            InitializeComponent();
            this.actualReporte = reporte;
            FillTextBoxes();
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
  
        private void FillTextBoxes()
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
    }
}
