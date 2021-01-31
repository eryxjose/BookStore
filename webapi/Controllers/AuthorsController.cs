using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using webapi.Contracts;
using AutoMapper;
using webapi.DTOs;
using webapi.Data;
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

        /// <summary>
        /// Creates an author
        /// </summary>
        /// <param name="authorCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorCreateDTO) 
        {
            try
            {
                // ModelState restaura as informações preenchidas para o form
                if (authorCreateDTO == null) 
                {
                    _logger.LogWarn($"Empty request.");
                    return BadRequest(ModelState);
                }
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Verifique os dados e tente novamente.");
                    return BadRequest(ModelState);
                }
                
                var author = _mapper.Map<Author>(authorCreateDTO);
                
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return InternalError($"Failed to create Author.");
                }

                _logger.LogInfo("Author created.");
                return Created("Create", new { author });
            }
            catch (Exception ex) 
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Update an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO) 
        {
            try
            {
                _logger.LogInfo($"Attempt to update Author id: {id}");
                if (id < 1 || authorDTO == null || id != authorDTO.Id) 
                {
                    _logger.LogWarn($"Empty request.");
                    return BadRequest(ModelState);
                }
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Verifique os dados e tente novamente.");
                    return BadRequest(ModelState);
                }
                
                var author = _mapper.Map<Author>(authorDTO);
                
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"Failed to update Author.");
                }

                _logger.LogInfo("Author updated.");
                return NoContent();

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