using System.Reflection;
using System.Text;

namespace SerializationComparer;

internal static class Serializer
{
    /// <summary>
    /// Формирование строки json из объекта
    /// </summary>
    /// <param name="type">Тип объекта</param>
    /// <param name="obj">Объект, который нужно преобразовать в json</param>
    /// <param name="includeFields">Флаг включить ли поля</param>
    /// <returns>Json-строка</returns>
    public static string ToString(Type type, object? obj, bool includeFields = false)
    {
        // Строка начинается с открывающейся фигурной скобки
        StringBuilder jsonStr = new StringBuilder("{");

        // сначала пробег по свойствам этого типа
        #region Properties
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            // преобразование поля в строковое значение json-атрибута в зависимости от типа
            string strPropertyValue = GetStringByType(property.PropertyType, property.GetValue(obj), includeFields);
            // формирование строки атрибута: "наименование атрибута в кавычках":его значение,
            string strPropertyName = $"\"{property.Name}\":{strPropertyValue},";
            // конкатенация в единую json-строку
            jsonStr.Append(strPropertyName);
        }
        #endregion

        // потом пробег по полям
        #region Fields
        // если поля включительно то смотрим их тоже
        if (includeFields)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                // преобразование поля в строковое значение json-атрибута в зависимости от типа
                string strFieldValue = GetStringByType(field.FieldType, field.GetValue(obj), includeFields);
                // формирование строки атрибута: "наименование атрибута в кавычках":его значение,
                string strFieldName = $"\"{field.Name}\":{strFieldValue},";
                // конкатенация в единую json-строку
                jsonStr.Append(strFieldName);
            }
        }
        #endregion

        // убираем последнюю запятую
        jsonStr = jsonStr[jsonStr.Length - 1] == ',' ? jsonStr.Remove(jsonStr.Length - 1, 1) : jsonStr;
        jsonStr.Append("}"); // json должен заканчиваться закрывающейся фигурной скобкой

        return jsonStr.ToString();
    }

    /// <summary>
    /// Преобразование объекта определенного типа в строковое представление
    /// </summary>
    /// <param name="type">Тип объекта</param>
    /// <param name="obj">Объект</param>
    /// <param name="includeFields">Флаг включить ли поля</param>
    /// <returns>Строковое представление объекта для json</returns>
    private static string GetStringByType(Type type, object? obj, bool includeFields)
    {
        if (obj == null)
        {
            return null;
        }

        return type.Name switch
        {
            nameof(String) => $"\"{obj}\"",
            nameof(Guid) => $"\"{obj}\"",
            nameof(DateTime) => $"\"{Convert.ToDateTime(obj):O}\"",
            nameof(Int32) => obj.ToString(),
            nameof(Double) => obj.ToString().Replace(',', '.'),
            nameof(Boolean) => obj.ToString().ToLower(),
            _ => ToString(type, obj, includeFields)
        };
    }
}
