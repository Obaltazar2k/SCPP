using SCPP.DataAcces;
using SCPP.Utilities;
using Syroot.Windows.IO;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using Shape = Microsoft.Office.Interop.Word.Shape;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GenerarOficioAsignacion.xaml
    /// </summary>
    public partial class GenerarOficioAsignacion : System.Windows.Controls.Page
    {
        private string _user;
        private readonly DateTime thisDay = DateTime.Today;
        private string period = Period.GetPeriod();
        private ObservableCollection<Estudiante> selectedStudents = null;
        private ObservableCollection<Estudiante> studentsCollection;
        public GenerarOficioAsignacion()
        {
            InitializeComponent();
            GetSesion();
            GetStudents();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void GenerateDocButton_Click(object sender, RoutedEventArgs e)
        {
            string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path;
            string folder = downloadsPath + "\\PracticasProfesionales\\";
            
            foreach (var student in selectedStudents)
            {
                string fullFilePath = folder + "Asignacion_" + student.Nombre + student.Apellidopaterno + student.Apellidomaterno;
                CreateAssignDocument(@"D:\Usuario\Omar\Descargas", student);
            }
            
        }

        private void GetStudents()
        {
            studentsCollection = new ObservableCollection<Estudiante>();
            IQueryable<Estudiante> studentsInDB = null;
            using (SCPPContext context = new SCPPContext())
            {
                studentsInDB = context.Grupo.Join(
                        context.Inscripción,
                        g => g.GrupoID,
                        i => i.GrupoID,
                        (g, i) => new { group = g, inscription = i })
                        .Join(
                        context.Estudiante,
                        j => j.inscription.Matriculaestudiante,
                        s => s.Matricula,
                        (j, s) => new { join = j, student = s })
                        .Where(q => q.join.inscription.Periodo.Equals(period) && q.join.inscription.GrupoID != null && q.student.Estado.Equals("Inscrito"))
                        .Select(q => q.student);

                if (studentsInDB != null)
                {
                    foreach (Estudiante student in studentsInDB)
                    {
                        if (student != null)
                            studentsCollection.Add(student);
                    }
                }
            }            
            StudentList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
        }

        private void TextBoxSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                string searchText = TextBoxSearch.Text;
                Search(searchText);
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void Search(string searchText)
        {
            studentsCollection.Clear();
            using (SCPPContext context = new SCPPContext())
            {
                var studentsBySearch = context.Estudiante.Where(
                    s => s.Matricula.Contains(searchText) ||
                    s.Nombre.Contains(searchText) ||
                    s.Apellidomaterno.Contains(searchText) ||
                    s.Apellidopaterno.Contains(searchText) ||
                    s.Correopersonal.Contains(searchText) ||
                    s.Telefono.Contains(searchText));

                if (studentsBySearch != null)
                {
                    foreach (Estudiante student in studentsBySearch)
                    {
                        if (student != null)
                            studentsCollection.Add(student);
                    }
                }
            }
        }

        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GenerateDocButton.IsEnabled == false)
            {
                GenerateDocButton.IsEnabled = true;
            }
            selectedStudents = new ObservableCollection<Estudiante>();
            foreach (var student in StudentList.SelectedItems)
            {
                selectedStudents.Add((Estudiante)student);
            }
        }

        private void FindAndReplace(Word.Application app, Word.Document doc, string findText, string replaceWithText)
        {
            Find findObject = app.Selection.Find;
            findObject.ClearFormatting();
            findObject.Text = findText;
            findObject.Replacement.ClearFormatting();
            findObject.Replacement.Text = replaceWithText;
            object missing = System.Reflection.Missing.Value;
            object replaceAll = WdReplace.wdReplaceAll;
            findObject.Execute(ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceAll, ref missing, ref missing, ref missing, ref missing);
            var shapes = doc.Shapes;
            foreach (Shape shape in shapes)
            {
                if (shape.TextFrame.HasText != 0)
                {
                    var initialText = shape.TextFrame.TextRange.Text;
                    var resultingText = initialText.Replace(findText, replaceWithText);
                    if (initialText != resultingText)
                    {
                        shape.TextFrame.TextRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                        shape.TextFrame.TextRange.Text = resultingText;
                    }
                }
            }
        }

        private void CreateAssignDocument(object SaveAs, Estudiante student)
        {
            object filename = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Utilities\\DocTemplates\\AsignDocTemplate.docx"));
            Word.Application wordApp = new Word.Application();
            object missing = Missing.Value;
            Word.Document myWordDoc = null;

            if (File.Exists((string)filename))
            {
                object readOnly = false;
                object isVisible = false;
                wordApp.Visible = false;

                myWordDoc = wordApp.Documents.Open(ref filename, ref missing, ref readOnly,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing, ref missing);
                myWordDoc.Activate();

                //find and replace
                this.FindAndReplace(wordApp, myWordDoc, "<nombre>", student.Nombre + " " + student.Apellidopaterno + " " + student.Apellidomaterno);
                this.FindAndReplace(wordApp, myWordDoc, "<matricula>", student.Matricula);
                this.FindAndReplace(wordApp, myWordDoc, "<nombre_proyecto>", student.Correopersonal);

                //Save as
                myWordDoc.SaveAs2(ref SaveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing);

                myWordDoc.Close();
                wordApp.Quit();
                MessageBox.Show("File Created!");
            }
            else
            {
                MessageBox.Show("File not Found!");
            }
        }
    }
}
