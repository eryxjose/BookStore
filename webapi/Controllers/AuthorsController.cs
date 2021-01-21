using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using webapi.Contracts;
using AutoMapper;
using webapi.DTOs;
using System.Threading.Tasks;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {

        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>All authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors() 
        {
            try 
            {
                _logger.LogInfo("Getting authors.");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                return Ok(response);
            }
            catch (Exception ex) 
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Get an author by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An author's record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id) 
        {
            try 
            {
                _logger.LogInfo($"Getting author by id. Id: {id}");
                var author = await _authorRepository.FindById(id);
                if (author == null) 
                {
                    _logger.LogWarn($"Author com id {id} não foi encontrado.");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                return Ok(response);
            }
            catch (Exception ex) 
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO) 
        {
            try
            {
                // ModelState restaura as informações preenchidas para o form
                if (authorDTO == null) 
                {
                    _logger.LogWarn($"Empty request.");
                    return BadRequest(ModelState);
                }
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Verifique os dados e tente novamente.");
                    return BadRequest(ModelState);
                }
                
                var author = _mapper.Map<AuthorCreateDTO>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
            }
            catch (Exception ex) 
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        private ObjectResult InternalError(string message) 
        {
            _logger.LogError(message);
            return StatusCode(500, "Something wrong happened. Contact support.");
        }
    }
}