﻿using System;
using System.Collections.Generic;
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

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para MenuEstudiante.xaml
    /// </summary>
    public partial class MenuEstudiante : Page
    {
        public MenuEstudiante()
        {
            InitializeComponent();
        }

        private void EscogerProyectoButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new EscogerProyecto());
            return;
        }
    }
}
