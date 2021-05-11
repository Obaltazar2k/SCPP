using SCPP.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace SCPP.View
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

        public void ChangeView(Page view)
        {
            if (view is IniciarSesion)
                Sesion.CloseSesion();
            FrameContent.NavigationService.Navigate(view);
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ChangeView(new IniciarSesion());
        }
    }
}