using System.Reflection;

namespace SerializationComparer;

internal static class Deserializer
{
    /// <summary>
    /// Формирование объекта из строки json
    /// </summary>
    /// <param name="type">Тип объекта</param>
    /// <param name="jsonString">Строка json (сериализованное представление объекта)</param>
    /// <param name="includeFields">Флаг включить ли поля</param>
    /// <returns>Объект</returns>
    public static object FromString(Type type, string jsonString, bool includeFields = false)
    {
        // создание объекта типа type
        var objectFromStr = Activator.CreateInstance(type);

        // атрибуты этого объекта (без вложенности) из переданной строки json 
        Dictionary<string, string> propertyDictionary = GetPropertyDictionary(jsonString);

        // сначала пробег по свойствам этого типа, заполнение свойств объекта значениями
        #region Properties
        // свойства объекта
        var properties = type.GetProperties();

        // пробег по свойствам
        foreach (var property in properties)
        {
            // имя свойства
            string propertyName = property.Name.ToLower();
            // если такого атрибута нет в словаре атрибутов из json, то проходим мимо
            if (!propertyDictionary.Keys.Contains(propertyName)) continue;
            // а если есть - то вытаскиваем значение в зависимости от типа св-ва
            var fieldValue = GetFromStringByType(property.PropertyType, propertyDictionary[propertyName], includeFields);
            // установка значения атрибута объекту
            property.SetValue(objectFromStr, fieldValue);
        }
        #endregion Properties

        // потом пробег по полям
        #region Fields
        // если поля включительно то смотрим их тоже
        if (includeFields)
        {
            // поля объекта
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            // пробег по полям
            foreach (var field in fields)
            {
                // имя поля
                string fieldName = field.Name.ToLower();
                // если такого атрибута нет в словаре атрибутов из json, то проходим мимо
                if (!propertyDictionary.Keys.Contains(fieldName)) continue;
                // а если есть - то вытаскиваем значение в зависимости от типа поля
                var fieldValue = GetFromStringByType(field.FieldType, propertyDictionary[fieldName], includeFields);
                // установка значения атрибута объекту
                field.SetValue(objectFromStr, fieldValue);
            }
        }
        #endregion Fields

        return objectFromStr;
    }

    /// <summary>
    /// Преобразование строкового значения в нужный тип
    /// </summary>
    /// <param name="type">Нужный тип</param>
    /// <param name="str">Строковое значение</param>
    /// <param name="includeFields">Флаг включить ли поля</param>
    /// <returns></returns>
    private static object GetFromStringByType(Type type, string str, bool includeFields)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return null;
        }

        return type.Name switch
        {
            nameof(String) => str,
            nameof(Guid) => Guid.Parse(str),
            nameof(DateTime) => DateTime.Parse(str),
            nameof(Int32) => int.Parse(str),
            nameof(Double) => double.Parse(str.Replace('.', ',')),
            nameof(Boolean) => bool.Parse(str),
            _ => FromString(type, str, includeFields)
        };
    }

    /// <summary>
    /// Получение словаря с атрибутами из строки json
    /// </summary>
    /// <param name="jsonString">Строка json</param>
    /// <returns>Словарь с атрибутами объекта (верхними, не вложенными!)</returns>
    private static Dictionary<string, string> GetPropertyDictionary(string jsonText)
    {
        // результат в виде: имя атрибута - текстовое значение атрибута
        Dictionary<string, string> result = new();

        string propertyName = ""; // имя атрибута
        string word = ""; // слово пробега

        // объект вспомогательного класса для отслеживания фигурных скобок
        Braces braces = new Braces();

        // убираем фигурные скобки в начале и конце
        jsonText = jsonText.Substring(1, jsonText.Length - 2);

        // посимвольный пробег по json-строке
        for (int i = 0; i < jsonText.Length; i++)
        {
            char symbol = jsonText[i];

            // если символ ':', при этом мы не во вложении, при этом имя свойства еще неизвестно - значит это имя свойства
            if (symbol == ':' && braces.IsValid && propertyName == "")
            {
                propertyName = word.Replace("\"", ""); // заполнение имени атрибута
                word = ""; // очищение слова пробега

                continue;
            }

            // если символ ',', при этом мы не во вложении - значит уже значение атрибута
            if (symbol == ',' && braces.IsValid)
            {
                // заполняем значение только в случае если имя атрибута известно, иначе это непонятно что и проходим мимо
                if (propertyName != "")
                {
                    // проверка на существовании в результате, если есть - то второй раз не положить
                    if (!result.ContainsKey(propertyName))
                    {
                        result.Add(propertyName.Trim().ToLower(), word.Replace("\"", "").Trim());
                    }

                    propertyName = ""; // очистка имени атрибута
                    word = "";
                }

                continue;
            }

            // если '{' - значит вложенность
            if (symbol == '{')
            {
                braces.CountOpen++;
            }

            // если '}' - вложенность
            if (symbol == '}')
            {
                braces.CountClose++;
            }

            word += symbol; // если не попали в условия - значит обычный симфол, читаем
        }

        // Последний атрибут. Если известно имя атрибута  берем его значение
        if (propertyName != "" && !result.ContainsKey(propertyName))
        {
            result.Add(propertyName.Trim().ToLower(), word.Replace("\"", "").Trim());
        }

        return result;
    }
}

/// <summary>
/// Вспомогательный класс подсчета фигурных скобок
/// </summary>
internal class Braces
{
    public int CountOpen { get; set; } = 0;
    public int CountClose { get; set; } = 0;

    // "Валидное" состояние - когда кол-во открытых и закрытых скобок равно 
    public bool IsValid => CountOpen == CountClose;
}