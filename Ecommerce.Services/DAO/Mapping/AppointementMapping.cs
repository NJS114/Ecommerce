using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Mapping
{
    public class AppointementMapping
    {
        public static AppointmentDTO ToDTO(Appointment appointment)
        {
            if (appointment == null) return null;

            return new AppointmentDTO
            {
                Id = appointment.Id,
                StartDate = appointment.StartDate,
                EndDate = appointment.EndDate,
                Title = appointment.Title,
                Description = appointment.Description,
                Location = appointment.Location,
                IsConfirmed = appointment.IsConfirmed,
                CustomerId = appointment.CustomerId
            };
        }

        // Méthode pour convertir un DTO en objet Appointment
        public static Appointment ToModel(AppointmentDTO appointmentDto)
        {
            if (appointmentDto == null) return null;

            return new Appointment
            {
                Id = appointmentDto.Id,
                StartDate = appointmentDto.StartDate,
                EndDate = appointmentDto.EndDate,
                Title = appointmentDto.Title,
                Description = appointmentDto.Description,
                Location = appointmentDto.Location,
                IsConfirmed = appointmentDto.IsConfirmed,
                CustomerId = appointmentDto.CustomerId
            };
        }

        // Méthode pour convertir une liste d'Appointments en une liste de DTOs
        public static IEnumerable<AppointmentDTO> ToDTOList(IEnumerable<Appointment> appointments)
        {
            return appointments?.Select(appointment => ToDTO(appointment)).ToList() ?? new List<AppointmentDTO>();
        }

        // Méthode pour convertir une liste de DTOs en une liste d'Appointments
        public static IEnumerable<Appointment> ToModelList(IEnumerable<AppointmentDTO> appointmentDtos)
        {
            return appointmentDtos?.Select(dto => ToModel(dto)).ToList() ?? new List<Appointment>();
        }
    }
}
