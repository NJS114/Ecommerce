using Ecommerce.Services.DAO.Repositories;
using Ecommerce.Services.DAO.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using System.Threading.Tasks;
using System.Linq;
using Ecommerce.Services.DAO.Interfaces.IDAO;

namespace Ecommerce.Services.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentController(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        #region GET Methods
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _repository.GetAllAppointmentDTO();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(string id)
        {
            var appointments = await _repository.GetAllAppointmentDTO();
            var foundAppointment = appointments.FirstOrDefault(a => a.Id == id);

            if (foundAppointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            return Ok(foundAppointment);
        }
        #endregion

        #region POST Methods
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null)
            {
                return BadRequest("Invalid appointment data.");
            }

            var createdAppointment = await _repository.CreateAppointmentDTO(appointmentDTO);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = createdAppointment.Id }, createdAppointment);
        }
        #endregion

        #region PUT Methods
        [HttpPut]
        public async Task<IActionResult> UpdateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null || appointmentDTO.Id == "0")
            {
                return BadRequest("Invalid appointment data.");
            }

            var updatedAppointment = await _repository.UpdateAppointmentDTO(appointmentDTO);
            return Ok(updatedAppointment);
        }
        #endregion

        #region DELETE Methods
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(string id)
        {
            try
            {
                var deletedAppointment = await _repository.DeleteAppointmentDTO(id);
                return Ok(deletedAppointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion
    }
}
