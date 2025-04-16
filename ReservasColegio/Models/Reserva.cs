using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasColegio.Models
{
    [Table("Reserva")]
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Required]
        public int EquipamentoId { get; set; }

        [ForeignKey("EquipamentoId")]
        public Equipamento Equipamento { get; set; }

        [Required(ErrorMessage = "A data da reserva é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataReserva { get; set; }

        [Required(ErrorMessage = "A hora de início é obrigatória.")]
        [DataType(DataType.Time)]
        public DateTime HoraInicio { get; set; }

        [Required(ErrorMessage = "A hora de término é obrigatória.")]
        [DataType(DataType.Time)]
        public DateTime HoraFim { get; set; }

        [Required]
        public StatusReserva Status { get; set; } = StatusReserva.Pendente;
    }

    public enum StatusReserva
    {
        Pendente,
        Aprovada,
        Cancelada,
        Expirada,
        EmUso,
        Concluida
    }
}
