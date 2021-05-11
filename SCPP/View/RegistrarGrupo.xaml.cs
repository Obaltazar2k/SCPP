using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Data.Entity.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para RegistrarGrupo.xaml
    /// </summary>
    public partial class RegistrarGrupo : Page
    {
        private string _period;
        public RegistrarGrupo()
        {
            InitializeComponent();
            _period = Period.GetPeriod();
            TextBoxPeriod.Text = _period;
            TextBoxPeriod.IsReadOnly = true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerificateFields())
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        Grupo grupo = new Grupo
                        {
                            Bloque = TextBoxBlock.Text,
                            Cupo = Convert.ToInt32(TextBoxCupo.Text),
                            Nrc = Convert.ToInt32(TextBoxNrc.Text),
                            Seccion = TextBoxSection.Text,
                            Periodo = _period,
                            Estado = "Disponible"
                        };

                        context.Grupo.Add(grupo);
                        context.SaveChanges();
                    }
                    MessageBoxResult confirmation = CustomMessageBox.ShowOK("El registro se ha realizado con éxito", "Registro exitoso",
                    "Finalizar");
                    CancelButton_Click(new object(), new RoutedEventArgs());
                }
            }
            catch (EntityException)
            {
                CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");

                ReturnToLogin(new object(), new RoutedEventArgs());
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)

                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void ReturnToLogin(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new IniciarSesion());
            return;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateCupo(TextBoxCupo.Text)
                && FieldsVerificator.VerificateNRC(TextBoxNrc.Text)
                && FieldsVerificator.VerficiateBlock(TextBoxBlock.Text)
                && FieldsVerificator.VerficiateSection(TextBoxSection.Text);
        }
    }
}
