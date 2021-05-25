using SCPP.DataAcces;
using SCPP.Utilities;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para EntregarReporte.xaml
    /// </summary>
    public partial class EntregarReporte : Page
    {
        private readonly Inscripción inscription;
        private readonly List<string> kindOfReport = new List<string> { "Mensual", "Temporal", "Excepcional" };
        private OpenFileDialog dialog;

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
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        myStream.CopyTo(ms);
                        byte[] file = ms.ToArray();

                        using (SCPPContext context = new SCPPContext())
                        {
                            Archivo oDocument = new Archivo
                            {
                                ExpedienteID = inscription.Expediente.First().ExpedienteID,
                                Archivo1 = file,
                                Fechaentrega = DateTime.Today,
                                Titulo = dialog.SafeFileName,
                                Validado = 0
                            };

                            Reporte oReport = new Reporte
                            {
                                Horasreportadas = int.Parse(TextBoxHours.Text),
                                Tiporeporte = ComboBoxKind.Text
                            };

                            context.Archivo.Add(oDocument);

                            oReport.Archivo = oDocument;

                            context.Reporte.Add(oReport);
                            context.SaveChanges();
                        }
                        CustomMessageBox.ShowOK("Reporte agregado con éxito.", "Éxito.", "Aceptar");
                        CancelButton_Click(new object(), new RoutedEventArgs());
                    }
                }
                catch (EntityException)
                {
                    CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                   "Fallo en conexión con la base de datos", "Aceptar");
                    Restarter.RestarSCPP();
                }
            }
        }

        private void TextBoxHours_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Tab)
            {
                if (TextBoxHours.Text.Length < 3)
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            else
                e.Handled = true;
        }

        private void UploadReportButton_Click(object sender, RoutedEventArgs e)
        {
            dialog = new OpenFileDialog
            {
                Filter = "Report files|*.pdf;*.doc;*.docs;*.docx",
                FilterIndex = 1,
                InitialDirectory = new KnownFolder(KnownFolderType.Documents).Path,
                RestoreDirectory = true
            };
            dialog.ShowDialog();
            TextBoxPath.Text = dialog.FileName;
        }
    }
}