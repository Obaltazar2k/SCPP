using WPFCustomMessageBox;

namespace SCPP.Utilities
{
    internal class Restarter
    {
        public static void RestarSCPP()
        {
            CustomMessageBox.ShowOK("Ocurrió un error en la conexión con la base de datos. Por favor intentelo más tarde.",
                    "Fallo en conexión con la base de datos", "Aceptar");
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }
    }
}