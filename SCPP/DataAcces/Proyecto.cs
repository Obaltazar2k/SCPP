//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCPP.DataAcces
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Proyecto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Proyecto()
        {
            this.Inscripción = new ObservableCollection<Inscripción>();
            this.Selecciónproyecto = new ObservableCollection<Selecciónproyecto>();
        }
    
        public string Actividades { get; set; }
        public int Clave { get; set; }
        public string Descripcion { get; set; }
        public Nullable<System.DateTime> Fecharegistro { get; set; }
        public Nullable<int> Noestudiantes { get; set; }
        public string Nombre { get; set; }
        public string Resbonsablenombre { get; set; }
        public Nullable<int> OrganizaciónID { get; set; }
        public Nullable<int> Activo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Inscripción> Inscripción { get; set; }
        public virtual Organización Organización { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Selecciónproyecto> Selecciónproyecto { get; set; }
    }
}
