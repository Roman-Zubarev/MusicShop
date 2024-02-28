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
using MongoDB.Driver;
using MongoDB.Bson;
using static Raven.Abstractions.Data.Constants;
using System.Xml.Linq;
using System;
using Raven.Abstractions.Data;

namespace MusicShop.Models
{
    public class BDWorks
    {
        public static User CurrentUser = null;
        public static bool IsAdmin = false;
        public static DateTime MinDate = DateTime.MinValue;
        public static DateTime MaxDate = DateTime.Now;
   
        public static async Task<List<Author>> GetAllAuthors() //good
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<Author>("Authors");
            using var cursor = await collection.FindAsync(new BsonDocument());

            List<Author> authors = cursor.ToList();

            return authors;

        }

        public static async Task<Author> GetAuthorById(ObjectId id) //good
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<Author>("Authors");

            var author = await collection.Find(new BsonDocument("_id", id)).FirstAsync();

            return author;
        }

        public static async void InsertAuthor(Author author) //good
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<Author>("Authors");

            await collection.InsertOneAsync(author);


        }

        public static async void EditAuthor(Author author)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<Author>("Authors");

            var result = await collection.FindOneAndReplaceAsync(p => p.Id == author.Id, author);
        } //good

        public static async void DeleteAuthorById(ObjectId id)
        {
            if (await BDWorks.CheckProductDependencies(id))
            {
                MongoClient client = new MongoClient("mongodb://localhost:27017");

                IMongoDatabase db = client.GetDatabase("test");
                var collection = db.GetCollection<Author>("Authors");

                var result = await collection.FindOneAndDeleteAsync(p => p.Id == id);
            }
          
        } //good

        public static async Task<List<Product>> GetAllProducts()
        {

            List<Product> products = new();


            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");

            using var cursor = await collection.FindAsync(new BsonDocument());

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                products.Add(new()
                {
                    Id = (ObjectId)item.GetValue("_id"),
                    Name = (string)item.GetValue("Name"),
                    Collection = (string)item.GetValue("Collection"),
                    Description = (string)item.GetValue("Description"),
                    Image = (string)item.GetValue("Image"),
                    Price = (double)item.GetValue("Price"),
                    Author = await BDWorks.GetAuthorById((ObjectId)item.GetValue("AuthorID")),
                    Count = (int)item.GetValue("Count")

                });
            }


            return products;

        } //good

        public static async Task<List<string>> GetDistinctCollections()
        {
            List<string> distinctCollections = new();


            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");

            using var cursor = await collection.FindAsync(new BsonDocument());

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                distinctCollections.Add((string)item.GetValue("Collection"));


            }

            
            return distinctCollections.Distinct().ToList();
        } //good

        public static async void InsertProduct(ProductVM product)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");


            BsonDocument prod = new BsonDocument
            {
                { "Name", product.Name},
                { "Collection",  product.Collection},
                { "Description",  product.Description},
                { "Image",  product.Image},
                { "Price",  product.Price},
                { "AuthorID", product.AuthorID},
                { "Count", product.Count},

            };
            await collection.InsertOneAsync(prod);

        } //good

        public static async Task<Product> GetProductById(ObjectId id)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");

            var cursor = await collection.Find(new BsonDocument("_id", id)).FirstAsync();

            Product product = new Product()
            {
                Id = (ObjectId)cursor.GetValue("_id"),
                Name = (string)cursor.GetValue("Name"),
                Collection = (string)cursor.GetValue("Collection"),
                Description = (string)cursor.GetValue("Description"),
                Image = (string)cursor.GetValue("Image"),
                Price = (double)cursor.GetValue("Price"),
                Author = await BDWorks.GetAuthorById((ObjectId)cursor.GetValue("AuthorID")),
                Count = (int)cursor.GetValue("Count")
            };

            return product;
        } //good

        public static async void DeleteProductById(ObjectId id)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");

             await collection.FindOneAndDeleteAsync(new BsonDocument("_id",id));
        } //good

        public static async void EditProduct(ProductVM product)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");


            BsonDocument prod = new BsonDocument
            {
                { "Name", product.Name},
                { "Collection",  product.Collection},
                { "Description",  product.Description},
                { "Image",  product.Image},
                { "Price",  product.Price},
                { "AuthorID", product.AuthorID},
                { "Count", product.Count},

            };

            await collection.FindOneAndReplaceAsync(new BsonDocument("_id", product.Id),prod);

        } //good

        public static async Task<List<Product>> GetProductsWithCollection(string collection)
        {

            List<Product> result = await BDWorks.GetAllProducts();

            return result.Where(p => p.Collection == collection).ToList();
        } //good

        public static async Task<List<Product>> GetNRandomProducts(int rand_count)
        {

            List<Product> result = await BDWorks.GetAllProducts();
            var random = new Random();



            var randomElements = result.OrderBy(x => random.Next()).Take(rand_count).ToList();
            return randomElements;
        } // good 

        public static async Task<bool> CheckProductDependencies(ObjectId authors_id)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");

            var cursor =  collection.Find(new BsonDocument("AuthorID", authors_id)).Count();

            Debug.WriteLine(cursor);
            return cursor == 0 ? true : false;

        } // good

        public static async void RegisterUser(User user)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<User>("Users");

            await collection.InsertOneAsync(user);

            await BDWorks.LogInUser(user);
        } // good

        public static async Task<bool> CheckUserExist(string email)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<User>("Users");

            var userCount = collection.Find(new BsonDocument("Email", email)).Count();

            return userCount == 0;

        } //good

        public static async Task<bool> LogInUser(User user)
        {

            if (user.Email == "admin@gmail.com" && user.Password == "Admin03@")
            {
                BDWorks.IsAdmin = true;
                BDWorks.CurrentUser = null;
                return true;
            }
            BDWorks.IsAdmin = false;


            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<User>("Users");

            BsonDocument userIn = new BsonDocument
            {
                {"Email", user.Email},
                {"Password", user.Password}
            };

            var newUser = collection.Find(userIn).FirstOrDefaultAsync();



            BDWorks.CurrentUser = await newUser;

            if (BDWorks.CurrentUser.Email != null)
            {
              //  await EmailService.SendEmailAsync(BDWorks.CurrentUser.Email, "qwe", "zxc");
                return true;
            }
            else
            {
                return false;
            }

            
        } //good


        public static async Task<bool> CheckProductIdExistence(ObjectId id)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");

            BsonDocument ProdIn = new BsonDocument
            {
                {"ProductId", id}
            };
            var collection = db.GetCollection<BsonDocument>("Basket");

            var basketItem = await collection.Find(ProdIn).FirstOrDefaultAsync();


            if (basketItem != null)
            {
                return true;
            }

            var collection2 = db.GetCollection<BsonDocument>("Orders");

            var basketItem2 = await collection2.Find(ProdIn).FirstOrDefaultAsync();
            

            if (basketItem2 != null)
            {
                return true;
            }

            var collection3 = db.GetCollection<BsonDocument>("ReservedProducts");

            var basketItem3 = await collection3.Find(ProdIn).FirstOrDefaultAsync();


            if (basketItem3 != null)
            {
                return true;
            }

            return false;


        } //good
        public static async Task<List<Basket>> GetBasketItems()
        {
            List<Basket> result = new List<Basket>();

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Basket");

            using var cursor = await collection.FindAsync(new BsonDocument("UserId", BDWorks.CurrentUser.Id));

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                result.Add(new()
                {
                    Id = (ObjectId)item.GetValue("_id"),
                    UserId = CurrentUser.Id,
                    Product = await BDWorks.GetProductById((ObjectId)item.GetValue("ProductId")),
                    Count = (int)item.GetValue("Count")

                });
            }

            return result;
        } //good

        public static async Task<bool> AddProductToBasket(ObjectId id)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Basket");

            BsonDocument userIn = new BsonDocument
            {
                {"UserId", CurrentUser.Id},
                {"ProductId", id}
            };

            var basketItem = await collection.Find(userIn).FirstOrDefaultAsync();


            if (basketItem != null)
            {
                BsonDocument newBasketItem = new BsonDocument
                {
                    {"UserId", CurrentUser.Id},
                    {"ProductId", id},
                    {"Count", (int)basketItem.GetValue("Count") + 1},
                };

                await collection.FindOneAndReplaceAsync(userIn, newBasketItem);

            }
            else
            {
                BsonDocument prod = new BsonDocument
                {
                    {"UserId", CurrentUser.Id},
                    {"Count", 1},
                    {"ProductId", id}

                };
                await collection.InsertOneAsync(prod);
            }
            return true;

        }  //good

        public static async Task<bool> IncrementCount(ObjectId id) //good
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Basket");


            BsonDocument userIn = new BsonDocument
            {
                {"UserId", CurrentUser.Id},
                {"ProductId", id}
            };


            var basketItem = await collection.Find(userIn).FirstOrDefaultAsync();
            var prod = await BDWorks.GetProductById(id);
            if (basketItem != null)
            {
                var a = (int)basketItem.GetValue("Count");

                if (prod.Count <= a + 1)
                {
                    await collection.FindOneAndUpdateAsync(new BsonDocument("ProductId", id), new BsonDocument("$set", new BsonDocument("Count", prod.Count)));

                }
                else {
                    await collection.FindOneAndUpdateAsync(new BsonDocument("ProductId", id), new BsonDocument("$inc", new BsonDocument("Count", 1)));

                }
            }
            else
            {
                return false;
            }


            return true;



        }

        public static async Task<bool> DecrementCount(ObjectId id)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Basket");


            BsonDocument userIn = new BsonDocument
            {
                {"UserId", CurrentUser.Id},
                {"ProductId", id}
            };


            var basketItem = await collection.Find(userIn).FirstOrDefaultAsync();
            var prod = await BDWorks.GetProductById(id);
            if (basketItem != null)
            {
                var a = (int)basketItem.GetValue("Count");

                if (prod.Count <= a - 1)
                {
                    await collection.FindOneAndUpdateAsync(new BsonDocument("ProductId", id), new BsonDocument("$set", new BsonDocument("Count", prod.Count)));

                }
                else if (a - 1 == 0)
                {
                    await collection.FindOneAndDeleteAsync(userIn);
                }
                else
                {
                    await collection.FindOneAndUpdateAsync(new BsonDocument("ProductId", id), new BsonDocument("$set", new BsonDocument("Count", a - 1)));
                }
            }
            else return false;
            return true;

        } //good 

        public static async Task<bool> MakeOrder(Order order)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Orders");


            var prod = await BDWorks.GetProductById(order.Product.Id);
            if (prod != null)
            {
                
                if (prod.Count >= order.Count)
                {
                    BsonDocument ord = new BsonDocument
                    {
                        { "Count", order.Count},
                        { "Country", order.Country},
                        { "FirstName",  order.FirstName},
                        { "LastName",  order.LastName},
                        { "Address",  order.Address},
                        { "PostalCode",  order.PostalCode},
                        { "City", order.City},
                        { "OrderDate", order.OrderDate},
                        { "Status",  order.Status},
                        { "DeliveryMethod", order.DeliveryMethod},
                        { "UserId",  order.UserId},
                        { "ProductId", order.Product.Id},

                    };

                    await collection.InsertOneAsync(ord);
                    BDWorks.ChangeProductCount(order.Product.Id, prod.Count - order.Count);
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }  //good 

        public static async void DeleteBasketById(ObjectId id)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Basket");

            await collection.FindOneAndDeleteAsync(new BsonDocument("_id", id));

        } //good 

        public static async void ChangeProductCount(ObjectId id, int count)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Products");



            await collection.FindOneAndUpdateAsync(new BsonDocument("_id", id), new BsonDocument("$set", new BsonDocument("Count", count)));


        } //good 

        public static async Task<List<Order>> GetUserOrders(ObjectId id, string status)
        {
            List<Order> orders = new List<Order>();

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Orders");


            BsonDocument userIn = new BsonDocument
            {
                {"UserId", id},
                {"Status", status}
            };

            using var cursor = await collection.FindAsync(userIn);

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                orders.Add(new()
                {
                    Id = (ObjectId)item.GetValue("_id"),
                    Count = (int)item.GetValue("Count"),
                    Country = (string)item.GetValue("Country"),
                    FirstName = (string)item.GetValue("FirstName"),
                    LastName = (string)item.GetValue("LastName"),
                    Address = (string)item.GetValue("Address"),
                    PostalCode = (string)item.GetValue("PostalCode"),
                    City = (string)item.GetValue("City"),
                    OrderDate = (DateTime)item.GetValue("OrderDate"),
                    Status = (string)item.GetValue("Status"),
                    DeliveryMethod = (string)item.GetValue("DeliveryMethod"),
                    UserId = (ObjectId)item.GetValue("UserId"),
                    Product =  await BDWorks.GetProductById( (ObjectId)item.GetValue("ProductId")),
                });
            }


            return orders;
        } //good

        public static async Task<List<ObjectId>> DistinctUsersIdFromOrders( )
        {
            List<ObjectId> distinctId = new();


            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Orders");

            using var cursor = await collection.FindAsync(new BsonDocument());

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                distinctId.Add((ObjectId)item.GetValue("UserId"));


            }


            return distinctId.Distinct().ToList();
        } //good 

        public static async Task<List<Order>> GetOrdersBetweenDate(ObjectId id,string status, DateTime? minDate, DateTime? maxDate)
        {
            List<Order> orders = new List<Order>();

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Orders");


            BsonDocument userIn = new BsonDocument
            {
                {"UserId", id},
                {"Status", status}
            };

            using var cursor = await collection.FindAsync(userIn);

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                orders.Add(new()
                {
                    Id = (ObjectId)item.GetValue("_id"),
                    Count = (int)item.GetValue("Count"),
                    Country = (string)item.GetValue("Country"),
                    FirstName = (string)item.GetValue("FirstName"),
                    LastName = (string)item.GetValue("LastName"),
                    Address = (string)item.GetValue("Address"),
                    PostalCode = (string)item.GetValue("PostalCode"),
                    City = (string)item.GetValue("City"),
                    OrderDate = (DateTime)item.GetValue("OrderDate"),
                    Status = (string)item.GetValue("Status"),
                    DeliveryMethod = (string)item.GetValue("DeliveryMethod"),
                    UserId = (ObjectId)item.GetValue("UserId"),
                    Product = await BDWorks.GetProductById((ObjectId)item.GetValue("ProductId")),
                });
            }


            return orders.Where(o => o.OrderDate >= minDate && o.OrderDate <= maxDate).ToList();
        } //good


        public static async Task<List<Order>> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Orders");


            using var cursor = await collection.FindAsync(new BsonDocument());

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                orders.Add(new()
                {
                    Id = (ObjectId)item.GetValue("_id"),
                    Count = (int)item.GetValue("Count"),
                    Country = (string)item.GetValue("Country"),
                    FirstName = (string)item.GetValue("FirstName"),
                    LastName = (string)item.GetValue("LastName"),
                    Address = (string)item.GetValue("Address"),
                    PostalCode = (string)item.GetValue("PostalCode"),
                    City = (string)item.GetValue("City"),
                    OrderDate = (DateTime)item.GetValue("OrderDate"),
                    Status = (string)item.GetValue("Status"),
                    DeliveryMethod = (string)item.GetValue("DeliveryMethod"),
                    UserId = (ObjectId)item.GetValue("UserId"),
                    Product = await BDWorks.GetProductById((ObjectId)item.GetValue("ProductId")),
                });
            }


            return orders;
        } //good

        public static async void EditOrder(Order order)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Orders");

            await collection.FindOneAndUpdateAsync(new BsonDocument("_id", order.Id), new BsonDocument("$set", new BsonDocument("Status", order.Status)));

        } //good

        public static async Task<bool> InsertReservedProduct(ObjectId id)
        {
            //    command.Parameters.Add("ExpirationDate", OracleDbType.Date).Value = DateTime.Now.AddDays(3);
            var prod = await BDWorks.GetProductById(id); 
            BDWorks.ChangeProductCount(id, prod.Count - 1);


            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("ReservedProducts");

            BsonDocument resProd = new BsonDocument
            {
                { "UserId", BDWorks.CurrentUser.Id},
                { "ProductId", id},
                { "ExpirationDate",  DateTime.Now.AddDays(3)},

            };
            await collection.InsertOneAsync(resProd);
            return true;
        } // good

        public static async Task<List<ReservedProduct>> GetUsersReservedItems()
        {
            List<ReservedProduct> result = new List<ReservedProduct>();

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("ReservedProducts");

            using var cursor = await collection.FindAsync(new BsonDocument("UserId", BDWorks.CurrentUser.Id));

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                result.Add(new()
                {
                    Id = (ObjectId)item.GetValue("_id"),
                    UserId = CurrentUser.Id,
                    Product = await BDWorks.GetProductById((ObjectId)item.GetValue("ProductId")),
                    ExpirationDate = (DateTime)item.GetValue("ExpirationDate")

                });
            }

            return result;
        } // good


        public static async Task<bool> СheckReservedItems(ObjectId userId, ObjectId productId)
        {


            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("ReservedProducts");

            BsonDocument userIn = new BsonDocument
            {
                {"UserId", userId},
                {"ProductId", productId}
            };

            var prod = await collection.Find(userIn).FirstOrDefaultAsync();

            if (prod == null) return true;
            return false;


        } //good


        public static async void DeleteReservedProductById(ObjectId id)
        {

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("ReservedProducts");



            var prod = await collection.Find(new BsonDocument("_id",id)).FirstOrDefaultAsync();

            var product = await BDWorks.GetProductById((ObjectId)prod.GetValue("ProductId"));
            if (product == null) return;
            BDWorks.ChangeProductCount(product.Id, product.Count + 1);

            await collection.FindOneAndDeleteAsync(new BsonDocument("_id", id));

        } //good 

  
        public static async Task<List<ReservedProduct>> GetAllReservedProducts()
        {
            // OracleConnection.ClearAllPools();

            List<ReservedProduct> result = new List<ReservedProduct>();

            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("ReservedProducts");

            using var cursor = await collection.FindAsync(new BsonDocument());

            var prod = cursor.ToList();

            foreach (var item in prod)
            {
                result.Add(new()
                {
                    Id = (ObjectId)item.GetValue("_id"),
                    UserId = (ObjectId)item.GetValue("UserId"),
                    Product = await BDWorks.GetProductById((ObjectId)item.GetValue("ProductId")),
                    ExpirationDate = (DateTime)item.GetValue("ExpirationDate")

                });
            }

            return result;



        } // test

        public static async Task<bool> CheckExpiratedReservations() //good
        {

            var resProducts = await BDWorks.GetAllReservedProducts();

            foreach (var item in resProducts)
            {
                if (item.ExpirationDate < DateTime.Now)
                {
                    BDWorks.DeleteReservedProductById(item.Id);
                    BDWorks.ChangeProductCount(item.Product.Id,  item.Product.Count + 1);

                }
            }
            return true;
        } // good


        public static async Task<List<Statistics>> GetStatistics()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<Statistics>("Statistics");
            using var cursor = await collection.FindAsync(new BsonDocument());

      

            return cursor.ToList();
        } //good

        public static async void UpdateStatistics()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            IMongoDatabase db = client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("Users");

            using var cursor = await collection.FindAsync(new BsonDocument());


            Statistics stat = new();
            var orderList = await BDWorks.GetAllOrders();
            var productList = await BDWorks.GetAllProducts();
            var reserveList = await BDWorks.GetAllReservedProducts();

            stat.NowProductCount = productList.Count;
            stat.NowProductPrice = (float)productList.Sum(p => p.Price);
            stat.OrderCount =  orderList.Count;
            Debug.WriteLine(stat.OrderCount);
            stat.OrderPrice = (float)orderList.Sum(a => a.Product.Price * a.Count);
            stat.AOrderCount = orderList.Where(p => p.Status == "Accepted").ToList().Count;
            stat.AOrderPrice = (float)orderList.Where(p => p.Status == "Accepted").Sum(a => a.Product.Price * a.Count);
            stat.DOrderCount = orderList.Where(p => p.Status == "Delivered").ToList().Count;
            stat.DOrderPrice = (float)orderList.Where(p => p.Status == "Delivered").Sum(a => a.Product.Price * a.Count);
            stat.COrderCount = orderList.Where(p => p.Status == "Canceled").ToList().Count;
            stat.COrderPrice = (float)orderList.Where(p => p.Status == "Canceled").Sum(a => a.Product.Price * a.Count);
            stat.ReservedCount = reserveList.Count;
            stat.RegisteredUserCount = cursor.ToList().Count;

            var collection2 = db.GetCollection<Statistics>("Statistics");

           // await collection2.InsertOneAsync(stat);


            
            var oldstat = await collection2.Find(new BsonDocument()).FirstAsync();
            stat.Id = oldstat.Id;
            Debug.WriteLine(oldstat.Id);
            Debug.WriteLine(oldstat.OrderCount);

             var result = await collection2.FindOneAndReplaceAsync(p => p.Id == oldstat.Id, stat);


        } //good

        public static async Task<Dictionary<Product, int>> GetTenTopProducts()
        {

            Dictionary<Product, int> top = new();

            var orders = await BDWorks.GetAllOrders();


            foreach (var item in await BDWorks.GetAllProducts())
            {

                top.Add(item, orders.Where(a => a.Product.Id == item.Id).Sum(a => a.Count));
            }




            return top.OrderByDescending(p => p.Value).Take(10).ToDictionary(k => k.Key, k => k.Value);
        }

    }
}
