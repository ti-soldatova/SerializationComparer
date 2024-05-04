namespace SerializationComparer.ClassesForTest;

internal class LetterClass
{
    public AA A { get; set; }

    public string P { get; set; }
    public double Q { get; set; }

    public LetterClass Get()
    {
        return new LetterClass()
        {
            A = new AA()
            {
                B = new BB()
                {
                    C = new CC()
                    {
                        D = 6,
                        E = "hhh"
                    },
                    F = new FF()
                    {
                        G = new GG()
                        {
                            H = "huh",
                            I = new II()
                            {
                                J = "ijoih",
                                K = true
                            },
                            L = 90
                        },
                        M = "iuhuih"
                    }
                },
                N = new NN() { O = 7 }
            },
            P = "huioh",
            Q = 89.9
        };
    }
}

internal class AA
{
    public BB B { get; set; }
    public NN N { get; set; }
}

internal class BB
{
    public CC C { get; set; }
    public FF F { get; set; }
}

internal class CC
{
    public int D { get; set; }
    public string E { get; set; }
}

internal class FF
{
    public GG G { get; set; }
    public string M { get; set; }
}

internal class GG
{
    public string H { get; set; }
    public II I { get; set; }
    public int L { get; set; }
}

internal class II
{
    public string J { get; set; }
    public bool K { get; set; }
}

internal class NN
{
    public int O { get; set; }
}
