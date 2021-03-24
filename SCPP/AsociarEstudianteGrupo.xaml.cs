using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCustomMessageBox;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para AsociarEstudianteGrupo.xaml
    /// </summary>
    public partial class AsociarEstudianteGrupo : Page
    {
        private Profesor _profesor;
        private ObservableCollection<Grupo> groupsCollection { get; set; }
        private ObservableCollection<Estudiante> studentsCollection;
        ObservableCollection<Estudiante> selectedStudents = null;
        private int _groupID = 0;

        public AsociarEstudianteGrupo(Profesor profesor)
        {
            _profesor = profesor;
            InitializeComponent();
            DataContext = this;
            GetStudents();
            FillComboBox(_profesor);
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grupo = (Grupo)GroupComboBox.SelectedItem;
            _groupID = grupo.GrupoID;
            CheckSelections();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeStudentsGroups();
            ConfirmedAssociationMessage();
        }

        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStudents = new ObservableCollection<Estudiante>();
            foreach(var student in StudentList.SelectedItems)
            {
                selectedStudents.Add((Estudiante)student);
            }
            CheckSelections();
        }

        private void FillComboBox(Profesor profesor)
        {
            groupsCollection = new ObservableCollection<Grupo>();
            using (SCPPContext context = new SCPPContext())
            {
                var groupsList = context.Grupo.OrderBy(s => s.Nrc).Where(s => s.Rfcprofesor.Equals(profesor.Rfc));
                if (groupsList != null)
                {
                    foreach (Grupo grupo in groupsList)
                    {
                        if (grupo != null)
                            groupsCollection.Add(grupo);
                    }
                }
            }
            GroupComboBox.ItemsSource = groupsCollection;
            DataContext = groupsCollection;
        }

        private void GetStudents()
        {
            studentsCollection = new ObservableCollection<Estudiante>();
            using (SCPPContext context = new SCPPContext())
            {
                var studentsWithoutGroupList = context.Estudiante.Join(
                    context.Inscripción,
                    e => e.Matricula, 
                    i => i.Matriculaestudiante,
                    (e, i) => new { e, i })
                    .Where(x => x.i.GrupoID == null)
                    .Select(x => x.e);
                if (studentsWithoutGroupList != null)
                {
                    foreach (Estudiante estudiante in studentsWithoutGroupList)
                    {
                        if (estudiante != null)
                            studentsCollection.Add(estudiante);
                    }
                }
            }
            StudentList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }

        private void CheckSelections()
        {
            if ((_groupID != 0) && (selectedStudents != null))
                AgreeButton.IsEnabled = true;
        }

        private void ChangeStudentsGroups()
        {
            ObservableCollection<Estudiante> studentList = new ObservableCollection<Estudiante>();
            try
            {
                using (SCPPContext context = new SCPPContext())
                {
                    foreach (var student in selectedStudents)
                    {
                        var foundInscription = context.Inscripción.Where(i => i.Matriculaestudiante.Equals(student.Matricula)).FirstOrDefault();
                        if (foundInscription != null)
                        {
                            foundInscription.GrupoID = _groupID;
                            context.SaveChanges();
                        }
                        studentList.Add(student);
                    }
                }

                foreach (var student in studentList)
                {
                    studentsCollection.Remove(student);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ConfirmedAssociationMessage()
        {
            MessageBoxResult confirmation =  CustomMessageBox.ShowYesNo("La asociación se ha realizado con éxito", "Asociación exitosa", 
                "Asociar otro estudiante",
                "Regresar a menú");
            if (confirmation == MessageBoxResult.Yes)
            {
                return;
            }
            else
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new MenuProfesor());
                return;
            }
        }
    }
}
