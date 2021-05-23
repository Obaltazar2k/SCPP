﻿using SCPP.Utilities;
using System.Windows;
using System.Windows.Controls;
using SCPP.DataAcces;
using System.Linq;
using System.Data.Entity;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para MenuEstudiante.xaml
    /// </summary>
    public partial class MenuEstudiante : Page
    {
        Sesion userSesion;
        public MenuEstudiante()
        {
            InitializeComponent();
            userSesion = Sesion.GetSesion;
        }

        private void EscogerProyectoButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new EscogerProyecto());
            return;
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
                    .FirstOrDefault(u => u.Matriculaestudiante == matricula);
            }
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new EntregarReporte(inscripcion));
            return;
        }


    }
}