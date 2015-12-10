namespace JsonFileToDB.Storage
{
    using JsonFileToDB.Models;
    using Pearson.ContentCompression.Business;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class Model : DbContext
    {
        // Your context has been configured to use a 'Model' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'JsonFileToDB.Storage.Model' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model' 
        // connection string in the application configuration file.
        public Model()
            : base("name=Model")
        {
           // Database.SetInitializer<Model>(new ModelDBInitializer());
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<FoldersMetadata> FoldersMetadata { get; set; }
        public DbSet<MediaFilesMetadata> MediaFilesMetadata { get; set; }

    }
    public class ModelDBInitializer : DropCreateDatabaseAlways<Model>
    {
        protected override void Seed(Model context)
        {
            base.Seed(context);
        }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}