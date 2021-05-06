using SCPP.DataAcces;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para EntregarReporte.xaml
    /// </summary>
    public partial class EntregarReporte : Page
    {
        private readonly List<string> kindOfReport = new List<string> { "Mensual", "Temporal", "Excepcional" };
        private OpenFileDialog dialog;
        private Inscripción inscription;

        public EntregarReporte(Inscripción inscription)
        {
            InitializeComponent();
            ComboBoxKind.ItemsSource = kindOfReport;
            this.inscription = inscription;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] file = null;
            Stream myStream = null;
            try
            {
                myStream = dialog.OpenFile();
            }
            catch (InvalidOperationException)
            {
                // Por si cancela el dialogo
            }

            if (myStream != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    myStream.CopyTo(ms);
                    file = ms.ToArray();

                    using (SCPPContext context = new SCPPContext())
                    {
                        Archivo oDocument = new Archivo();
                        oDocument.ExpedienteID = inscription.Expediente.First().ExpedienteID;
                        oDocument.Archivo1 = file;
                        oDocument.Fechaentrega = DateTime.Today;
                        oDocument.Titulo = dialog.SafeFileName;
                        oDocument.Validado = 0;

                        Reporte oReport = new Reporte();
                        oReport.Horasreportadas = int.Parse(TextBoxHours.Text);
                        oReport.Tiporeporte = ComboBoxKind.Text;

                        context.Archivo.Add(oDocument);

                        oReport.Archivo = oDocument;

                        context.Reporte.Add(oReport);
                        context.SaveChanges();
                    }
                    CustomMessageBox.ShowOK("Reporte agregado con éxito.", "Éxito.", "Aceptar");
                    CancelButton_Click(new object(), new RoutedEventArgs());
                }
            }
        }

        private void UploadReportButton_Click(object sender, RoutedEventArgs e)
        {
            dialog = new OpenFileDialog
            {
                Filter = "Todos los archivos (*.*)|*.*",
                FilterIndex = 1,
                InitialDirectory = new KnownFolder(KnownFolderType.Documents).Path,
                RestoreDirectory = true
            };
            dialog.ShowDialog();
            TextBoxPath.Text = dialog.FileName;
        }
    }
}