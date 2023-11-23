using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SQLLib.Annotations;

namespace SQLLib.Utils
{
    public static class SQLConverter
    {
        public static T CastToEntity<T>(this DataRow dataRow) where T : new()
        {
            T instance = new T();
            var properties = instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                var columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true) as ColumnAttribute[];
                if (columnAttributes is null || columnAttributes.Length < 1) continue;

                var columnAttribute = columnAttributes[0];
                var value = dataRow[columnAttribute.Name];
                if (value == DBNull.Value || value is null)
                {
                    if (columnAttribute.IsNullable) value = null;
                    else throw new ArgumentNullException($"{property.Name}은 null일 수 없습니다.");
                }

                property.SetValue(instance, value, null);
            }

            return instance;
        }

        public static List<T> CastToEntities<T>(this DataRowCollection dataRowCollection) where T : new()
        {
            var result = new List<T>(dataRowCollection.Count);

            foreach (DataRow dataRow in dataRowCollection)
            {
                result.Add(dataRow.CastToEntity<T>());
            }

            return result;
        }
    }
}
