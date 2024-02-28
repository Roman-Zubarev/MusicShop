using MusicShop.Models.ViewModels;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Org.BouncyCastle.Asn1.Pkcs;
using Raven.Database.Indexing;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using static Raven.Database.Indexing.IndexingWorkStats;

namespace MusicShop.Models
{
    public class BDWorks
    {

        public static User CurrentUser = null;

        public static bool IsAdmin = false;


        public static async Task<List<Product>> GetAllProducts()
        {
            //    OracleConnection.ClearAllPools();
            var product = new List<Product>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GAllProducts";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        product.Add( new Product
                        {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            Name = row["name"].ToString(),
                            Collection = row["Collection"].ToString(),
                            Description = row["Description"].ToString(),
                            Image = row["Image"].ToString(),
                            Price = Convert.ToDouble(row["Price"].ToString()),
                            Author = await BDWorks.GetAuthorById(Convert.ToInt32(row["Authorsid"].ToString())),
                            Count = Convert.ToInt32(row["Count"].ToString())
                        });
                    }
                }
                await connection.CloseAsync();
            }

            return product;

        }
        public static async Task<List<Author>> GetAllAuthors()
        {
         //     OracleConnection.ClearAllPools();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";
            List<Author> authors = new List<Author>();

            string commandText = "select * from GetAllAuthors";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        authors.Add(new Author
                        {
                            Id = reader.GetInt16(0),
                            Name = reader.GetString(1),
                            Country = reader.GetString(2),
                            Image = reader.GetString(3),
                            Biography = reader.GetString(4),
                        });
                    }
                }
                await connection.CloseAsync();
            }

            return authors;

        }

        public static async Task<List<ReservedProduct>> GetAllReservedProducts()
        {
            // OracleConnection.ClearAllPools();

            var product = new List<ReservedProduct>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GAllReserveProducts";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        product.Add(new ReservedProduct
                        {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            UserId = Convert.ToInt32(row["USERID"].ToString()),
                            Product = await BDWorks.GetProductById(Convert.ToInt32(row["ProductId"].ToString())),
                            ExpirationDate = (DateTime)row["ExpirationDate"],

                        });
                    }
                }
                await connection.CloseAsync();
            }

            return product;

        }

        public static async Task<List<string>> GetDistinctCollections()
        {
            
              // OracleConnection.ClearAllPools();
            List<string> collections = new List<string>();


            var product = new List<Product>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GDistinctProductCollection";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        collections.Add(row["Collection"].ToString());
                    }
                }
                await connection.CloseAsync();
            }

            return collections;
        }

        public static async Task<Author> GetAuthorById(int id)
        {
            //    OracleConnection.ClearAllPools();
            Author author = new Author();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetAuthorById";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                OracleParameter userID = new OracleParameter("author_id", OracleDbType.Int32);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = id;
                command.Parameters.Add(userID);

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        author = new Author
                        {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            Name = row["Name"].ToString(),
                            Country = row["Country"].ToString(),  
                            Image = row["Image"].ToString(),
                            Biography = row["Biography"].ToString(),
                 
                        };
                    }
                }
                await connection.CloseAsync();
            }

            return author;
        }


        public static async Task<List<Product>> GetProductsWithCollection(string collection)
        {

            List<Product> result = new List<Product>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetProductsWithCollection";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                OracleParameter userID = new OracleParameter("product_collection ", OracleDbType.Varchar2);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = collection;
                command.Parameters.Add(userID);

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Product product = new Product();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        product = new Product{
                            Id = Convert.ToInt32(row["id"].ToString()),
                            Name = row["name"].ToString(),
                            Collection = row["Collection"].ToString(),
                            Description = row["Description"].ToString(),
                            Image = row["Image"].ToString(),
                            Price = Convert.ToDouble(row["Price"].ToString()),
                            Author = await BDWorks.GetAuthorById(Convert.ToInt32(row["Authorsid"].ToString())),
                            Count = Convert.ToInt32(row["Count"].ToString())
                        };
                   
             
                        result.Add(product);

                    }
                }
                await connection.CloseAsync();
            }

            return result;
        }

    
        public static async Task<Product> GetProductById(int id)
        {
            //    OracleConnection.ClearAllPools();
            Product product = new Product();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetProductById";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                OracleParameter userID = new OracleParameter("product_id", OracleDbType.Int32);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = id;
                command.Parameters.Add(userID);

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        product = new Product
                        {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            Name = row["name"].ToString(),
                            Collection = row["Collection"].ToString(),
                            Description = row["Description"].ToString(),
                            Image = row["Image"].ToString(),
                            Price = Convert.ToDouble(row["Price"].ToString()),
                            Author = await BDWorks.GetAuthorById(Convert.ToInt32(row["Authorsid"].ToString())),
                            Count = Convert.ToInt32(row["Count"].ToString())
                        };               
                    }
                }
                await connection.CloseAsync();
            }

            return product;
        }


        public static async Task<List<Product>> GetNRandomProducts(int rand_count)
        {

            List<Product> result = new List<Product>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetNRandomProducts";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                OracleParameter userID = new OracleParameter("rand_count", OracleDbType.Int32);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = rand_count;
                command.Parameters.Add(userID);

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Product product = new Product();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        product = new Product
                        {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            Name = row["name"].ToString(),
                            Collection = row["Collection"].ToString(),
                            Description = row["Description"].ToString(),
                            Image = row["Image"].ToString(),
                            Price = Convert.ToDouble(row["Price"].ToString()),
                            Author = await BDWorks.GetAuthorById(Convert.ToInt32(row["Authorsid"].ToString())),
                            Count = Convert.ToInt32(row["Count"].ToString())
                        };

                        result.Add(product);

                    }
                }
                await connection.CloseAsync();
            }

            return result;
        }

        public static async void EditProduct(ProductVM product)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "update_product";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("product_id", OracleDbType.Int32).Value = product.Id;
                command.Parameters.Add("product_name", OracleDbType.Varchar2).Value = product.Name;
                command.Parameters.Add("product_collection", OracleDbType.Varchar2).Value = product.Collection;
                command.Parameters.Add("product_description", OracleDbType.Varchar2).Value = product.Description;
                command.Parameters.Add("product_image", OracleDbType.Varchar2).Value = product.Image;
                command.Parameters.Add(" product_price", OracleDbType.Double).Value = product.Price;
                command.Parameters.Add("author_id", OracleDbType.Int32).Value = product.AuthorID;
                command.Parameters.Add("product_count", OracleDbType.Int32).Value = product.Count;



                await command.ExecuteNonQueryAsync();


                await connection.CloseAsync();
            }

        }

        public static async void InsertProduct(ProductVM product)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";
         
            string commandText = "insert_product";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("product_name", OracleDbType.Varchar2).Value = product.Name;
                command.Parameters.Add("product_collection", OracleDbType.Varchar2).Value = product.Collection;
                command.Parameters.Add("product_description", OracleDbType.Varchar2).Value = product.Description;
                command.Parameters.Add("product_image", OracleDbType.Varchar2).Value = product.Image;
                command.Parameters.Add("product_price", OracleDbType.Double).Value = product.Price;
                command.Parameters.Add("author_id", OracleDbType.Int32).Value = product.AuthorID;
                command.Parameters.Add("product_count", OracleDbType.Int32).Value = product.Count;



                await command.ExecuteNonQueryAsync();


                await connection.CloseAsync();
            }

        }
        public static async void DeleteProductById(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "delete_product";

            using (OracleConnection connection = new(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("product_id", OracleDbType.Int32).Value = id;

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

        }

        public static async void InsertAuthor(Author author)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "insert_author";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("author_name", OracleDbType.Varchar2).Value = author.Name;
                command.Parameters.Add("author_country", OracleDbType.Varchar2).Value = author.Country;
                command.Parameters.Add("author_image", OracleDbType.Varchar2).Value = author.Image;
                command.Parameters.Add("author_biography", OracleDbType.Varchar2).Value = author.Biography;


                await command.ExecuteNonQueryAsync();


                await connection.CloseAsync();
            }

        }

        public static async void EditAuthor(Author author)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "update_author";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("author_id", OracleDbType.Int32).Value = author.Id;
                command.Parameters.Add("author_name", OracleDbType.Varchar2).Value = author.Name;
                command.Parameters.Add("author_country", OracleDbType.Varchar2).Value = author.Country;
                command.Parameters.Add("author_image", OracleDbType.Varchar2).Value = author.Image;
                command.Parameters.Add("author_biography", OracleDbType.Varchar2).Value = author.Biography;


                await command.ExecuteNonQueryAsync();


                await connection.CloseAsync();
            }

        }

        public static async void DeleteAuthorById(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "delete_author";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("author_id", OracleDbType.Int32).Value = id;

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

        }

        public static async Task<bool> CheckProductDependencies(int authors_id)
        {
            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "CheckProductDependencies";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;


                OracleParameter retval = new OracleParameter("resultItem", OracleDbType.Boolean);
                retval.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(retval);

                OracleParameter inval = new OracleParameter("authors_id", OracleDbType.Int32);
                inval.Direction = ParameterDirection.Input;
                inval.Value = authors_id;
                command.Parameters.Add(inval);


                await command.ExecuteNonQueryAsync();



                await connection.CloseAsync();

                OracleBoolean oracleDecimal = (OracleBoolean)retval.Value;
                return (bool)oracleDecimal.Value;
            }
        }
        public static async void RegisterUser(User user)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "register_user";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("u_firstname", OracleDbType.Varchar2).Value = user.FistName;
                command.Parameters.Add("u_lastname", OracleDbType.Varchar2).Value = user.LastName;
                command.Parameters.Add("u_email", OracleDbType.Varchar2).Value = user.Email;
                command.Parameters.Add("u_password", OracleDbType.Varchar2).Value = user.Password;


                await command.ExecuteNonQueryAsync();


                await connection.CloseAsync();
            }
            await BDWorks.LogInUser(user);
        }

        public static async Task<bool> CheckUserExist(string email)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "check_user_exist";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;


                OracleParameter retval = new OracleParameter("myoutvar", OracleDbType.Boolean);
                retval.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(retval);

                OracleParameter inval = new OracleParameter("u_email", OracleDbType.Varchar2);
                inval.Direction = ParameterDirection.Input;
                inval.Value = email;
                command.Parameters.Add(inval);


                await command.ExecuteNonQueryAsync();

          

                await connection.CloseAsync();
                OracleBoolean res = (OracleBoolean)retval.Value;

                return res.Value ;
            }

        }

        public static async Task<bool> LogInUser(User user)
        {

            if(user.Email == "admin@gmail.com" && user.Password == "Admin03@")
            {
                BDWorks.IsAdmin = true;
                BDWorks.CurrentUser = null;
                return true;
            }
            BDWorks.IsAdmin = false;

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "log_in";


            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;


                OracleParameter email = new OracleParameter("user_email", OracleDbType.Varchar2);
                email.Direction = System.Data.ParameterDirection.Input;
                email.Value = user.Email;
                command.Parameters.Add(email);

                OracleParameter password = new OracleParameter("user_password", OracleDbType.Varchar2);
                password.Direction = System.Data.ParameterDirection.Input;
                password.Value = user.Password;
                command.Parameters.Add(password);


                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                User u = new User();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        u = new User {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            FistName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Email = row["Email"].ToString(),
                            Password = row["Password"].ToString()

                        };
                    }

                    BDWorks.CurrentUser = u;
                    await connection.CloseAsync();
                    Debug.WriteLine(CurrentUser.Email);
                    return true;

                }
                else
                {
                    await connection.CloseAsync();

                    return false;
                }


                /*                if (BDWorks.CurrentUser.Email != null)
                                {
                                    await EmailService.SendEmailAsync(BDWorks.CurrentUser.Email, "qwe", "zxc");
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }*/
            }

        }
        public static async Task<List<Basket>> GetBasketItems()
        {
            List<Basket> result = new List<Basket>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetBasketItems";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                OracleParameter userID = new OracleParameter("user_id ", OracleDbType.Varchar2);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = BDWorks.CurrentUser.Id;
                command.Parameters.Add(userID);

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Basket basket = new Basket();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        basket = new Basket
                        {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            Count = Convert.ToInt32(row["Count"].ToString()),
                            UserId = Convert.ToInt32(row["UserId"].ToString()),
                            Product = await BDWorks.GetProductById(Convert.ToInt32(row["ProductId"].ToString()))

                        };

                        result.Add(basket);
                    }
                }
                await connection.CloseAsync();
            }

            return result;
        }
        public static async void IncrementCount(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "IncrementCount";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("user_id", OracleDbType.Int32).Value = BDWorks.CurrentUser.Id;
                command.Parameters.Add("product_id", OracleDbType.Int32).Value = id;


                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

        }
        public static async void DecrementCount(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "DecrementCount";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("user_id", OracleDbType.Int32).Value = BDWorks.CurrentUser.Id;
                command.Parameters.Add("product_id", OracleDbType.Int32).Value = id;


                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

        }
        public static async void AddProductToBasket(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "AddToBasket";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("user_id", OracleDbType.Int32).Value = BDWorks.CurrentUser.Id;
                command.Parameters.Add("product_id", OracleDbType.Int32).Value = id;


                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

        }
        public static async Task<bool> MakeOrder(Order order)
        {
            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "AddOrder";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                    command.Parameters.Add("p_Count", OracleDbType.Int32, ParameterDirection.Input).Value = order.Count;
                    command.Parameters.Add("p_Country", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.Country;
                    command.Parameters.Add("p_FirstName", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.FirstName;
                    command.Parameters.Add("p_LastName", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.LastName;
                    command.Parameters.Add("p_Address", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.Address;
                    command.Parameters.Add("p_PostalCode", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.PostalCode;
                    command.Parameters.Add("p_City", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.City;
                    command.Parameters.Add("p_OrderDate", OracleDbType.Date, ParameterDirection.Input).Value = order.OrderDate;
                    command.Parameters.Add("p_Status", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.Status;
                    command.Parameters.Add("p_DeliveryMethod", OracleDbType.Varchar2, ParameterDirection.Input).Value = order.DeliveryMethod;
                    command.Parameters.Add("p_UserId", OracleDbType.Int32, ParameterDirection.Input).Value = order.UserId;
                    command.Parameters.Add("p_ProductId", OracleDbType.Int32, ParameterDirection.Input).Value = order.Product.Id;

                    OracleParameter retval = new OracleParameter("myoutvar", OracleDbType.Boolean);
                    retval.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(retval);



                await command.ExecuteNonQueryAsync();
           
                await connection.CloseAsync();
                OracleBoolean res = (OracleBoolean)retval.Value;

                return res.Value;
            }

            
        }

        public static async void ClearBasket(User u)
        {
            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "clearBasket";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("user_id", OracleDbType.Int32).Value = u.Id;

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }
        public static async Task<List<Order>> GetUserOrders(int id, string status)
        {
            List<Order> result = new List<Order>();
            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetUserOrders";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;


                OracleParameter userID = new OracleParameter("user_id ", OracleDbType.Int32);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = id;
                command.Parameters.Add(userID);

                OracleParameter delStatus = new OracleParameter("delivery_status", OracleDbType.Varchar2);
                delStatus.Direction = System.Data.ParameterDirection.Input;
                delStatus.Value = status;
                command.Parameters.Add(delStatus);


                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Order o = new Order();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        o = new Order
                        {

                            Id = Convert.ToInt32(row["id"].ToString()),
                            Count = Convert.ToInt32(row["count"].ToString()),
                            Country = row["Country"].ToString(),
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Address = row["Address"].ToString(),
                            PostalCode = row["PostalCode"].ToString(),
                            City = row["City"].ToString(),
                            OrderDate = (DateTime)row["OrderDate"],
                            Status = row["Status"].ToString(),
                            DeliveryMethod = row["DeliveryMethod"].ToString(),
                            UserId = Convert.ToInt32(row["UserId"].ToString()),
                            Product = await BDWorks.GetProductById(Convert.ToInt32(row["ProductId"].ToString())),
                        };
                        result.Add(o);

                    }
                }
              

                await connection.CloseAsync();

               

            }

            return result;

        }
        public static async Task<List<int>> DistinctUsersIdFromOrders( )
        {
            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            //    string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";
            List<int> users = new List<int>();



            string commandText = "select *  from distinctUsersIdFromOrders";

            using (OracleConnection connection = new(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        users.Add(reader.GetInt32(0));

                    }
                }
                await connection.CloseAsync();
            }
            return users;
        }
        public static async void EditOrder(Order order)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "update_order";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("order_id", OracleDbType.Int32).Value = order.Id;
                command.Parameters.Add("order_status", OracleDbType.Varchar2).Value = order.Status;
      


                await command.ExecuteNonQueryAsync();


                await connection.CloseAsync();
            }

        }
        public static async Task<List<Order>> GetOrdersBetweenDate(int id,string status, DateTime? minDate, DateTime? maxDate)
        {
            List<Order> result = new List<Order>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetUserOrdersBetweenDays";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                OracleParameter userID = new OracleParameter("user_id ", OracleDbType.Int32);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = id;
                command.Parameters.Add(userID);

                OracleParameter delStatus = new OracleParameter("delivery_status", OracleDbType.Varchar2);
                delStatus.Direction = System.Data.ParameterDirection.Input;
                delStatus.Value = status;
                command.Parameters.Add(delStatus);

                OracleParameter minOrderDate = new OracleParameter("minDate", OracleDbType.Date);
                minOrderDate.Direction = System.Data.ParameterDirection.Input;
                minOrderDate.Value = minDate;
                command.Parameters.Add(minOrderDate);

                OracleParameter maxOrderDate = new OracleParameter("maxDate", OracleDbType.Date);
                maxOrderDate.Direction = System.Data.ParameterDirection.Input;
                maxOrderDate.Value = maxDate;
                command.Parameters.Add(maxOrderDate);

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Order o = new Order();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        o = new Order
                        {

                            Id = Convert.ToInt32(row["id"].ToString()),
                            Count = Convert.ToInt32(row["count"].ToString()),
                            Country = row["Country"].ToString(),
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Address = row["Address"].ToString(),
                            PostalCode = row["PostalCode"].ToString(),
                            City = row["City"].ToString(),
                            OrderDate = (DateTime)row["OrderDate"],
                            Status = row["Status"].ToString(),
                            DeliveryMethod = row["DeliveryMethod"].ToString(),
                            UserId = Convert.ToInt32(row["UserId"].ToString()),
                            Product = await BDWorks.GetProductById(Convert.ToInt32(row["ProductId"].ToString())),
                        };
                        result.Add(o);

                    }
                }
                await connection.CloseAsync();
            }
            Debug.WriteLine(result.Count.ToString());
            return result;
        }

        public static async void InsertReservedProduct(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "insert_reservedProduct";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("user_id", OracleDbType.Int32).Value = BDWorks.CurrentUser.Id;
                command.Parameters.Add("product_id", OracleDbType.Int32).Value = id;
                command.Parameters.Add("ExpirationDate", OracleDbType.Date).Value = DateTime.Now.AddDays(3);




                await command.ExecuteNonQueryAsync();


                await connection.CloseAsync();
            }

        }

        public static async void DeleteReservedProductById(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "delete_reservedProduct";

            using (OracleConnection connection = new(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("reservedProduct_id", OracleDbType.Int32).Value = id;

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

        }

        public static async Task<bool> CheckProductIdExistence(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "CheckProductIdExistence";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;


                OracleParameter retval = new OracleParameter("myoutvar", OracleDbType.Boolean);
                retval.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(retval);

                OracleParameter inval = new OracleParameter("p_ProductId", OracleDbType.Int32);
                inval.Direction = ParameterDirection.Input;
                inval.Value = id;
                command.Parameters.Add(inval);


                await command.ExecuteNonQueryAsync();



                await connection.CloseAsync();
                OracleBoolean res = (OracleBoolean)retval.Value;

                return res.Value;
            }

        }

        public static async Task<List<ReservedProduct>> GetUsersReservedItems()
        {
            List<ReservedProduct> result = new List<ReservedProduct>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GetUsersReservedItems";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                OracleParameter userID = new OracleParameter("user_id ", OracleDbType.Varchar2);
                userID.Direction = System.Data.ParameterDirection.Input;
                userID.Value = BDWorks.CurrentUser.Id;
                command.Parameters.Add(userID);

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ReservedProduct reserved = new ();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        reserved = new ReservedProduct
                        {
                            Id = Convert.ToInt32(row["id"].ToString()),
                            UserId = BDWorks.CurrentUser.Id,
                            Product = await BDWorks.GetProductById(Convert.ToInt32(row["ProductId"].ToString())),
                            ExpirationDate = (DateTime)row["ExpirationDate"]


                        };

                        result.Add(reserved);
                    }
                }
                await connection.CloseAsync();
            }

            return result;
        }

        public static async void CheckExpiratedReservations()
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "checkExpiratedReservations";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }

        }

        public static async Task<bool> СheckReservedItems(int userId,int productId)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "check_reserved_items";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;


                OracleParameter retval = new OracleParameter("myoutvar", OracleDbType.Boolean);
                retval.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(retval);

                OracleParameter inval = new OracleParameter("user_id", OracleDbType.Int32);
                inval.Direction = ParameterDirection.Input;
                inval.Value = userId;
                command.Parameters.Add(inval);

                OracleParameter inval2 = new OracleParameter("product_id", OracleDbType.Int32);
                inval2.Direction = ParameterDirection.Input;
                inval2.Value = productId;
                command.Parameters.Add(inval2);




                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                OracleBoolean res = (OracleBoolean)retval.Value;

                return res.Value;
            }

        }

        public static async void DeleteBasketById(int id)
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "delete_basket";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add("basket_id", OracleDbType.Int32).Value = id;

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

        }


        //Statistics

        public static async Task<List<Statistics>> GetStatistics()
        {
            var statistics = new List<Statistics>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GAllStatistics";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        statistics.Add(new Statistics
                        {
                            NowProductCount = Convert.ToInt32(row["NowProductCount"].ToString()),
                            NowProductPrice = (float)Convert.ToDouble(row["NowProductPrice"].ToString()),
                            OrderCount = Convert.ToInt32(row["OrderCount"].ToString()),
                            OrderPrice = (float)Convert.ToDouble(row["OrderPrice"].ToString()),
                            AOrderCount = Convert.ToInt32(row["AOrderCount"].ToString()),
                            AOrderPrice = (float)Convert.ToDouble(row["AOrderPrice"].ToString()),
                            DOrderCount = Convert.ToInt32(row["DOrderCount"].ToString()),
                            DOrderPrice = (float)Convert.ToDouble(row["DOrderPrice"].ToString()),
                            COrderCount = Convert.ToInt32(row["COrderCount"].ToString()),
                            COrderPrice = (float)Convert.ToDouble(row["COrderPrice"].ToString()),
                            ReservedCount = Convert.ToInt32(row["ReservedCount"].ToString()),
                            RegisteredUserCount = Convert.ToInt32(row["RegisteredUserCount"].ToString()),
                        });
                    }
                }
                await connection.CloseAsync();
            }

            return statistics;


        }
        public static async Task<Dictionary<Product, int>> GetTenTopProducts()
        {
            //     OracleConnection.ClearAllPools();

            var top = new Dictionary<Product,int>();

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "GTopTenProd";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);

                command.InitialLONGFetchSize = 1000;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.BindByName = true;

                command.Parameters.Add("resultItems", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                OracleDataAdapter da = new OracleDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        top.Add(await BDWorks.GetProductById(Convert.ToInt32(row["ProductId"].ToString())), Convert.ToInt32(row["TOTALSOLD"].ToString()));
                    }
                }
                await connection.CloseAsync();
            }

            return top;
        }


        public static async void UpdateStatistics()
        {

            string ConnectionString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-UBC0KGK)(PORT = 1521))    (CONNECT_DATA = (SERVICE_NAME = PDB_Cours))  );User Id=admin;Password=123;";

            string commandText = "update_statistics";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                OracleCommand command = new OracleCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }

        }


    }
}
