using System;
using System.Windows;
using System.Windows.Controls;

namespace SCPP
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            Loaded += OnMainWindowLoaded;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ChangeView(new MenuCoordinador());
        }

        public void ChangeView(Page view)
        {
            FrameContent.NavigationService.Navigate(view);
        }
    }
}
