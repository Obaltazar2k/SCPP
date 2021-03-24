using System;
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

        public AsociarEstudianteGrupo(Profesor profesor)
        {
            _profesor = profesor;
            InitializeComponent();
            DataContext = this;
            FillComboBox(_profesor);
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
            /*Profesor profesor = new Profesor();
            profesor.Nombre = "Jorge Octavio";
            profesor.Apellidopaterno = "Ocharan";
            profesor.Apellidomaterno = "Hernandez";
            profesor.Rfc = "123";
            profesor.Correopersonal = "ocha@email.com";
            profesor.Contraseña = "1234";
            ChangeView(new AsociarEstudianteGrupo(profesor));*/
        }

        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                var studentsListWithoutGroup = context.Estudiante.Join(
                    context.Inscripción,
                    s => s.Matricula,
                    i => i.Matriculaestudiante,
                    (s, i) => new { student = s, inscription = i })
                    .Where(s2 => s2.inscription.GrupoID.Equals(null))
                    .Select(s2 => s2.student);
                if (studentsListWithoutGroup != null)
                {
                    foreach(Estudiante estudiante in studentsListWithoutGroup)
                    {
                        if (estudiante != null)
                            studentsCollection.Add(estudiante);
                    }
                }
            }
            StudentList.ItemsSource = studentsCollection;
            DataContext = studentsCollection;
        }
    }
}
