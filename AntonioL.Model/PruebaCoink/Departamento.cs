using System;
using System.Collections.Generic;

namespace AntonioL.Models.PruebaCoink;

public partial class Departamento
{
    public int DepartamentoId { get; set; }

    public int PaisId { get; set; }

    public string? Nombre { get; set; }

    public string? Codigo { get; set; }

    public virtual ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();

    public virtual Pais Pais { get; set; } = null!;
}
