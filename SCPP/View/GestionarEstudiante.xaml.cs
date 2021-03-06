﻿using SCPP.DataAcces;
using SCPP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using WPFCustomMessageBox;

namespace SCPP.View
{
    /// <summary>
    /// Lógica de interacción para GestionarEstudiante.xaml
    /// </summary>
    public partial class GestionarEstudiante : Page
    {
        private readonly List<string> differentsStates = new List<string> { "Inactivo", "Preinscrito", "Inscrito"};
        private static Sesion userSesion;
        private Estudiante actualStudent;
        private ObservableCollection<Inscripción> inscriptionsCollection;
        private Inscripción inscriptionSelected;
        private bool isModifying = false;

        public GestionarEstudiante(Estudiante student)
        {
            try
            {
                InitializeComponent();
                ComboBoxState.ItemsSource = differentsStates;
                actualStudent = student;
                FillTextBoxes();
                GetSesion();
                GetInscriptions();
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (isModifying)
            {
                FillTextBoxes();
                ItsNotModifying();
            }
            else
            {
                ReturnToPreviousList(new object(), new RoutedEventArgs());
            }
        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteDone = false;
            try
            {
                MessageBoxResult confirmation = CustomMessageBox.ShowYesNo("¿Seguro que desea eliminar al ESTUDIANTE "
                    + actualStudent.Nombre + " " + actualStudent.Apellidopaterno + " con matricula " + actualStudent.Matricula
                    + "?", "Confirmación", "Si", "No");

                if (confirmation == MessageBoxResult.Yes)
                {
                    Estudiante student;
                    using (SCPPContext context = new SCPPContext())
                    {
                        student = context.Estudiante.FirstOrDefault(s => s.Matricula == actualStudent.Matricula);
                        student.Activo = 0;
                        student.Estado = "Inactivo";
                        context.SaveChanges();
                    }
                    actualStudent = student;
                    deleteDone = true;
                }
                else
                    return;

                if (deleteDone == true)
                {
                    MessageBoxResult result = CustomMessageBox.ShowOK("El registro ha sido eliminado con éxito.", "Eliminación", "Finalizar");
                    ReturnToPreviousList(new object(), new RoutedEventArgs());
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            ItsModifying();
        }

        private void FillTextBoxes()
        {
            TextBoxMatricula.Text = actualStudent.Matricula;
            TextBoxName.Text = actualStudent.Nombre;
            TextBoxApellidoPaterno.Text = actualStudent.Apellidopaterno;
            TextBoxApellidoMaterno.Text = actualStudent.Apellidomaterno;
            TextBoxEmail.Text = actualStudent.Correopersonal;
            TextBoxPhone.Text = actualStudent.Telefono;
            TextBoxStatus.Text = actualStudent.Estado;
            switch (actualStudent.Estado)
            {
                case "Inactivo":
                    ComboBoxState.SelectedIndex = 0;
                    TextBoxStatus.Text = "Inactivo";
                    break;

                case "Inscrito":
                    ComboBoxState.SelectedIndex = 2;
                    TextBoxStatus.Text = "Inscrito";
                    break;

                case "Preinscrito":
                    ComboBoxState.SelectedIndex = 1;
                    TextBoxStatus.Text = "Preinscrito";
                    break;
            }
        }

        private void GetExpedientButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new GestionarExpediente(inscriptionSelected));
            return;
        }

        private void GetInscriptions()
        {
            inscriptionsCollection = new ObservableCollection<Inscripción>();
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionsList = context.Inscripción.Where(i => i.Matriculaestudiante == actualStudent.Matricula)
                    .Include(i => i.Proyecto)
                    .Include(i => i.Proyecto.Organización)
                    .Include(i => i.Expediente)
                    .Include(i => i.Grupo);
                if (inscriptionsList != null)
                {
                    foreach (Inscripción inscription in inscriptionsList)
                    {
                        if (inscription != null)
                            inscriptionsCollection.Add(inscription);
                    }
                }
            }

            if (userSesion.Kind == "Coordinator")
                DeleteStudentButton.Visibility = Visibility.Visible;

            ProyectColumn.Binding = new Binding("Proyecto.Nombre");
            OrganizationColumn.Binding = new Binding("Proyecto.Organización.Nombre");

            InscriptionList.ItemsSource = inscriptionsCollection;
            DataContext = inscriptionsCollection;
        }

        private void GetSesion()
        {
            userSesion = Sesion.GetSesion;
        }

        private void InscriptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                inscriptionSelected = (Inscripción)dataGrid.SelectedItems[0];
                if (inscriptionSelected != null)
                    GetExpedientButton.IsEnabled = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch necesario al seleccionar de la tabla y dar clic en registrar
            }
        }

        private void ItsModifying()
        {
            isModifying = true;
            GetExpedientButton.Visibility = Visibility.Hidden;
            SaveChangesButton.Visibility = Visibility.Visible;
            InscriptionList.Visibility = Visibility.Hidden;
            TextBoxName.IsReadOnly = false;
            TextBoxApellidoPaterno.IsReadOnly = false;
            TextBoxApellidoMaterno.IsReadOnly = false;
            TextBoxEmail.IsReadOnly = false;
            TextBoxPhone.IsReadOnly = false;
            TextBoxStatus.IsReadOnly = false;
            TextBoxStatus.Visibility = Visibility.Hidden;
            ComboBoxState.Visibility = Visibility.Visible;
        }

        private void ItsNotModifying()
        {
            isModifying = false;
            GetExpedientButton.Visibility = Visibility.Visible;
            SaveChangesButton.Visibility = Visibility.Hidden;
            InscriptionList.Visibility = Visibility.Visible;

            InscriptionList.IsEnabled = true;
            InscriptionList.Focusable = true;
            InscriptionList.IsReadOnly = false;

            TextBoxName.IsReadOnly = true;
            TextBoxApellidoPaterno.IsReadOnly = true;
            TextBoxApellidoMaterno.IsReadOnly = true;
            TextBoxEmail.IsReadOnly = true;
            TextBoxPhone.IsReadOnly = true;
            TextBoxStatus.IsReadOnly = true;
            TextBoxStatus.Visibility = Visibility.Visible;
            TextBoxStatus.Text = ComboBoxState.Text;
            ComboBoxState.Visibility = Visibility.Hidden;
        }

        private void ReturnToPreviousList(object v, RoutedEventArgs routedEventArgs)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                CustomMessageBox.ShowOK("No hay entrada a la cual volver.", "Error al navegar hacía atrás", "Aceptar");
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerificateFields())
                {
                    using (SCPPContext context = new SCPPContext())
                    {
                        var studentUptdated = UpdateStudent();
                        CustomMessageBox.ShowOK("El registro se ha cambiado con éxito", "Cambio exitoso", "Finalizar");
                    }
                    ItsNotModifying();
                }
            }
            catch (EntityException)
            {
                Restarter.RestarSCPP();
            }
        }

        private void TextBoxPhone_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Tab)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private object UpdateStudent()
        {
            Estudiante student;
            using (SCPPContext context = new SCPPContext())
            {
                student = context.Estudiante.FirstOrDefault(s => s.Matricula == actualStudent.Matricula);

                student.Nombre = TextBoxName.Text;
                student.Apellidopaterno = TextBoxApellidoPaterno.Text;
                student.Apellidomaterno = TextBoxApellidoMaterno.Text;
                student.Telefono = TextBoxPhone.Text;
                student.Correopersonal = TextBoxEmail.Text;
                student.Estado = ComboBoxState.Text;

                context.SaveChanges();
            }
            actualStudent = student;
            return student;
        }

        private bool VerificateFields()
        {
            return FieldsVerificator.VerificateEmail(TextBoxEmail.Text)
                && FieldsVerificator.VerificateName(TextBoxName.Text)
                && FieldsVerificator.VerificateName(TextBoxApellidoPaterno.Text)
                && FieldsVerificator.VerificateName(TextBoxApellidoMaterno.Text)
                && FieldsVerificator.VerificatePhone(TextBoxPhone.Text);
        }
    }
}