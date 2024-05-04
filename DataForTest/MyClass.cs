namespace SerializationComparer.ClassesForTest;

internal class MyClass
{
    public Guid id;
    public string name;
    public int age;
    public bool isMen;
    public DateTime birthDate;
    public F f;

    public MyClass Get() => 
        new()
        {
            id = Guid.NewGuid(),
            name = "Petya",
            age = 81,
            isMen = true,
            birthDate = DateTime.Now,
            f = new F().Get()
        };
}
