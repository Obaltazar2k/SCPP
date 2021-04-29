using Microsoft.Win32;
using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GestionarExpediente.xaml
    /// </summary>
    public partial class GestionarExpediente : Page
    {

        public Inscripción Inscription { get; }
        private string _user;
        private ObservableCollection<Archivo> filesCollection;
        private ObservableCollection<Reporte> reportsCollection;

        public GestionarExpediente(Inscripción inscription)
        {
            InitializeComponent();
            Inscription = inscription;
            FillTextBoxes();
            GetSesion();
            GetDocuments();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void FilesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void FillTextBoxes()
        {
            DateTime InscrpitionDate = (DateTime)Inscription.Fecha;
            InscriptionDateTextBox.Text = InscrpitionDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/');
            KindInscriptionTextBox.Text = Inscription.Tipo;
            PorjectTextBox.Text = Inscription.Proyecto.Nombre;
            OrganizationTextBox.Text = Inscription.Proyecto.Organización.Nombre;

            DateTime InscrpitionStartDate = (DateTime)Inscription.Expediente.First().Fechainiciopp;
            StartDateTextBox.Text = InscrpitionStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/');
            DateTime? InscrpitionEndDate = Inscription.Expediente.First().Fechafinpp;
            EndDateTextBox.Text = InscrpitionEndDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/');

            if (Inscription.Grupo != null)
            {
                BlockTextBox.Text = Inscription.Grupo.Bloque;
                SectionTextBox.Text = Inscription.Grupo.Seccion;
                NRCTextBox.Text = Inscription.Grupo.Nrc.ToString();
                PeriodTextBox.Text = Inscription.Grupo.Periodo;
            }

            NumOfReportsTextBox.Text = Inscription.Expediente.First().Numreportesentregados.ToString();
            TotalHoursTextBox.Text = Inscription.Expediente.First().Horasacumuladas.ToString();
        }

        private void GetDocuments()
        {
            filesCollection = new ObservableCollection<Archivo>();
            reportsCollection = new ObservableCollection<Reporte>();
            var expedientID = Inscription.Expediente.First().ExpedienteID;
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

                var coordinator = context.Coordinador.FirstOrDefault(c => c.Rfc == _user);
                if (coordinator != null)
                    DeleteFileColumn.Visibility = Visibility.Visible;
            }

            //ProyectColumn.Binding = new Binding("Proyecto.Nombre");
            //OrganizationColumn.Binding = new Binding("Proyecto.Organización.Nombre");

            FilesGrid.ItemsSource = filesCollection;
            DataContext = filesCollection;
        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
        }
        private void ReportsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            //Dialogo
            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Todos los archivos (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            dialog.ShowDialog();
            string filePath = dialog.FileName;
            //Console.WriteLine(filePath);

            //Guardar el archivo
            byte[] file = null;
            if (file != null)
            {
                Stream myStream = dialog.OpenFile();
                using (MemoryStream ms = new MemoryStream())
                {
                    myStream.CopyTo(ms);
                    file = ms.ToArray();

                    using (SCPPContext context = new SCPPContext())
                    {
                        Archivo oDocument = new Archivo();
                        oDocument.ExpedienteID = Inscription.Expediente.First().ExpedienteID;
                        //oDocument.Rutaubicación = file;
                        oDocument.Fechaentrega = DateTime.Today;
                        oDocument.Titulo = dialog.SafeFileName;
                        oDocument.Validado = 2;

                        context.Archivo.Add(oDocument);
                        context.SaveChanges();
                    }
                }

                filesCollection.Clear();
                GetDocuments();
            }
        }
    }
}