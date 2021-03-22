using System;
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
    /// Lógica de interacción para AsignarProfesorGrupo.xaml
    /// </summary>
    public partial class AsignarProfesorGrupo : Page
    {
        public AsignarProfesorGrupo()
        {
            InitializeComponent();
            GetProfesors();
        }

        private void GetProfesors()
        {
            List<Profesor>profesorsCollection = new List<Profesor>();
            using (SCPPContext context = new SCPPContext())
            {
                var profesorsList = context.Profesor.ToList();
                if (profesorsList != null)
                {
                    foreach (Profesor profesor in profesorsList)
                    {
                        if (profesor != null)
                            profesorsCollection.Add(profesor);
                    }
                }
            }
            ProfesorsList.ItemsSource = profesorsCollection;
        }
        private void AssignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        { 
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
