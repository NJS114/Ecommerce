using Ecommerce.Services.DAO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IAppointmentDAO
    {
        Task<Appointment> CreateAppointment(Appointment appointment);
        Task<List<Appointment>> GetAllAppointments();
        Task<Appointment> GetAppointmentById(string id);
        Task<Appointment> UpdateAppointment(Appointment appointment);
        Task<Appointment> DeleteAppointment(string id);
    }
}
