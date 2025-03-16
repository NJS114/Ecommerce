using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Mapping;
using Ecommerce.Services.DAO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IAppointmentDAO _appointmentDAO;

        public AppointmentRepository(IAppointmentDAO appointmentDAO)
        {
            _appointmentDAO = appointmentDAO;
        }

        #region Méthodes CRUD
        public async Task<AppointmentDTO> GetAppointmentByIdAsync(string id)
        {
            var appointment = await _appointmentDAO.GetAppointmentById(id);
            if (appointment == null) return null;

            var appointmentDTO = new AppointmentDTO
            {
                Id = appointment.Id,
                StartDate = appointment.StartDate,
                EndDate = appointment.EndDate,
                Title = appointment.Title,
                Description = appointment.Description,
                Location = appointment.Location,
                IsConfirmed = appointment.IsConfirmed,
                CustomerId = appointment.CustomerId,
                User = UserSummaryMapper.ToDto(appointment.User)
            };

            return appointmentDTO;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAllAppointmentDTO()
        {
            var appointments = await _appointmentDAO.GetAllAppointments();

            var appointmentDTOs = appointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Title = a.Title,
                Description = a.Description,
                Location = a.Location,
                IsConfirmed = a.IsConfirmed,
                CustomerId = a.CustomerId,
                User = UserSummaryMapper.ToDto(a.User)
            }).ToList();

            return appointmentDTOs;
        }

        public async Task<AppointmentDTO> CreateAppointmentDTO(AppointmentDTO appointmentDTO)
        {
            var appointment = new Appointment
            {
                StartDate = appointmentDTO.StartDate,
                EndDate = appointmentDTO.EndDate,
                Title = appointmentDTO.Title,
                Description = appointmentDTO.Description,
                Location = appointmentDTO.Location,
                IsConfirmed = appointmentDTO.IsConfirmed,
                CustomerId = appointmentDTO.CustomerId,
                User = UserSummaryMapper.ToModel(appointmentDTO.User)
            };

            await _appointmentDAO.CreateAppointment(appointment);

            return new AppointmentDTO
            {
                Id = appointment.Id,
                StartDate = appointmentDTO.StartDate,
                EndDate = appointmentDTO.EndDate,
                Title = appointmentDTO.Title,
                Description = appointmentDTO.Description,
                Location = appointmentDTO.Location,
                IsConfirmed = appointmentDTO.IsConfirmed,
                CustomerId = appointmentDTO.CustomerId,
                User = appointmentDTO.User
            };
        }

        public async Task<AppointmentDTO> UpdateAppointmentDTO(AppointmentDTO appointmentDTO)
        {
            var appointment = new Appointment
            {
                Id = appointmentDTO.Id,
                StartDate = appointmentDTO.StartDate,
                EndDate = appointmentDTO.EndDate,
                Title = appointmentDTO.Title,
                Description = appointmentDTO.Description,
                Location = appointmentDTO.Location,
                IsConfirmed = appointmentDTO.IsConfirmed,
                CustomerId = appointmentDTO.CustomerId,
                User = UserSummaryMapper.ToModel(appointmentDTO.User)
            };

            await _appointmentDAO.UpdateAppointment(appointment);

            return new AppointmentDTO
            {
                Id = appointment.Id,
                StartDate = appointmentDTO.StartDate,
                EndDate = appointmentDTO.EndDate,
                Title = appointmentDTO.Title,
                Description = appointmentDTO.Description,
                Location = appointmentDTO.Location,
                IsConfirmed = appointmentDTO.IsConfirmed,
                CustomerId = appointmentDTO.CustomerId,
                User = appointmentDTO.User
            };
        }

        public async Task<AppointmentDTO> DeleteAppointmentDTO(string id)
        {
            var appointment = await _appointmentDAO.GetAppointmentById(id);
            if (appointment == null) throw new KeyNotFoundException($"Appointment avec ID {id} introuvable.");

            await _appointmentDAO.DeleteAppointment(id);

            return new AppointmentDTO
            {
                Id = appointment.Id,
                StartDate = appointment.StartDate,
                EndDate = appointment.EndDate,
                Title = appointment.Title,
                Description = appointment.Description,
                Location = appointment.Location,
                IsConfirmed = appointment.IsConfirmed,
                CustomerId = appointment.CustomerId,
                User = UserSummaryMapper.ToDto(appointment.User)
            };
        }
        #endregion

        #region Méthodes de Recherche
        public async Task<IEnumerable<AppointmentDTO>> SearchAppointments(string query)
        {
            var appointments = await _appointmentDAO.GetAllAppointments();
            var filteredAppointments = appointments.Where(a => a.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                                                a.Description.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            return filteredAppointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Title = a.Title,
                Description = a.Description,
                Location = a.Location,
                IsConfirmed = a.IsConfirmed,
                CustomerId = a.CustomerId,
                User = UserSummaryMapper.ToDto(a.User)
            });
        }
        #endregion
    }
}
