using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    public abstract class DbContext
    {
        private readonly DatabaseConnection connection;
        private readonly Dictionary<Type, PropertyInfo> dbSetProperties;
        internal static readonly Type[] AllowedSqlType =
        {
            typeof(int),
            typeof(string),
            typeof(double),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime),
        };

        protected DbContext(string connectionString)
        {
            this.connection = new DatabaseConnection(connectionString);
            this.dbSetProperties = this.DiscoverDbSets();

            using (new ConnectionManager(this.connection))
            {
                this.InitializeDbSets();
            }

            this.MapAllRelation();
        }

        public void SaveChanges()
        {
            object[] dbSets = this.dbSetProperties
                .Select(propertyInfo => propertyInfo.Value.GetValue(this))
                .ToArray();

            //Check for invalid entities in dbSet
            //TODO IEnumerable<object> replace var
            foreach (var dbSet in dbSets)
            {
                object[] invalidEntities = dbSets
                    .Where(entity => !IsObjectValid(entity))
                    .ToArray();

                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException(
                        $"{invalidEntities.Length} Invalid Entities found in {dbSets.GetType().Name}!");
                }
            }

            using (new ConnectionManager(this.connection))
            {
                using SqlTransaction transaction = this.connection.StartTransaction();

                foreach (var dbSet in dbSets)
                {
                    Type dbSetType = dbSet
                        .GetType()
                        .GetGenericArguments()
                        .First();

                    MethodInfo persistMethod = typeof(DbContext)
                        .GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.MakeGenericMethod(dbSetType);

                    try
                    {
                        persistMethod.Invoke(this, new object[] { dbSet });
                    }
                    catch (TargetInvocationException tie)
                    {
                        throw tie.InnerException;
                    }
                    catch (InvalidOperationException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                transaction.Commit();
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)

            where TEntity : class, new()
        {
            string tableName = this.GetTableName(typeof(TEntity));

            string[] columns = this.connection.FetchColumnNames(tableName).ToArray();

            if (dbSet.ChangeTracker.Added.Any())
            {
                this.connection.InsertEntities(dbSet.ChangeTracker.Added, tableName, columns);
            }

            TEntity[] modifiedEntities = dbSet.ChangeTracker.GetModifiedEntities(dbSet).ToArray();

            if (modifiedEntities.Any())
            {
                this.connection.UpdateEntities(modifiedEntities, tableName, columns);
            }

            if (dbSet.ChangeTracker.Removed.Any())
            {
                this.connection.DeleteEntities(dbSet.ChangeTracker.Removed, tableName, columns);
            }
        }

        private void InitializeDbSets()
        {
            foreach (var dbSetProperty in this.dbSetProperties)
            {
                Type dbSeType = dbSetProperty.Key;
                PropertyInfo dbSetPropertyInfo = dbSetProperty.Value;

                MethodInfo populateDbSetGeneric = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.MakeGenericMethod(dbSeType);

                populateDbSetGeneric.Invoke(this, new object[] { dbSetProperty });
            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
            where TEntity : class, new()
        {
            IEnumerable<TEntity> entities = LoadTableEntities<TEntity>();

            DbSet<TEntity> dbSetInstance = new DbSet<TEntity>(entities);

            ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstance);
        }

        private void MapAllRelation()
        {
            foreach (var dbSetProperty in this.dbSetProperties)
            {
                Type dbSetType = dbSetProperty.Key;

                MethodInfo mapRelationsGenericMethod = typeof(DbContext)
                    .GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.MakeGenericMethod(dbSetType);

                object dbSet = dbSetProperty.Value.GetValue(this);

                mapRelationsGenericMethod.Invoke(this, new object[] { dbSet });
            }
        }

        private void MapRelations<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            this.MapNavigationProperties(dbSet);

            PropertyInfo[] collections = entityType
                .GetProperties()
                .Where(propertyInfo => propertyInfo.PropertyType.IsGenericType &&
                                       propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                .ToArray();

            foreach (var collection in collections)
            {
                Type collectionType = collection.PropertyType.GenericTypeArguments.First();

                MethodInfo mapCollectionGhenericMethod = typeof(DbContext)
                    .GetMethod("MapCollection", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entityType, collectionType);

                mapCollectionGhenericMethod.Invoke(this, new object[] { dbSet, collection });
            }
        }

        private void MapCollection<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            throw new NotImplementedException();
        }

        private void MapNavigationProperties<TDbSet, ICollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionProperty)
            where TDbSet : class, new()
            where ICollection : class, new()
        {
            Type entityType = typeof(TDbSet);
            Type collectionType = typeof(ICollection);

            PropertyInfo primaryKey = collectionType
                .GetProperties()
                .First(pi => pi.HasAttribute<KeyAttribute>());

            PropertyInfo foreignKey = collectionType
                .GetProperties()
                .First(pi => pi.HasAttribute<KeyAttribute>());
        }

        private static bool IsObjectValid(object entity)
        {
            throw new NotImplementedException();
        }

        private string GetTableName(Type tableType)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>()
            where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}