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
    public class ArticleDAO : IArticleDAO
    {
        private readonly IMongoCollection<Article> _articles;

        public ArticleDAO(IMongoDbConnection mongoConnection)
        {
            var database = mongoConnection.GetDatabase();
            _articles = database.GetCollection<Article>("Articles");
        }

        // Créer un article
        public async Task<Article> CreateArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article), "L'article ne peut pas être nul.");
            }

            article.PublishedDate = DateTime.UtcNow;
            await _articles.InsertOneAsync(article);
            return article;
        }

        // Obtenir tous les articles
        public async Task<List<Article>> GetAllArticles()
        {
            return await _articles.Find(_ => true).ToListAsync();
        }

        // Obtenir un article par son ID
        public async Task<Article> GetArticleById(string id)
        {
            var article = await _articles.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (article == null)
            {
                throw new KeyNotFoundException($"Article avec ID {id} introuvable.");
            }
            return article;
        }

        // Mettre à jour un article
        public async Task<Article> UpdateArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article), "L'article ne peut pas être nul.");
            }

            var update = Builders<Article>.Update
                .Set(a => a.Title, article.Title)
                .Set(a => a.Content, article.Content)
                .Set(a => a.Author, article.Author)
                .Set(a => a.PublishedDate, article.PublishedDate)
                .Set(a => a.ImageUrl, article.ImageUrl)
                .Set(a => a.Category, article.Category);

            var result = await _articles.UpdateOneAsync(a => a.Id == article.Id, update);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Article avec ID {article.Id} introuvable.");
            }

            return article;
        }

        // Supprimer un article
        public async Task<Article> DeleteArticle(string id)
        {
            var article = await _articles.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (article != null)
            {
                await _articles.DeleteOneAsync(a => a.Id == id); // Suppression de l'article
            }
            else
            {
                throw new KeyNotFoundException($"Article avec ID {id} introuvable.");
            }
            return article;
        }
    }
}
