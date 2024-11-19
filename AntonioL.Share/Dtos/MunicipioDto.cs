using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Share.Dtos
{
    public class MunicipioDto
    {
        public int MunicipioId { get; set; }

        public int DepartamentoId { get; set; }

        public string? Codigo { get; set; }

        public string? Nombre { get; set; }
    }
}
