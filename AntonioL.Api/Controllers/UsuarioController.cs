using AntonioL.Logic.Interfaces;
using AntonioL.Models.PruebaCoink;
using AntonioL.Share.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AntonioL.Api.Controllers
{
    /// <summary>
    /// Controlador para usuario
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUnitOfWork<AntonioLContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        public UsuarioController(IUnitOfWork<AntonioLContext> unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Registrar usuario, incluye validar localización
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost("registrarUsuario")]
        public async Task<ActionResult<UsuarioDto>> CreateAsync(UsuarioDto usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Validar localización antes de crear el usuario/ sino es válido impide el registro del usuario
                var localizacionDto = new LocalizacionDto
                {
                    PaisId = usuario.PaisId,
                    DepartamentoId = usuario.DepartamentoId,
                    MunicipioId = usuario.MunicipioId
                };

                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("_pais_id", NpgsqlDbType.Integer) { Value = localizacionDto.PaisId },
                    new NpgsqlParameter("_departamento_id", NpgsqlDbType.Integer) { Value = localizacionDto.DepartamentoId },
                    new NpgsqlParameter("_municipio_id", NpgsqlDbType.Integer) { Value = localizacionDto.MunicipioId }
                };

                var mensaje = await _unitOfWork.Repository<string>()
                    .ExecuteStoredProcedureAsync("prueba_coink.validar_localizacion", parameters.ToArray());

                if (mensaje?.ToString().Contains("valida") == false) // Verificar el mensaje de validación
                {
                    return BadRequest(new { error = "Localización inválida", mensaje });
                }



                var data = _mapper.Map<UsuarioDto, Usuario>(usuario);

                _unitOfWork.Repository<Usuario>().AddEntity(data);
                await _unitOfWork.Complete();

                await _unitOfWork.Repository<Usuario>().Update(data);
                await _unitOfWork.Complete();

                usuario.UsuarioId = data.UsuarioId;

                //return CreatedAtAction(nameof(GetByIdAsync), new { id = data.IdElemento }, data);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Function para obtener usuarios por filtro
        /// </summary>
        /// <param name="filtro">puede enviar uno o todos los valores, deje vacio ("") o en 0 los que no enviará a filtrar</param>
        /// <returns></returns>
        [HttpPost("obtenerUsuarios")]
        public async Task<IActionResult> ObtenerUsuarios_func(FiltroUsuarios filtro)
        {
            try
            {
                // Crear el parámetro para la función
                var parameters = new List<NpgsqlParameter>
                {
                    // Para 'nombre' y 'telefono', asignamos DBNull.Value si son cadenas vacías
                    new NpgsqlParameter("_nombre", NpgsqlDbType.Text)
                    {
                        Value = string.IsNullOrEmpty(filtro.Nombre) ? DBNull.Value : (object)filtro.Nombre
                    },
                    new NpgsqlParameter("_telefono", NpgsqlDbType.Text)
                    {
                        Value = string.IsNullOrEmpty(filtro.Telefono) ? DBNull.Value : (object)filtro.Telefono
                    },
                    // Para los valores enteros, asignamos DBNull.Value si son 0
                    new NpgsqlParameter("_pais_id", NpgsqlDbType.Integer)
                    {
                        Value = filtro.PaisId == 0 ? DBNull.Value : (object)filtro.PaisId
                    },
                    new NpgsqlParameter("_departamento_id", NpgsqlDbType.Integer)
                    {
                        Value = filtro.DepartamentoId == 0 ? DBNull.Value : (object)filtro.DepartamentoId
                    },
                    new NpgsqlParameter("_municipio_id", NpgsqlDbType.Integer)
                    {
                        Value = filtro.MunicipioId == 0 ? DBNull.Value : (object)filtro.MunicipioId
                    }
                };

                // Llamar a la función usando el UnitOfWork
                var municipios = await _unitOfWork.Repository<ResponseFiltroUsuarios>()
                    .ExecuteFunctionAsync<ResponseFiltroUsuarios>("prueba_coink.filtrar_usuarios", parameters.ToArray());

                if (municipios == null || municipios.Count == 0)
                    return NotFound("No se encontraron usuarios.");

                return Ok(municipios);
            }
            catch (Exception ex)
            {
                // Manejar excepciones y devolver una respuesta de error
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error al obtener los usuarios: {ex.Message}");
            }
        }
    }
}
