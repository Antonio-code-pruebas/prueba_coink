using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Share.Dtos
{
    public class MunicipioLocalizacionDto
    {
        public int PaisId { get; set; }

        public string PaisCodigo { get; set; } = null!;

        public string? PaisNombre { get; set; }

        public int DepartamentoId { get; set; }

        public string? DepartamentoNombre { get; set; }

        public string? DepartamentoCodigo { get; set; }

        public int MunicipioId { get; set; }

        public string? MunicipioCodigo { get; set; }

        public string? MunicipioNombre { get; set; }
    }
}
