using System;
using System.Collections.Generic;

namespace AntonioL.Models.PruebaCoink;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string? Nombre { get; set; }

    public string? Telefono { get; set; }

    public int? PaisId { get; set; }

    public int? DepartamentoId { get; set; }

    public int? MunicipioId { get; set; }

    public string? Direccion { get; set; }
}
