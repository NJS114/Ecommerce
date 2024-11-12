
using System;

namespace  Ecommerce.Services.DAO.Connexion
{
    public class ConnexionManager
    {
        private static ConnexionManager _instance;
        public string ConnectionString { get; private set; }
        private static readonly object _lock = new object();
        private ConnexionManager()
        {
            ConnectionString = GetConnectionString();
        }
        public static ConnexionManager Instance
        {
            get
            {
               
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConnexionManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public static string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if(string.IsNullOrEmpty(connectionString) )
            {
                throw new InvalidOperationException("La chaîne de connexion est introuvable dans les variables d'environnement.");
            }
            return connectionString;
        }
        
    }
}
