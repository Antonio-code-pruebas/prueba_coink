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
    }
}
