using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OLXAsp.Models.Dapper
{
    public class Category
    {
        public int Category_Id { get; set; }
        public string Category_Name { get; set; }
    }

    public static class CategorySingleton
    {
        private static CategoryRepository repository;

        public static CategoryRepository Repository
        {
            get 
            {
                if (repository == null)
                    repository = new CategoryRepository(connStr);

                return repository; 
            }
            private set { repository = value; }
        }

        private static string connStr;

        public static void Init(IConfiguration configuration) 
            => connStr = configuration.GetConnectionString("connStr");
    }


    public class CategoryRepository
    {
        private readonly string connStr;

        public List<Category> SelectCategory()
        {
            const string procedure = "EXEC [SelectCategory]";
            List<Category> categories;

            using (IDbConnection db = new SqlConnection(connStr))
            {
                db.Open();

                try
                {
                    categories = db.Query<Category>(procedure).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return categories;
        }

        public void DeleteCategory(Category category)
        {
            const string procedure = "EXEC [DeleteCategory] @Category_Id";
            var values = new { Category_Id = category.Category_Id };

            using (IDbConnection db = new SqlConnection(connStr))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Query(procedure, values, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex) 
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void UpdateCategory(Category category)
        {
            const string procedure = "EXEC [DeleteCategory] @Category_Name";
            var values = new { Category_Name = category.Category_Name };

            using (IDbConnection db = new SqlConnection(connStr))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Query(procedure, values, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public CategoryRepository(string connStr) => this.connStr = connStr;
    }
}
