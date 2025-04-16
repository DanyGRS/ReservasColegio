using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasColegio.Models
{
    [Table("DiretivaPermissao")]
    public class DiretivaPermissao
    {
        [Key]
        public int DiretivaPermissaoId {get; set;}
        public int DiretivaId { get; set; }

        [ForeignKey("DiretivaId")]
        public Diretiva Diretiva { get; set; }

        public int PermissaoId { get; set; }

        [ForeignKey("PermissaoId")]
        public Permissao Permissao { get; set; }
    }
}
