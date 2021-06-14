using SCPP.Utilities;
using System.Windows;
using System.Windows.Controls;
using SCPP.DataAcces;
using System.Linq;
using System.Data.Entity;
using WPFCustomMessageBox;
using System.Data.Entity.Core;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuEstudiante.xaml
    /// </summary>
    public partial class MenuEstudiante : Page
    {
        Sesion userSesion;
        private string period = Period.GetPeriod();
        public MenuEstudiante()
        {
            InitializeComponent();
            userSesion = Sesion.GetSesion;
        }

        private void CloseSesionButton_Clicked(object sender, RoutedEventArgs e)
        {
            Sesion.CloseSesion();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }

        private void EscogerProyectoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsStudentEnrolled() && !StudentChoseProjectsAlready() && ProjectsAvailable())
                {
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow?.ChangeView(new EscogerProyecto());
                    return;
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private bool IsStudentEnrolled()
        {
            string matricula = userSesion.Username;
            Inscripción inscripción;
            using (SCPPContext context = new SCPPContext())
            {
                inscripción = context.Inscripción.FirstOrDefault(i => i.Matriculaestudiante.Equals(matricula) && i.Estatus.Equals("Cursando"));
                if (inscripción != null)
                {
                    CustomMessageBox.ShowOK("Ya tienes asignado un proyecto", "Error", "Aceptar");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool ProjectsAvailable()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var projects = context.Proyecto.Where(p => p.Inscripción.Count < p.Noestudiantes);
                if (projects.Count() >= 3)
                {
                    return true;
                }
                else
                {
                    CustomMessageBox.ShowOK("No hay proyectos disponibles por el momento", "Error", "Aceptar");
                    return false;
                }
            }
        }

        private bool StudentChoseProjectsAlready()
        {
            string matricula = userSesion.Username;
            using (SCPPContext context = new SCPPContext())
            {
                var projectsSelected = context.Selecciónproyecto.FirstOrDefault(s => s.Matriculaestudiante.Equals(matricula) && s.PeriodoID.Equals(period));
                if (projectsSelected != null)
                {
                    CustomMessageBox.ShowOK("Ya has escogido proyectos", "Error", "Aceptar");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void SubirReporteButton_Click(object sender, RoutedEventArgs e)
        {
            string matricula = userSesion.Username;
            Inscripción inscripcion;
            using (SCPPContext context = new SCPPContext())
            {
                inscripcion = context.Inscripción
                    .Include(i => i.Proyecto)
                    .Include(i => i.Proyecto.Organización)
                    .Include(i => i.Expediente)
                    .Include(i => i.Grupo)
                    .FirstOrDefault(i => i.Matriculaestudiante.Equals(matricula));
            }
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new EntregarReporte(inscripcion));
            return;
        }

        private void GestionarExpedienteButton_Click(object sender, RoutedEventArgs e)
        {
            string matricula = userSesion.Username;
            Inscripción inscripcion;
            using (SCPPContext context = new SCPPContext())
            {
                inscripcion = context.Inscripción
                    .Include(i => i.Proyecto)
                    .Include(i => i.Proyecto.Organización)
                    .Include(i => i.Expediente)
                    .Include(i => i.Grupo)
                    .FirstOrDefault(i => i.Matriculaestudiante.Equals(matricula));
            }
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarExpediente(inscripcion));
            return;
        }
    }
}