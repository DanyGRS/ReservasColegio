using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasColegio.Models
{
    [Table("Login")]
    public class Login
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo de login é obrigatório.")]
        [StringLength(50, ErrorMessage = "O login deve ter no máximo 50 caracteres.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(255, ErrorMessage = "A senha deve ter no máximo 255 caracteres.")]
        public string Senha { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }
    }
}
