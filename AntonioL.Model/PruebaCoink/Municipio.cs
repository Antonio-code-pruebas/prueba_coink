using System;
using System.Collections.Generic;

namespace AntonioL.Models.PruebaCoink;

public partial class Municipio
{
    public int MunicipioId { get; set; }

    public int DepartamentoId { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public virtual Departamento Departamento { get; set; } = null!;
}
