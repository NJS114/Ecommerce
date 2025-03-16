using Ecommerce.Services.DAO.Connexion;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Models;
using MongoDB.Driver;
using MongoFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMongoDbConnection = Ecommerce.Services.DAO.Interfaces.IRepository.IMongoDbConnection;

namespace Ecommerce.Services.DAO.Implementations
{
    public class AppointmentDAO : IAppointmentDAO
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentDAO(IMongoDbConnection mongoConnection)
        {
            var database = mongoConnection.GetDatabase();
            _appointments = database.GetCollection<Appointment>("Appointments");
        }

        public async Task<Appointment> CreateAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment), "Le rendez-vous ne peut pas être nul.");
            }

            appointment.CreatedAt = DateTime.UtcNow;
            await _appointments.InsertOneAsync(appointment);
            return appointment;
        }

        public async Task<List<Appointment>> GetAllAppointments()
        {
            return await _appointments.Find(_ => true).ToListAsync();
        }

        public async Task<Appointment> GetAppointmentById(string id)
        {
            var appointment = await _appointments.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Rendez-vous avec ID {id} introuvable.");
            }
            return appointment;
        }

        public async Task<Appointment> UpdateAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment), "Le rendez-vous ne peut pas être nul.");
            }

            var update = Builders<Appointment>.Update
                .Set(a => a.StartDate, appointment.StartDate)
                .Set(a => a.EndDate, appointment.EndDate)
                .Set(a => a.Title, appointment.Title)
                .Set(a => a.Description, appointment.Description)
                .Set(a => a.Location, appointment.Location)
                .Set(a => a.IsConfirmed, appointment.IsConfirmed)
                .Set(a => a.CustomerId, appointment.CustomerId);

            var result = await _appointments.UpdateOneAsync(a => a.Id == appointment.Id, update);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Rendez-vous avec ID {appointment.Id} introuvable.");
            }

            return appointment;
        }

        // Supprimer un rendez-vous
        public async Task<Appointment> DeleteAppointment(string id)
        {
            var appointment = await _appointments.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (appointment != null)
            {
                await _appointments.DeleteOneAsync(a => a.Id == id); 
            }
            else
            {
                throw new KeyNotFoundException($"Rendez-vous avec ID {id} introuvable.");
            }
            return appointment;
        }
    }
}
