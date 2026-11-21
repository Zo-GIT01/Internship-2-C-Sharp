using System.Text;

const int LoanPeriodDays = 21;
const decimal DailyLateFee = 0.50m;
const int AdultAge = 18;

Console.OutputEncoding = Encoding.UTF8;

var members = new Dictionary<int, (string FirstName, string LastName, DateTime BirthDate, Dictionary<int, (string Title, string Author, DateTime LoanDate, DateTime? ReturnDate, int LateDays, decimal LateFee)> Loans)>();
int nextMemberId = 1;
int nextLoanId = 1;

SeedInitialData();
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

void SeedInitialData()
{
    int ninoId = AddSeedMember("Nino", "Milas", new DateTime(1998, 4, 12));
    AddSeedLoan(ninoId, "Anger Management for Dummies", "W. Doyle Gentry", new DateTime(2026, 9, 1), new DateTime(2026, 9, 15));
    AddSeedLoan(ninoId, "Beyond Anger", "Thomas Harbin", new DateTime(2026, 10, 1), new DateTime(2026, 10, 29));
    AddSeedLoan(ninoId, "The Cow in the Parking Lot", "Leonard Scheff", new DateTime(2026, 10, 15), null);

    int davidId = AddSeedMember("David", "Anđelić", new DateTime(2010, 8, 23));
    AddSeedLoan(davidId, "Crvenkapica", "Braća Grimm", new DateTime(2026, 10, 10), new DateTime(2026, 10, 20));
    AddSeedLoan(davidId, "Tri praščića", "Joseph Jacobs", new DateTime(2026, 11, 10), null);

    int lukaId = AddSeedMember("Luka", "Žuljević", new DateTime(1985, 11, 3));
    AddSeedLoan(lukaId, "Overcoming Impotence", "J. Stephen Jones", new DateTime(2026, 9, 10), new DateTime(2026, 10, 8));
    AddSeedLoan(lukaId, "The Impotence Cure", "Mark J. Newman", new DateTime(2026, 10, 1), new DateTime(2026, 10, 12));
}

int AddSeedMember(string firstName, string lastName, DateTime birthDate)
{
    int id = nextMemberId;
    nextMemberId++;

    var loans = new Dictionary<int, (string Title, string Author, DateTime LoanDate, DateTime? ReturnDate, int LateDays, decimal LateFee)>();
    members[id] = (firstName, lastName, birthDate, loans);

    return id;
}

void AddSeedLoan(int memberId, string title, string author, DateTime loanDate, DateTime? returnDate)
{
    int loanId = nextLoanId;
    nextLoanId++;

    int lateDays = CalculateLateDays(loanDate, returnDate);
    decimal lateFee = CalculateLateFee(lateDays);

    members[memberId].Loans[loanId] = (title, author, loanDate, returnDate, lateDays, lateFee);
}

int CalculateLateDays(DateTime loanDate, DateTime? returnDate)
{
    DateTime referenceDate = DateTime.Today;
    if (returnDate.HasValue)
    {
        referenceDate = returnDate.Value.Date;
    }

    int totalDays = (referenceDate - loanDate.Date).Days;
    int lateDays = totalDays - LoanPeriodDays;

    if (lateDays > 0)
    {
        return lateDays;
    }

    return 0;
}

decimal CalculateLateFee(int lateDays)
{
    return lateDays * DailyLateFee;
}