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

        public static bool VerificateName(string name)
        {
            Regex rgx = new Regex(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$");
            if (rgx.IsMatch(name))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de que el nombre y apellidos solo contenga datos alfabéticos", "Error nombre inválido", "Aceptar");
                return false;
            }
        }

        public static bool VerificateWorkerNumber(string workerNumber)
        {
            Regex rgx = new Regex("^[0-9]+$");
            if (rgx.IsMatch(workerNumber))
                return true;
            else
            {
                CustomMessageBox.ShowOK("Asegurese de ingresar un numero de trabajador válido", "Error de formato de RFC", "Aceptar");
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