using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
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
    /// Lógica de interacción para GestionarResposable.xaml
    /// </summary>
    public partial class GestionarResposable : Page
    {
        Responsableproyecto actualResposable;
        private bool isModifying = false;
        private ObservableCollection<Proyecto> proyectsCollection;


        public GestionarResposable(Responsableproyecto resposable)
        {
            try
            {
                InitializeComponent();
                this.actualResposable = resposable;
                proyectsCollection = new ObservableCollection<Proyecto>();
                FillTextBoxes();
                GetOrganization();
            }
            catch(EntityException)
            {

            }
            
        }

        private void EditResponsableButton_Click(object sender, RoutedEventArgs e)
        {
            ItsModifying();
            FillTextBoxes();
        }

        private void DeleteResposableButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar el Resposable "
                    + " con correo " + actualResposable.Correopersonal
                    + "?", "Confirmación", "Si", "No");;

                if(confirmation == MessageBoxResult.Yes)
                {
                    Responsableproyecto resposable;
                    using(SCPPContext context = new SCPPContext())
                    {
                        resposable = context.Responsableproyecto.FirstOrDefault(s => s.Correopersonal == actualResposable.Correopersonal);
                        context.Responsableproyecto.Remove(resposable);
                        context.SaveChanges();
                    }
                    actualResposable = resposable;
                    deleteDone = true;
                }
                else
                    return;

                if(deleteDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.ShowOK("El registro ha sido eliminado con éxito.", "Eliminación", "Finalizar");
                    ReturnToPreviousList(new object(), new RoutedEventArgs());
                }
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void ReturnToPreviousList(object v, RoutedEventArgs routedEventArgs)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(isModifying)
            {
                FillTextBoxes();
                ItsNotModifying();
            }
            else
            {
                ReturnToPreviousList(new object(), new RoutedEventArgs());
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using(SCPPContext context = new SCPPContext())
                {
                    var resposableUptdated = UpdateResposable();
                    ResposableUpdatedMessage(resposableUptdated);
                }
                ItsNotModifying();
            }
            catch(EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void ResposableUpdatedMessage(object resposableUptdated)
        {
            CustomMessageBox.ShowOK("El registro se ha cambiado con éxito", "Cambio exitoso", "Finalizar");
        }

        private void GetOrganization()
        {

            using(SCPPContext context = new SCPPContext())
            {
                //recupera el id del responsable
                var resposable = context.Responsableproyecto.FirstOrDefault(a => a.Correopersonal.Equals(actualResposable.Correopersonal));

                var proyectsList = context.Proyecto.Where(p => p.ResponsableproyectoID == resposable.ResponsableproyectoID);

                if(proyectsList != null)
                {
                    foreach(var proyects in proyectsList)
                    {
                        if(proyects != null)
                            proyectsCollection.Add(proyects);
                    }
                }
                
                ProyectsList.ItemsSource = proyectsCollection;
                DataContext = ProyectsList;

                if(proyectsCollection.Count == 0)
                {
                    Label.Content = "El resposable aun no cuenta con proyectos";
                }
            }
        }

        private object UpdateResposable()
        {
           
            Responsableproyecto resposable;
            using(SCPPContext context = new SCPPContext())
            {
                resposable = context.Responsableproyecto.FirstOrDefault(s => s.Correopersonal.Equals(actualResposable.Correopersonal));

                resposable.Nombre = TextBoxName.Text;
                resposable.Apellidopaterno = TextBoxLastName.Text;
                resposable.Apellidomaterno = TextBoxMothersLastName.Text;
                resposable.Telefono = TextBoxPhone.Text;
                resposable.Activo = Int32.Parse(TextBoxStatus.Text);

                context.SaveChanges();
            }

            actualResposable = resposable;
            return resposable;

        }



        private void FillTextBoxes()
        {
            TextBoxEMail.Text = actualResposable.Correopersonal;
            TextBoxName.Text = actualResposable.Nombre;
            TextBoxLastName.Text = actualResposable.Apellidopaterno;
            TextBoxMothersLastName.Text = actualResposable.Apellidomaterno;
            TextBoxPhone.Text = actualResposable.Telefono;
            TextBoxStatus.Text = actualResposable.Activo.ToString();
        }

        private void ItsModifying()
        {
            isModifying = true;
            SaveChangesButton.Visibility = Visibility.Visible;

            TextBoxStatus.IsReadOnly = false;
            TextBoxName.IsReadOnly = false;
            TextBoxLastName.IsReadOnly = false;
            TextBoxMothersLastName.IsReadOnly = false;
            TextBoxPhone.IsReadOnly = false;

        }

        private void ItsNotModifying()
        {
            isModifying = false;
            SaveChangesButton.Visibility = Visibility.Hidden;

            TextBoxEMail.IsReadOnly = true;
            TextBoxName.IsReadOnly = true;
            TextBoxLastName.IsReadOnly = true;
            TextBoxMothersLastName.IsReadOnly = true;
            TextBoxPhone.IsReadOnly = true;
            TextBoxStatus.IsReadOnly = false;
        }

    }
}
