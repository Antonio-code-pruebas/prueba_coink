using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Share.Dtos
{
    public class DepartamentoDto
    {
        public int DepartamentoId { get; set; }

        public int PaisId { get; set; }

        public string? Nombre { get; set; }

        public string? Codigo { get; set; }
    }
}
