using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasColegio.Models
{
    [Table("Diretiva")]
    public class Diretiva
    {
        [Key]
        public int Id { get; set; }
        [Display (Name = "Nome")]
        [Required(ErrorMessage = "O nome da diretiva é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo {0} caracteres.")]
        public string Nome { get; set; }

        public ICollection<DiretivaPermissao> DiretivaPermissoes { get; set; } = new List<DiretivaPermissao>();

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
