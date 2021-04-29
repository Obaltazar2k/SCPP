using OfficeOpenXml;
using SCPP.DataAcces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GenerarReporte.xaml
    /// </summary>
    public partial class GenerarReporte : Page
    {
        private readonly List<string> sectorsList = new List<string> { "Trasporte"};

        private static string selectedPath;

        private static bool exitReport = false;
        public GenerarReporte()
        {
            InitializeComponent();
        }

        private void GenerateReportButton_Clicked(object sender, RoutedEventArgs e)
        {
            GenerateExcelReport();
        }

        private void SelectPathButton_Clicked(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if(folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = folderBrowser.SelectedPath + @"\Reporte.xlsx";
                TextBoxPath.Text = selectedPath;
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private static async Task GenerateExcelReport()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(selectedPath);

            await SaveExcelFile(file);
            if (exitReport)
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowOK("El reporte se ha generado con éxito", "Reporte exitoso",
                "Finalizar");
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow?.ChangeView(new MenuCoordinador());
                return;
            }
            else{
                MessageBoxResult confirmation = CustomMessageBox.ShowOK("Error al generar reporte, intente mas tarde", "Reporte fallido",
                "Aceptar");
            }
        }

        private static async Task SaveExcelFile(FileInfo file)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var workSheet = package.Workbook.Worksheets.Add("Reporte");

                workSheet.Cells["A1"].Value = "Informe prácticas profesionales";
                workSheet.Cells["A1:J1"].Merge = true;
                workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Size = 16;
                workSheet.Cells["A1"].Style.Font.Bold = true;

                workSheet.Cells["A3"].Value = "Región";
                workSheet.Cells["A4"].Value = "Área académica";
                workSheet.Cells["A5"].Value = "Modalidad";
                workSheet.Cells["A6"].Value = "Nivel";
                workSheet.Cells["A7"].Value = "Programa educativo";
                workSheet.Cells["A8"].Value = "ServiciosS.S. Pract. Prof. e Int. Acad.";
                workSheet.Cells["A8:D8"].Merge = true;
                workSheet.Cells["A3:A8"].Style.Font.Bold = true;

                workSheet.Cells["B3"].Value = "XALAPA";
                workSheet.Cells["B4"].Value = "ECONÓMICO ADMINISTRATIVO";
                workSheet.Cells["B5"].Value = "ESCOLARIZADO";
                workSheet.Cells["B6"].Value = "LICENCIATURA";
                workSheet.Cells["B7"].Value = "INGENIERÍA DE SOFTWARE";

                workSheet.Cells["A10"].Value = "Número de alumnos que terminaron en el ciclo escolar anterior y que realizaron prácticas profesionales, por sexo";
                workSheet.Cells["A10:J10"].Merge = true;
                workSheet.Cells["A10"].Style.Font.Bold = true;
                workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                workSheet.Cells["A12"].Value = "Sector";
                workSheet.Cells["C12"].Value = "Hombres";
                workSheet.Cells["E12"].Value = "Mujeres";
                workSheet.Cells["G12"].Value = "Total";
                workSheet.Row(12).Style.Font.Bold = true;
                workSheet.Row(12).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                var range = workSheet.Cells["A3:A8"];
                range.AutoFitColumns();

                await package.SaveAsync();
            }

            exitReport = true;
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }

        private int countStudentsByGender(string sector, string gender)
        {
            int totalStudents = 0;
            using (SCPPContext context = new SCPPContext())
            {
                
            }
            return totalStudents;
        }
    }
}
