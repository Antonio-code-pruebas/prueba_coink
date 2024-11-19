using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Share.Dtos
{
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }

        public string? Nombre { get; set; }

        public string? Telefono { get; set; }

        public int? PaisId { get; set; }

        public int? DepartamentoId { get; set; }

        public int? MunicipioId { get; set; }

        public string? Direccion { get; set; }
    }
}
