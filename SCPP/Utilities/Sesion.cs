﻿namespace SCPP.Utilities
{
    class Sesion
    {
        private static Sesion _userSesion = null;

        public string Username { get; set; }
        public string Email { get; set; }

        public static Sesion GetSesion
        {
            get
            {
                if (_userSesion == null)
                {
                    _userSesion = new Sesion();
                }
                return _userSesion;
            }
        }
    }
}