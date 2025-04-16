using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasColegio.Models
{
    [Table("Equipamento")]
    public class Equipamento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do equipamento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O tipo do equipamento é obrigatório.")]
        public TipoEquipamento Tipo { get; set; }

        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
        public string Descricao { get; set; }

        [StringLength(50, ErrorMessage = "A identificação patrimonial deve ter no máximo 50 caracteres.")]
        public string IdentificacaoPatrimonio { get; set; }

        public bool Disponivel { get; set; } = true;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

    public enum TipoEquipamento
    {
        Projetor,
        Notebook,
        CaixaDeSom,
        Microfone,
        Câmera,
        Tela,
        Tripé,
        CaboHDMI,
        Outro
    }
}
