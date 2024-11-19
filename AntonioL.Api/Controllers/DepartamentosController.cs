using AntonioL.Logic.Interfaces;
using AntonioL.Models.PruebaCoink;
using AntonioL.Share.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntonioL.Api.Controllers
{
    /// <summary>
    /// Controlador para acceder a la información de Departamentos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentosController : ControllerBase
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
        public DepartamentosController(IUnitOfWork<AntonioLContext> unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Trae todos los departamentos del sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departamentos = await _unitOfWork.Repository<Departamento>().GetAllAsync();
            var data = _mapper.Map<IReadOnlyList<Departamento>, IReadOnlyList<DepartamentoDto>>(departamentos);
            return Ok(data);
        }
    }
}
