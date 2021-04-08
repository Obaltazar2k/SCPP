using SCPP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using SCPP.DataAcces;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para EscogerProyecto.xaml
    /// </summary>
    public partial class EscogerProyecto : Page
    {
        private readonly DateTime thisDay = DateTime.Today;
        private string _user;
        private ObservableCollection<Proyecto> choosenProjects = null;
        private ObservableCollection<Proyecto> choosenProjectsCollection = new ObservableCollection<Proyecto>();
        private string period = Period.GetPeriod();
        private ObservableCollection<Proyecto> projectsCollection;
        private ObservableCollection<Proyecto> selectedProjects = null;
        
        public EscogerProyecto()
        {
            InitializeComponent();
            GetSesion();
            GetProyects();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckProjectSelection())
            {
                foreach (var project in selectedProjects)
                {
                    choosenProjectsCollection.Add(project);
                }
                ChoosenProjectsList.ItemsSource = choosenProjectsCollection;
                UpdateProjectsList();
                CheckAgreedButton();
            }
            else
            {
                CustomMessageBox.ShowOK("Solo se pueden añadir 3 proyectos", "Error", "Aceptar");
            }
        }

        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckChoosenProjects())
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea escoger estos proyectos?", "Confirmacion", "Si", "No");
                if (confirmation == MessageBoxResult.Yes)
                {
                    CreateProjectsSelection();
                    ConfirmedChoosenProjectsMessage();
                }
                else
                {
                    return;
                }
            }
            else
            {
                CustomMessageBox.ShowOK("Debes elegir 3 proyectos", "Error", "Aceptar");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CheckAgreedButton()
        {
            if (choosenProjectsCollection.Count() != 0)
            {
                AgreeButton.IsEnabled = true;
            }
            else
            {
                AgreeButton.IsEnabled = false;
            }
        }

        private bool CheckChoosenProjects()
        {
            if (choosenProjectsCollection.Count() == 3)
            {
                return true;
            }
            return false;
        }

        private bool CheckProjectSelection()
        {
            if ((selectedProjects.Count() + choosenProjectsCollection.Count()) <= 3)
            {
                return true;
            }
            return false;
        }

        private void ChoosenProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddButton.IsEnabled = false;
            if (RemoveButton.IsEnabled == false)
            {
                RemoveButton.IsEnabled = true;
            }
            ProjectsList.SelectedItems.Clear();
            choosenProjects = new ObservableCollection<Proyecto>();
            foreach (var project in ChoosenProjectsList.SelectedItems)
            {
                choosenProjects.Add((Proyecto)project);
            }
        }
        private void ConfirmedChoosenProjectsMessage()
        {
            CustomMessageBox.ShowOK("Los proyectos escogidos se han guardado en la base de datos", "Proyectos escogidos exitosamente", "Aceptar");
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MenuEstudiante());
            return;
        }

        private void CreateProjectsSelection()
        {
            var matricula = GetUserMatricula();
            foreach (var project in choosenProjectsCollection)
            {
                var projectsSelection = new Selecciónproyecto
                {
                    Fecha = thisDay,
                    PeriodoID = period,
                    Matriculaestudiante = matricula,
                    Claveproyecto = project.Clave
                };
                using (SCPPContext context = new SCPPContext())
                {
                    context.Selecciónproyecto.Add(projectsSelection);
                    context.SaveChanges();
                }
            }
        }

        private void GetProyects()
        {
            projectsCollection = new ObservableCollection<Proyecto>();
            using (SCPPContext context = new SCPPContext())
            {
                var proyectsList = context.Proyecto.Where(p => p.Inscripción.Count <= 2);
                if (proyectsList != null)
                {
                    foreach (Proyecto proyecto in proyectsList)
                    {
                        if (proyecto != null)
                            projectsCollection.Add(proyecto);
                    }
                }
            }
            ProjectsList.ItemsSource = projectsCollection;
        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
        }

        private string GetUserMatricula()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var student = context.Estudiante.FirstOrDefault(s => s.Matricula.Equals(_user));
                return student.Matricula;
            }
        }

        private void ProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveButton.IsEnabled = false;
            if (AddButton.IsEnabled == false)
            {
                AddButton.IsEnabled = true;
            }
            ChoosenProjectsList.SelectedItems.Clear();
            selectedProjects = new ObservableCollection<Proyecto>();
            foreach (var project in ProjectsList.SelectedItems)
            {
                selectedProjects.Add((Proyecto)project);
            }
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var project in choosenProjects)
            {
                projectsCollection.Add(project);
            }
            UpdateChoosenProjectsList();
            CheckAgreedButton();
        }
        private void UpdateChoosenProjectsList()
        {
            foreach (var project in choosenProjects)
            {
                choosenProjectsCollection.Remove(project);
            }
        }

        private void UpdateProjectsList()
        {
            foreach (var project in selectedProjects)
            {
                projectsCollection.Remove(project);
            }
        }
    }
}