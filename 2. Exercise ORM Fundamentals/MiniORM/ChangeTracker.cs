using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    public class ChangeTracker<T>
        where T : class, new() // This is because entities are always classes and there we are telling that it is a class with empty constructor
    {
        //Example: Proxy entity is the current state of the entity, the entity is the physical state of the entity in the database
        private readonly List<T> allEntities;
        private readonly List<T> added;
        private readonly List<T> removed;

        public ChangeTracker(IEnumerable<T> entities)
        {
            this.added = new List<T>();
            this.removed = new List<T>();

            this.allEntities = CloneEntities(entities);
        }

        //AllEntities -> proxy entities -> current state of the memory -> locally in the memory
        public IReadOnlyCollection<T> AllEntities => this.allEntities.AsReadOnly();

        //State of the added entities logically, not added physically yet
        public IReadOnlyCollection<T> Added => this.added.AsReadOnly();

        //State of the removed entities logically, not removed physically yet
        public IReadOnlyCollection<T> Removed => this.removed.AsReadOnly();

        public void Add(T item) => this.added.Add(item);
        public void Remove(T item) => this.added.Remove(item);

        //DbSet is the current state of the Database
        public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
            List<T> modifiedEntities = Activator.CreateInstance<List<T>>();

            //It may have composite primary key(multiple primary keys) thats why we name it primaryKeys not primaryKey
            //primaryKeys gets all properties which have primary key (.HasAttribute<KeyAttribute>())
            PropertyInfo[] primaryKeys = typeof(T)
                .GetProperties()
                .Where(propertyInfo => propertyInfo.HasAttribute<KeyAttribute>())
                .ToArray();

            foreach (var proxyEntity in this.AllEntities)
            {
                //TODO obj[]
                IEnumerable<object> primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity);

                //SequenceEqual tells if two arrays values are equal
                T dbEntity = dbSet
                    .Entities
                    .Single(entity => GetPrimaryKeyValues(primaryKeys, entity).SequenceEqual(primaryKeyValues));

                if (IsModified(proxyEntity, dbEntity))
                {
                    modifiedEntities.Add(dbEntity);
                }
            }

            return modifiedEntities;
        }

        private static List<T> CloneEntities(IEnumerable<T> entities)
        {
            List<T> clonedEntities = Activator.CreateInstance<List<T>>(); // Activator.CreateInstance<List<T>>(); == new List<T>(); but it is bad to use 'new' keyword

            //Taking all properties that are valid for clone
            //typeof(T) is the type of the object like Employees.GetProperties()... 
            PropertyInfo[] propertiesToClone = typeof(T)
                .GetProperties()
                .Where(propertyInfo => DbContext.AllowedSqlType.Contains(propertyInfo.PropertyType))
                .ToArray();

            //For each entity in entities we get all properties that we add to clonedEntities
            foreach (var entity in entities)
            {
                T clonedEntity = Activator.CreateInstance<T>(); // Activator.CreateInstance<T>() == new T(); but it is bad to use 'new' keyword

                foreach (var property in propertiesToClone)
                {
                    object value = property.GetValue(entity);

                    property.SetValue(clonedEntity, value);
                }

                clonedEntities.Add(clonedEntity);
            }

            return clonedEntities;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
            => primaryKeys.Select(pk => pk.GetValue(entity));

        private static bool IsModified(T proxyEntity, T dbEntity)
        {
            PropertyInfo[] monitoredProperties = typeof(T)
                .GetProperties()
                .Where(propertyInfo => DbContext.AllowedSqlType.Contains(propertyInfo.PropertyType))
                .ToArray();

            PropertyInfo[] modifiedProperties = monitoredProperties
                .Where(propertyInfo => propertyInfo.GetValue(proxyEntity) != propertyInfo.GetValue(dbEntity))
                .ToArray();

            return modifiedProperties.Any();
        }
    }
}