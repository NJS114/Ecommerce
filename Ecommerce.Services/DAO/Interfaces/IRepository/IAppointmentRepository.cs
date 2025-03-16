using Ecommerce.Services.DAO.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IAppointmentRepository
    {
        Task<AppointmentDTO> GetAppointmentByIdAsync(string id);
        Task<IEnumerable<AppointmentDTO>> GetAllAppointmentDTO();
        Task<AppointmentDTO> CreateAppointmentDTO(AppointmentDTO appointmentDTO);
        Task<AppointmentDTO> UpdateAppointmentDTO(AppointmentDTO appointmentDTO);
        Task<AppointmentDTO> DeleteAppointmentDTO(string id);
        Task<IEnumerable<AppointmentDTO>> SearchAppointments(string query);
    }
}
