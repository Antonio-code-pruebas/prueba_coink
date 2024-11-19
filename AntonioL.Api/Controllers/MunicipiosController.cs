using AntonioL.Logic.Interfaces;
using AntonioL.Models.PruebaCoink;
using AntonioL.Share.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace AntonioL.Api.Controllers
{
    /// <summary>
    /// Controlador para acceder a la información de municipios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MunicipiosController : ControllerBase
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
        public MunicipiosController(IUnitOfWork<AntonioLContext> unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Trae todos los municipios del sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var municipios = await _unitOfWork.Repository<Municipio>().GetAllAsync();
            var data = _mapper.Map<IReadOnlyList<Municipio>, IReadOnlyList<MunicipioDto>>(municipios);
            return Ok(data);
        }

        /// <summary>
        /// Obtener pais, dpto, municipio para cualquier municipio
        /// </summary>
        /// <param name="nombreMunicipio">puede digitar una parte o todo el nombre</param>
        /// <returns></returns>
        [HttpGet("obtenerMunicipiosFunc")]
        public async Task<IActionResult> ObtenerMunicipios_func([FromQuery] string nombreMunicipio)
        {
            try
            {
                // Crear el parámetro para la función
                var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("municipio_nombre_param", NpgsqlDbType.Text)
                {
                    Value = nombreMunicipio ?? string.Empty
                }
            };

                // Llamar a la función usando el UnitOfWork
                var municipios = await _unitOfWork.Repository<MunicipioLocalizacionDto>()
                    .ExecuteFunctionAsync<MunicipioLocalizacionDto>("prueba_coink.function_obtener_municipios", parameters.ToArray());

                if (municipios == null || municipios.Count == 0)
                    return NotFound("No se encontraron municipios.");

                return Ok(municipios);
            }
            catch (Exception ex)
            {
                // Manejar excepciones y devolver una respuesta de error
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error al obtener los municipios: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida un conjunto de ids (pais, dpto, mncipio)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("validarLocalizacion")]
        public async Task<IActionResult> ValidarLocalizacion([FromBody] LocalizacionDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            try
            {

                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("_pais_id", NpgsqlDbType.Integer) { Value = dto.PaisId },
                    new NpgsqlParameter("_departamento_id", NpgsqlDbType.Integer) { Value = dto.DepartamentoId },
                    new NpgsqlParameter("_municipio_id", NpgsqlDbType.Integer) { Value = dto.MunicipioId }
                };

                var mensaje = await _unitOfWork.Repository<String>().ExecuteStoredProcedureAsync("prueba_coink.validar_localizacion", parameters.ToArray());

                return Ok(new { mensaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
