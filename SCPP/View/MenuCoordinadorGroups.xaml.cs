using System;
using System.Windows;
using System.Windows.Controls;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuCoordinadorGroups.xaml
    /// </summary>
    public partial class MenuCoordinadorGroups : UserControl
    {
        public MenuCoordinadorGroups()
        {
            InitializeComponent();
        }

        private void GetGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ConsultarGrupos());
            return;
        }

        private void RegisterGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarGrupo());
            return;
        }
    }
}
