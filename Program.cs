using SerializationComparer;
using SerializationComparer.ClassesForTest;
using System.Diagnostics;
using System.Text.Json;

#region Initialization

F f = new F().Get();
//MyClass myClass = new MyClass().Get();
//LetterClass letterClass = new LetterClass().Get();

string path = "./DataForTest/LetterClassJson.json";
string jsonText = File.ReadAllText(path);

int countIterations = 100000;
Stopwatch stopwatch = new Stopwatch();
string result = string.Empty;
long timeMs;

#endregion

#region Serialization

// 1. Написать сериализацию свойств или полей класса в строку
// 2. Проверить на классе: class F { int i1, i2, i3, i4, i5; Get() => new F(){ i1 = 1, i2 = 2, i3 = 3, i4 = 4, i5 = 5 }; }
var myJsonString = HomeSerializer.Serialize(f, new HomeJsonSerializerOptions() { IncludeFields = true });

// 3.Замерить время до и после вызова функции (для большей точности можно сериализацию сделать в цикле 100-100000 раз)
stopwatch.Start();
for (int i = 0; i < countIterations; i++)
{
    result = HomeSerializer.Serialize(f, new HomeJsonSerializerOptions() { IncludeFields = true });
}
stopwatch.Stop();
timeMs = stopwatch.ElapsedMilliseconds;

// 4. Вывести в консоль полученную строку и разницу времен
// 5.Отправить в чат полученное время с указанием среды разработки и количества итераций
Console.WriteLine($"Iterations count: {countIterations} \r\nTime: {timeMs} ms \r\nHome serialized string: {result}");

// 6.Замерить время еще раз и вывести в консоль сколько потребовалось времени на вывод текста в консоль
stopwatch.Reset();
stopwatch.Start();
for (int i = 0; i < countIterations; i++)
{
    result = HomeSerializer.Serialize(f, new HomeJsonSerializerOptions() { IncludeFields = true });
    Console.WriteLine(result);
}
stopwatch.Stop();
timeMs = stopwatch.ElapsedMilliseconds;

Console.WriteLine($"Iterations count: {countIterations} \r\nTime: {timeMs} ms");

// 7. Провести сериализацию с помощью каких-нибудь стандартных механизмов (например в JSON)
var jsonString = JsonSerializer.Serialize(f, new JsonSerializerOptions() { IncludeFields = true });

// 8. И тоже посчитать время и прислать результат сравнения
// без вывода на консоль
stopwatch.Reset();
stopwatch.Start();
for (int i = 0; i < countIterations; i++)
{
    result = JsonSerializer.Serialize(f, new JsonSerializerOptions() { IncludeFields = true });
}
stopwatch.Stop();
timeMs = stopwatch.ElapsedMilliseconds;

Console.WriteLine($"Iterations count: {countIterations} \r\nTime: {timeMs} ms \r\nStandard serialized string: {result}");

// с выводом на консоль
stopwatch.Reset();
stopwatch.Start();
for (int i = 0; i < countIterations; i++)
{
    result = JsonSerializer.Serialize(f, new JsonSerializerOptions() { IncludeFields = true });
    Console.WriteLine(result);
}
stopwatch.Stop();
timeMs = stopwatch.ElapsedMilliseconds;

Console.WriteLine($"Iterations count: {countIterations} \r\nTime: {timeMs} ms");

#endregion

#region Deserialization

// 9.Написать десериализацию / загрузку данных из строки (ini/csv-файла) в экземпляр любого класса
var myJsonObject = HomeSerializer.Deserialize<LetterClass>(jsonText, new HomeJsonSerializerOptions() { IncludeFields = true });

// 10. Замерить время на десериализацию
stopwatch.Reset();
stopwatch.Start();
for (int i = 0; i < countIterations; i++)
{
    HomeSerializer.Deserialize<LetterClass>(jsonText, new HomeJsonSerializerOptions() { IncludeFields = true });
}
stopwatch.Stop();
timeMs = stopwatch.ElapsedMilliseconds;

Console.WriteLine($"Iterations count: {countIterations} \r\nTime: {timeMs} ms");

// Стандартная десериализация
var jsonObject = JsonSerializer.Deserialize<LetterClass>(jsonText, new JsonSerializerOptions() { IncludeFields = true, PropertyNameCaseInsensitive = true });

// Замер времени на стандартную десериализацию
stopwatch.Reset();
stopwatch.Start();
for (int i = 0; i < countIterations; i++)
{
    JsonSerializer.Deserialize<LetterClass>(jsonText, new JsonSerializerOptions() { IncludeFields = true, PropertyNameCaseInsensitive = true });
}
stopwatch.Stop();
timeMs = stopwatch.ElapsedMilliseconds;

Console.WriteLine($"Iterations count: {countIterations} \r\nTime: {timeMs} ms");

#endregion
