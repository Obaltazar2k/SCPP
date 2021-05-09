using SCPP.View;
using SCPP.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuCoordinador.xaml
    /// </summary>
    public partial class MenuCoordinador : Page
    {
        public MenuCoordinador()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            //Set tooltip visibility
            if (MenuToggleButton.IsChecked == true)
            {
                HomeToolTip.Visibility = Visibility.Collapsed;
                StudentToolTip.Visibility = Visibility.Collapsed;
                GroupToolTip.Visibility = Visibility.Collapsed;
                OrganizationToolTip.Visibility = Visibility.Collapsed;
                ProfessorToolTip.Visibility = Visibility.Collapsed;
                ProjectToolTip.Visibility = Visibility.Collapsed;
            }
            else
            {
                HomeToolTip.Visibility = Visibility.Visible;
                StudentToolTip.Visibility = Visibility.Visible;
                GroupToolTip.Visibility = Visibility.Visible;
                OrganizationToolTip.Visibility = Visibility.Visible;
                ProfessorToolTip.Visibility = Visibility.Visible;
                ProjectToolTip.Visibility = Visibility.Visible;
            }
        }

        private void MenuToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            BackgroundImage.Opacity = 1;
        }

        private void MenuToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            BackgroundImage.Opacity = 0.3;
        }

        private void Background_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();
            LabelTitle.Content = "";

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "HomeButton":
                    usc = new MenuCoordinadorHome();
                    GridMain.Children.Add(usc);
                    LabelTitle.Content = "";
                    break;
                case "StudentButton":
                    usc = new MenuCoordinadorStudent();
                    GridMain.Children.Add(usc);
                    LabelTitle.Content = "Estudiantes";
                    break;
                case "GroupButton":
                    usc = new MenuCoordinadorGroups();
                    GridMain.Children.Add(usc);
                    LabelTitle.Content = "Grupos";
                    break;
                case "OrganizationButton":
                    usc = new MenuCoordinadorOrganization();
                    GridMain.Children.Add(usc);
                    LabelTitle.Content = "Organización";
                    break;
                case "ProfessorButton":
                    usc = new MenuCoordinadorProfessor();
                    GridMain.Children.Add(usc);
                    LabelTitle.Content = "Profesores";
                    break;
                case "ProjectButton":
                    usc = new MenuCoordinadorProject();
                    GridMain.Children.Add(usc);
                    LabelTitle.Content = "Proyectos";
                    break;
                default:
                    break;
            }
        }

        private void CloseSesionButton_Clicked(object sender, RoutedEventArgs e)
        {
            Sesion.CloseSesion();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }
    }
}