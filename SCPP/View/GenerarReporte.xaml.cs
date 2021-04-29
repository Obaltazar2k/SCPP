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
        private readonly List<string> sectorsList = new List<string> { "Transporte", "Comunicaciones", "Comercial", "Turístico" , "Sanitario" ,
            "Educativo" , "Artes" , "Financiero" , "Administrativo", "Tecnológico" };

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

        private async Task GenerateExcelReport()
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

        private async Task SaveExcelFile(FileInfo file)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var workSheet = package.Workbook.Worksheets.Add("Reporte");

                workSheet.Cells["A1"].Value = "Informe de prácticas profesionales";
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

                int cellsIndex = 13;
                foreach (string sector in sectorsList)
                {       
                    int mujeres = countStudentsByGender(sector, "Femenino");
                    int hombres = countStudentsByGender(sector, "Masculino");
                    if(mujeres != 0 || hombres != 0)
                    {
                        workSheet.Cells["A" + cellsIndex.ToString()].Value = sector;
                        workSheet.Cells["E" + cellsIndex.ToString()].Value = mujeres;
                        workSheet.Cells["C" + cellsIndex.ToString()].Value = hombres;
                       workSheet.Cells["G" + cellsIndex.ToString()].Value = (hombres + mujeres);

                        workSheet.Row(cellsIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        cellsIndex++;
                    }                   
                }

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
                var studentsList = context.Estudiante.Join(
                        context.Inscripción,
                        s => s.Matricula,
                        i => i.Matriculaestudiante,
                        (s, i) => new { student = s, inscription = i })
                        .Join(
                        context.Proyecto,
                        j => j.inscription.Claveproyecto,
                        p => p.Clave,
                        (j, p) => new { join = j, project = p })
                        .Join(
                        context.Organización,
                        j2 => j2.project.OrganizaciónID,
                        o => o.OrganizaciónID,
                        (j2, o) => new { join2 = j2, organization = o })
                        .Where(q => q.organization.Sector.Equals(sector))
                        .Where(q => q.join2.join.student.Genero.Equals(gender))
                        .Select(q => q.join2.join.student);

                totalStudents = studentsList.Count();
            }
            return totalStudents;
        }

        private int RecorrerList()
        {
            int total = 0;
            foreach(string sector in sectorsList)
            {
                total = countStudentsByGender(sector, "Femenino");
            }

            return total;
        }
    }
}
