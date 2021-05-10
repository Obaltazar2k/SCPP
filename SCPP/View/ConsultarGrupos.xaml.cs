using SCPP.DataAcces;
using SCPP.Utilities;
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

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para ConsultarGrupos.xaml
    /// </summary>
    public partial class ConsultarGrupos : Page
    {

        private string _user;
        private ObservableCollection<Grupo> groupsCollection;
        private Grupo groupSelected = null;
        private string _period;

        public ConsultarGrupos()
        {
            InitializeComponent();
            _period = Period.GetPeriod();
            _user = "";
            DataContext = this;
            groupsCollection = new ObservableCollection<Grupo>();
            GetSesion();
            GetGroups();
        }

        private void GetSesion()
        {
            Sesion userSesion = Sesion.GetSesion;
            _user = userSesion.Username;
           
        }

        private void Search(string searchText)
        {
            groupsCollection.Clear();
            using(SCPPContext context = new SCPPContext())
            {
                var groupsBySearch = context.Grupo.Where(s => (s.Nrc.ToString().Contains(searchText) || 
                    s.Cupo.ToString().Contains(searchText) ||
                    s.Bloque.Contains(searchText) ||
                    s.Seccion.Contains(searchText) ||
                    s.Periodo.Contains(searchText))
                    && s.Rfcprofesor.Equals(_user));

                if(groupsBySearch != null)
                {
                    foreach(Grupo group in groupsBySearch)
                    {
                        if(group != null)
                            groupsCollection.Add(group);
                    }
                }
            }
        }

        private void GetGroups()
        {
            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    
                    var groupsList = context.Grupo.Where(p => p.Rfcprofesor.Equals(_user) && p.Periodo.Equals(_period));

                    if(groupsList != null)
                    {
                        foreach(Grupo grupo in groupsList)
                        {
                            if(grupo != null)
                                groupsCollection.Add(grupo);
                        }
                    }
                }

                GroupList.ItemsSource = groupsCollection;
                DataContext = groupsCollection;
            }
            catch(Exception ex)
            {

            }
        }
        
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TextBoxSearch.Text;
            Search(searchText);

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigated += NavigationService_Navigated;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarGrupo(groupSelected));
            return;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            groupsCollection.Clear();
            GetGroups();

            ManageButton.IsEnabled = false;
        }

        private void GroupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                groupSelected = (Grupo)dataGrid.SelectedItems[0];
                if(groupSelected != null)
                    ManageButton.IsEnabled = true;
            }
            catch(ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }
    }
}
