using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
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

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GestionarGrupo.xaml
    /// </summary>
    public partial class GestionarGrupo : Page
    {
        private ObservableCollection<Estudiante> studentsCollection;
        private string _user;
        private Grupo actualGroup;
        private bool isModifying = false;


        public GestionarGrupo(Grupo grupo )
        {
            InitializeComponent();
            actualGroup = grupo;
            FillTextBoxes();
            GetSesion();
            GetStudents();
        }

        private void GroupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;

        }

        private void GetStudents()
        {
            
            using(SCPPContext context = new SCPPContext())
            {
                IQueryable<Estudiante> studentsInDB = null;

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
                    .Where(q => q.join.group.Rfcprofesor == _user)
                    .Select(q => q.student);


                if(studentsInDB != null)
                {
                    foreach(Estudiante student in studentsInDB)
                    {
                        if(student != null)
                            studentsCollection.Add(student);
                    }
                }

                StudentList.ItemsSource = studentsCollection;
                DataContext = studentsCollection;
            }
        }

        private void FillTextBoxes()
        {
            TextBoxNRC.Text = actualGroup.Nrc.ToString();
            TextBoxBloque.Text = actualGroup.Bloque;
            TextBoxCupo.Text = actualGroup.Cupo.ToString();
            TextBoxPeriodo.Text = actualGroup.Periodo;
            TextBoxSeccion.Text = actualGroup.Seccion;

        }

        private void ItsModifying()
        {
            isModifying = true;
            SaveChangesButton.Visibility = Visibility.Visible;

            TextBoxNRC.IsReadOnly = false;
            TextBoxBloque.IsReadOnly = false;
            TextBoxCupo.IsReadOnly = false;
            TextBoxPeriodo.IsReadOnly = false;
            TextBoxSeccion.IsReadOnly = false;

        }

        private void ItsNotModifying()
        {
            isModifying = false;
            SaveChangesButton.Visibility = Visibility.Hidden;

            TextBoxNRC.IsReadOnly = true;
            TextBoxBloque.IsReadOnly = true;
            TextBoxCupo.IsReadOnly = true;
            TextBoxPeriodo.IsReadOnly = true;
            TextBoxSeccion.IsReadOnly = true;
        }

        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            ItsModifying();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            
            using(SCPPContext context = new SCPPContext())
            {
                var studentUptdated = UpdateGroup();
                GroupUpdatedMessage(studentUptdated);
            }
            ItsNotModifying();
            
        }

        private void GroupUpdatedMessage(object studentUptdated)
        {
            CustomMessageBox.ShowOK("El registro se ha cambiado con éxito", "Cambio exitoso", "Finalizar");
        }

        private object UpdateGroup()
        {
            Grupo group;
            using(SCPPContext context = new SCPPContext())
            {
                group = context.Grupo.FirstOrDefault(s => s.Nrc == actualGroup.Nrc);

                group.Bloque = TextBoxBloque.Text;
                group.Cupo = int.Parse(TextBoxCupo.Text);
                group.Periodo = TextBoxPeriodo.Text;
                group.Seccion = TextBoxSeccion.Text;

                context.SaveChanges();
            }

            actualGroup = group;
            return group;
            
        }

        private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar el GRUPO "
                    + " con NRC " + actualGroup.Nrc
                    + "?", "Confirmación", "Si", "No");

                if(confirmation == MessageBoxResult.Yes)
                {
                    Grupo group;
                    using(SCPPContext context = new SCPPContext())
                    {
                        group = context.Grupo.FirstOrDefault(s => s.Nrc == actualGroup.Nrc);
                        context.SaveChanges();
                    }
                    actualGroup = group;
                    deleteDone = true;
                }
                else
                    return;

                if(deleteDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.ShowOK("El registro ha sido eliminado con éxito.", "Eliminación", "Finalizar");
                    ReturnToPreviousList(new object(), new RoutedEventArgs());
                }
            }
            catch(EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                     "Fallo en conexión con la base de datos", "Aceptar");
                
            }
        }

        private void ReturnToPreviousList(object v, RoutedEventArgs routedEventArgs)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(isModifying)
            {
                FillTextBoxes();
                ItsNotModifying();
            }
            else
            {
                ReturnToPreviousList(new object(), new RoutedEventArgs());
            }
        }
    }
}
