//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCPP
{
    using System;
    using System.Collections.Generic;
    
    public partial class Grupo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Grupo()
        {
            this.Inscripción = new HashSet<Inscripción>();
        }
    
        public string Bloque { get; set; }
        public Nullable<int> Cupo { get; set; }
        public Nullable<int> Nrc { get; set; }
        public string Seccion { get; set; }
        public int GrupoID { get; set; }
        public string Rfcprofesor { get; set; }
        public Nullable<int> PeriodoID { get; set; }
    
        public virtual Periodo Periodo { get; set; }
        public virtual Profesor Profesor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inscripción> Inscripción { get; set; }
    }
}
