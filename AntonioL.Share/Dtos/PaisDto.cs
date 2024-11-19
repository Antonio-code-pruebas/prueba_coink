using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Share.Dtos
{
    public class PaisDto
    {
        public int PaisId { get; set; }

        public string Codigo { get; set; } = null!;

        public string? Nombre { get; set; }
    }
}
