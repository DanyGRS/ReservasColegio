using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasColegio.Models
{
    [Table("Permissao")]
    public class Permissao
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome da permissão é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        public ICollection<DiretivaPermissao> DiretivaPermissoes { get; set; } = new List<DiretivaPermissao>();
    }
}
