﻿using System.Windows;
using System.Windows.Controls;

namespace SCPP
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

        private void AssignProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new AsignarProyectoEstudiante());
            return;
        }

        private void AssignGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new AsignarProfesorGrupo());
            return;
        }

        private void DesassignGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new DesasignarProfesorGrupo());
            return;
        }

        private void RegisterStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarEstudiante());
            return;
        }

        private void RegisterProyectButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new RegistrarProyecto());
            return;
        }
    }   
}
