using System.Text;

Console.OutputEncoding = Encoding.UTF8;

RunMainMenu();

void RunMainMenu()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("APLIKACIJA ZA EVIDWNCIJU POSUDBI U KNJIŽNICI\n");
        Console.WriteLine("1 - Članovi");
        Console.WriteLine("2 - Posudbe");
        Console.WriteLine("0 - Izlaz iz aplikacije\n");

        int choice = ReadMenuChoice(0, 2);

        switch (choice)
        {
            case 1:
                Console.WriteLine("Članovi - skužajte nije dodano.");
                WaitForContinue();
                break;
            case 2:
                Console.WriteLine("Posudbe - skužajte još nije dodano.");
                WaitForContinue();
                break;
            case 0:
                Console.WriteLine("Arivederči!");
                return;
        }
    }
}

int ReadMenuChoice(int minOption, int maxOption)
{
    while (true)
    {
        Console.Write("Odabir: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= minOption && choice <= maxOption)
            return choice;

        Console.WriteLine($"Nije dobar unos. Unesite broj između {minOption} i {maxOption}.");
    }
}

void WaitForContinue()
{
    Console.WriteLine("\nPritisnite bilo koji botun za nastavak"); /* malo splickog štiha */
    Console.ReadKey(true);
}