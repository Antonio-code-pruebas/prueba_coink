using AntonioL.Logic.Interfaces;
using AntonioL.Models.PruebaCoink;
using AntonioL.Share.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntonioL.Api.Controllers
{
    /// <summary>
    /// Controlador para acceder a la información de los paises
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaisesController : ControllerBase
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
        public PaisesController(IUnitOfWork<AntonioLContext> unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Trae todos los paises del sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var paises = await _unitOfWork.Repository<Pais>().GetAllAsync();
            var data = _mapper.Map<IReadOnlyList<Pais>, IReadOnlyList<PaisDto>>(paises);
            return Ok(data);
        }
    }
}
