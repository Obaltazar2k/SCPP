using SCPP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para AsociarEstudianteGrupo.xaml
    /// </summary>
    public partial class AsociarEstudianteGrupo : Page
    {
        private int _groupID = 0;
        private string _user;
        private ObservableCollection<Estudiante> selectedStudents = null;
        private ObservableCollection<Estudiante> studentsCollection;
        
        public AsociarEstudianteGrupo()
        {
            InitializeComponent();
            DataContext = this;
            GetSesion();
            GetStudents();
            FillComboBox(_user);
        }

        private ObservableCollection<Grupo> groupsCollection { get; set; }
        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeStudentsGroups();
            ConfirmedAssociationMessage();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CheckSelections()
        {
            if ((_groupID != 0) && (selectedStudents != null))
                AgreeButton.IsEnabled = true;
        }

        private void ConfirmedAssociationMessage()
        {
            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("La asociación se ha realizado con éxito", "Asociación exitosa",
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

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;

        }

        private void FillComboBox(string _user)
        {
            groupsCollection = new ObservableCollection<Grupo>();
            using (SCPPContext context = new SCPPContext())
            {
                var groupsList = context.Grupo.OrderBy(g => g.Nrc).Where(g => g.Rfcprofesor.Equals(_user));
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

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grupo = (Grupo)GroupComboBox.SelectedItem;
            _groupID = grupo.GrupoID;
            CheckSelections();
        }
        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStudents = new ObservableCollection<Estudiante>();
            foreach (var student in StudentList.SelectedItems)
            {
                selectedStudents.Add((Estudiante)student);
            }
            CheckSelections();
        }
    }
}