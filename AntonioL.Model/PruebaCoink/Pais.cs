using System;
using System.Collections.Generic;

namespace AntonioL.Models.PruebaCoink;

public partial class Pais
{
    public int PaisId { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}
