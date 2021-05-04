using Microsoft.Win32;
using SCPP.DataAcces;
using SCPP.Utilities;
using Syroot.Windows.IO;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GestionarExpediente.xaml
    /// </summary>
    public partial class GestionarExpediente : Page
    {
        private static Sesion userSesion;
        private ObservableCollection<Archivo> filesCollection;
        private Archivo fileSelected;
        private Inscripción inscription;
        private bool pageIsLoad = false;
        private ObservableCollection<Reporte> reportsCollection;

        public GestionarExpediente(Inscripción inscription)
        {
            pageIsLoad = false;
            InitializeComponent();
            this.inscription = inscription;
            FillTextBoxes();
            GetSesion();
            ChangeComponentsVisibility();
            GetDocuments();
            Loaded += GestionarExpediente_Loaded;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            pageIsLoad = false;
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ChangeComponentsVisibility()
        {
            if (userSesion.Kind == "Student")
            {
                FileValidationColumn.IsReadOnly = true;
                ReportValidationColumn.IsReadOnly = true;
            }
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            fileSelected = ((FrameworkElement)sender).DataContext as Archivo;

            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que deseas eliminar el archivo <" + fileSelected.Titulo + ">?", "Confirmación", "Si", "No");
            if (confirmation == MessageBoxResult.No)
                return;

            if (fileSelected != null)
            {
                using (SCPPContext context = new SCPPContext())
                {
                    context.Archivo.Remove(context.Archivo.Find(fileSelected.ArchivoID));
                    context.SaveChanges();

                    filesCollection.Remove(fileSelected);
                }
            }
            CustomMessageBox.ShowOK("Archivo eliminado con éxito.", "Éxito.", "Aceptar");
        }

        private void DownloadFileButton_Click(object sender, RoutedEventArgs e)
        {
            fileSelected = ((FrameworkElement)sender).DataContext as Archivo;

            if (fileSelected != null)
            {
                string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path;
                string folder = downloadsPath + "\\PracticasProfesionales\\";
                string fullFilePath = folder + fileSelected.Titulo;

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (File.Exists(fullFilePath))
                {
                    int count = 1;

                    string fileNameOnly = Path.GetFileNameWithoutExtension(fullFilePath);
                    string extension = Path.GetExtension(fullFilePath);
                    string path = Path.GetDirectoryName(fullFilePath);
                    string newFullPath = fullFilePath;

                    while (File.Exists(newFullPath))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        newFullPath = Path.Combine(path, tempFileName + extension);
                    }
                    fullFilePath = newFullPath;
                }
                File.WriteAllBytes(fullFilePath, fileSelected.Archivo1);
            }
            CustomMessageBox.ShowOK("Archivo descargado con éxito.", "Éxito.", "Aceptar");
        }

        private void FillTextBoxes()
        {
            DateTime InscrpitionDate = (DateTime)inscription.Fecha;
            InscriptionDateTextBox.Text = InscrpitionDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/');
            KindInscriptionTextBox.Text = inscription.Tipo;
            PorjectTextBox.Text = inscription.Proyecto.Nombre;
            OrganizationTextBox.Text = inscription.Proyecto.Organización.Nombre;

            DateTime InscrpitionStartDate = (DateTime)inscription.Expediente.First().Fechainiciopp;
            StartDateTextBox.Text = InscrpitionStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/');
            DateTime? InscrpitionEndDate = inscription.Expediente.First().Fechafinpp;
            EndDateTextBox.Text = InscrpitionEndDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/');

            if (inscription.Grupo != null)
            {
                BlockTextBox.Text = inscription.Grupo.Bloque;
                SectionTextBox.Text = inscription.Grupo.Seccion;
                NRCTextBox.Text = inscription.Grupo.Nrc.ToString();
                PeriodTextBox.Text = inscription.Grupo.Periodo;
            }

            NumOfReportsTextBox.Text = inscription.Expediente.First().Numreportesentregados.ToString();
            TotalHoursTextBox.Text = inscription.Expediente.First().Horasacumuladas.ToString();
        }

        private void GestionarExpediente_Loaded(object sender, RoutedEventArgs e)
        {
            pageIsLoad = true;
        }

        private void GetDocuments()
        {
            filesCollection = new ObservableCollection<Archivo>();
            reportsCollection = new ObservableCollection<Reporte>();
            var expedientID = inscription.Expediente.First().ExpedienteID;
            using (SCPPContext context = new SCPPContext())
            {
                var filesList = context.Archivo.Where(f => f.ExpedienteID == expedientID);

                if (filesList != null)
                {
                    foreach (Archivo file in filesList)
                    {
                        if (file != null)
                            filesCollection.Add(file);
                    }
                }
            }

            FilesGrid.ItemsSource = filesCollection;
            DataContext = filesCollection;
        }

        private void GetSesion()
        {
            userSesion = Sesion.GetSesion;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (pageIsLoad && userSesion.Kind != "Student")
            {
                fileSelected = ((FrameworkElement)sender).DataContext as Archivo;

                if (fileSelected != null)
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        var fileInDB = context.Archivo.Find(fileSelected.ArchivoID);
                        if (fileInDB.Validado == 1)
                            fileInDB.Validado = 0;
                        else
                            fileInDB.Validado = 1;
                        context.SaveChanges();
                    }
                }
            }
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Todos los archivos (*.*)|*.*",
                FilterIndex = 1,
                InitialDirectory = new KnownFolder(KnownFolderType.Documents).Path,
                RestoreDirectory = true
            };
            dialog.ShowDialog();
            string filePath = dialog.FileName;
            Console.WriteLine(filePath);
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

                        context.Archivo.Add(oDocument);
                        context.SaveChanges();

                        filesCollection.Add(oDocument);
                    }
                    CustomMessageBox.ShowOK("Archivo agregado con éxito.", "Éxito.", "Aceptar");
                }
            }
        }

        private void UploadReportButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}