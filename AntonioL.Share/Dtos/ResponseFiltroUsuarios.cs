using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonioL.Share.Dtos
{
    public class ResponseFiltroUsuarios
    {
        public int usuario_id { get; set; }

        public string? nombre { get; set; }

        public string? telefono { get; set; }

        public int? pais_id { get; set; }

        public int? departamento_id { get; set; }

        public int? municipio_id { get; set; }

        public string? direccion { get; set; }
    }
}
