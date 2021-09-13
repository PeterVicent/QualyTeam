using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualyTeam
{
    [Table("Documento")]
    public class Documento
    {
        [Key]
        [Required(ErrorMessage = "É necessário inserir um Código.")]
        [Display(Name = "Código")]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "É necessário inserir um Título.")]
        [Display(Name = "Título")]
        [Column("Titulo")]
        public string Titulo { get; set; }

        [Display(Name = "Processo")]
        [Column("NomeProcesso")]
        public string NomeProcesso { get; set; }

        [Required(ErrorMessage = "É necessário selecionar um Processo.")]
        [Display(Name = "Processo")]
        [Column("Processo")]
        public int Processo { get; set; }

        [Required(ErrorMessage = "É necessário inserir uma Categoria.")]
        [Display(Name = "Categoria")]
        [Column("Categoria")]
        public string Categoria { get; set; }

        [Display(Name = "Arquivo")]
        [Column("NomeDocumento")]
        public string NomeArquivo { get; set; }

        [Display(Name = "Arquivo")]
        [Column("Documento")]
        public byte[] Arquivo { get; set; }

        [Column("Extensao")]
        public string ExtensaoArquivo { get; set; }
    }

    [Table("Processo")]
    public class Processos
    {
        [Column("Processo")]
        public string Processo { get; set; }

        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }
    }
}
