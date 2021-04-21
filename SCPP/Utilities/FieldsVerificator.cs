using System;
using System.Text.RegularExpressions;
using WPFCustomMessageBox;

namespace SCPP.Utilities
{
    internal class FieldsVerificator
    {
        public static bool VerificateEmail(string email)
        {
            string emailFormat = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, emailFormat))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de introducir un correo valido.", "Error de formato de correo", "Aceptar");
                return false;
            }
        }

        public static bool VerificateMatricula(string matricula)
        {
            Regex rgx = new Regex(@"^[S]\d{7}[a-zA-Z0-9]$");
            if (rgx.IsMatch(matricula))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese que la matricula es una S seguida de 8 números", "Error de formato de matricula", "Aceptar");
                return false;
            }
        }

        public static bool VerificatePhone(string phoneNumber)
        {
            Regex rgx = new Regex(@"^\+?[\d- ]{9,}$");
            if (rgx.IsMatch(phoneNumber))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de ingresar un numero de telefono correcto", "Error de formato de telefono", "Aceptar");
                return false;
            }
        }

        public static bool VerificatePromedio(string promedio)
        {
            double average = 0;
            bool IsDouble = false;

            try
            {
                average = Convert.ToDouble(promedio);
                IsDouble = true;
            }
            catch (Exception)
            {
            }

            Regex rgx = new Regex(@"^((\d+)((\.\d{1,2})?))$");
            if (rgx.IsMatch(promedio) && average <= 10 && IsDouble)
                return true;
            else
            {
                CustomMessageBox.ShowOK("El promedio no tiene el formato correcto", "Error de formato de promedio", "Aceptar");
                return false;
            }
        }
    }
}