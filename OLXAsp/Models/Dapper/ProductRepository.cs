using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OLXAsp.Models.Dapper
{
    public class Product
    {
        public int Product_Id { get; set; }
        public string Product_ImageURL { get; set; }
        public string Product_Description { get; set; }
        public decimal Product_Price { get; set; }
        public int Product_CategoryId { get; set; }
    }
    public static class ProductSingleton
    {
        private static ProductRepository repository;

        public static ProductRepository Repository
        {
            get
            {
                if (repository == null)
                    repository = new ProductRepository(connStr);

                return repository;
            }
            private set { repository = value; }
        }

        private static string connStr;

        public static void Init(IConfiguration configuration) 
            => connStr = configuration.GetConnectionString("connStr");       
    }
    public class ProductRepository
    {
        private readonly string connStr;

        public List<Product> SelectProduct()
        {
            const string procedure = "EXEC [SelectProduct]";
            List<Product> products;

            using (IDbConnection db = new SqlConnection(connStr))
            {
                db.Open();

                try
                {
                    products = db.Query<Product>(procedure).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return products;
        }

        public void DeleteProduct(Product product)
        {
            const string procedure = "EXEC [DeleteProduct] @Product_Id";
            var values = new { Product_Id = product.Product_Id };

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

        public void InsertProduct(Product product)
        {
            const string procedure = "EXEC [InsertProduct] @Product_ImageURL, @Product_Description, @Product_Price, @Product_CategoryId";
            var values = new 
            { 
                Product_ImageURL = product.Product_ImageURL, 
                Product_Description = product.Product_Description, 
                Product_Price = product.Product_Price, 
                Product_CategoryId = product.Product_CategoryId 
            };

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

        public void UpdateProduct(Product product)
        {
            const string procedure = "EXEC [InsertProduct] @Product_Id, @Product_ImageURL, @Product_Description, @Product_Price";
            var values = new
            {
                Product_Id = product.Product_Id,
                Product_ImageURL = product.Product_ImageURL,
                Product_Description = product.Product_Description,
                Product_Price = product.Product_Price
            };

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

        public ProductRepository(string connStr) => this.connStr = connStr;
    }
}
