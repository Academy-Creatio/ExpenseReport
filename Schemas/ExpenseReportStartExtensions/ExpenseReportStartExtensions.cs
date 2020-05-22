using System;
using System.Collections.Generic;
using Terrasoft.Core.Entities;
using Terrasoft.Core.DB;
using Terrasoft.Common;

namespace ExpenseReportStart
{
    public static class ExpenseReportStartExtensions
    {
        public static bool IsChangeInteresting<Entity>(this Entity entity, string[] InterestingColumns, EntityColumnValueCollection entityColumnValues = null)
            where Entity : Terrasoft.Core.Entities.Entity
        {
            bool result = false;
            IEnumerable<EntityColumnValue> changedColumns;

            if (entityColumnValues != null)
            {
                changedColumns = entityColumnValues;
            }
            else
            {
                changedColumns = entity.GetChangedColumnValues();
            }

            foreach (EntityColumnValue column in changedColumns)
            {
                if (Array.IndexOf(InterestingColumns, column.Name) > -1)
                    return true;
            }
            return result;
        }

        public static bool DoesExist<Entity>(this Entity entity, string RootSchemName, object searchValue, string searchColumn)
            where Entity : Terrasoft.Core.Entities.Entity
        {
            Select select = new Select(entity.UserConnection)
                .Column(Func.Count("Id"))
                .From(RootSchemName)
                .Where(searchColumn).IsEqual(Column.Parameter(searchValue)) as Select;
            int count = select.ExecuteScalar<int>();

            return (count == 0) ? false : true;
        }

        public static Guid FindIdByValue<Entity>(this Entity entity, string RootSchemName, object searchValue, string searchColumn)
            where Entity : Terrasoft.Core.Entities.Entity
        {
            Select select = new Select(entity.UserConnection)
                .Top(1)
                .Column("Id")
                .From(RootSchemName)
                .Where(searchColumn).IsEqual(Column.Parameter(searchValue)) as Select;
            Guid Id = select.ExecuteScalar<Guid>();

            return Id;
        }

        /// <summary>
        /// Uses Select to Find value by Id
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="RootSchemName">Table to search</param>
        /// <param name="Id">Record Id</param>
        /// <param name="SearchColumn">Column to return</param>
        /// <example>
        /// <returns></returns>
        public static T FindValueById<T>(this Entity entity, string RootSchemName, Guid Id, string SearchColumn)
            //where Entity : Terrasoft.Core.Entities.Entity 
            where T : IConvertible
        {
            Select select = new Select(entity.UserConnection)
                .Top(1)
                .Column(SearchColumn)
                .From(RootSchemName)
                .Where("Id").IsEqual(Column.Parameter(Id)) as Select;
            var result = select.ExecuteScalar<T>();
            return result;
        }

        /// <summary>
        /// Uses Select to Find value by Id <see cref="Terrasoft.Core.DB.Select"/>
        /// </summary>
        /// <example>
        /// Get currency short name by Id
        /// <code>
        /// string shortCurrency = entity.FindValueById<string>("Currency", currencyId, "ShortName");
        /// </code>
        /// </example>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="entity"></param>
        /// <param name="RootSchemName">Table to Search</param>
        /// <param name="Id">Id to Search</param>
        /// <param name="SearchColumn">Return Column</param>
        /// <returns>value of T found in SearchColumn by Id</returns>
        public static string GetLocalizableString<Entity>(this Entity entity, string resourceName, string schemaName) where Entity : Terrasoft.Core.Entities.Entity
        {
            var localizableString = string.Format("LocalizableStrings.{0}.Value", resourceName);
            string result = new LocalizableString(entity.UserConnection.ResourceStorage,
                schemaName, localizableString);
            return result;
        }
    }
}