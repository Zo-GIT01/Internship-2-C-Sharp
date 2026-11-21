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
                RunMembersMenu();
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
void RunMembersMenu()
{
   
        while (true)
    {
        Console.Clear();
        Console.WriteLine("ČLANOVI\n");
        Console.WriteLine("1 - Unos novog člana");
        Console.WriteLine("2 - Brisanje člana");
        Console.WriteLine("3 - Uređivanje člana");
        Console.WriteLine("4 - Pregled svih članova");
        Console.WriteLine("0 - Povratak na glavni izbornik\n");

        int choice = ReadMenuChoice(0, 4);

        switch (choice)
        {
            case 1:
                AddMember();
                WaitForContinue();
                break;
            case 2:
                Console.WriteLine("Brisanje člana - scusi još nije dodano.");
                WaitForContinue();
                break;
            case 3:
                Console.WriteLine("Uređivanje člana - scusi još nije dodano.");
                WaitForContinue();
                break;
            case 4:
                ShowMembers();
                break;
            case 0:
                return;
        }
    }
}

void ShowMembers()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("PREGLED ČLANOVA\n");
        Console.WriteLine("1 - Svi članovi abecedno po prezimenu");
        Console.WriteLine("2 - Članovi mlađi od 18 godina");
        Console.WriteLine("3 - Članovi s barem jednom nevraćenom knjigom");
        Console.WriteLine("0 - Povratak\n");

        int choice = ReadMenuChoice(0, 3);

        if (choice == 0)
        {
            return;
        }

        Console.Clear();

        if (choice == 1)
        {
            List<int> ids = GetSortedMemberIds();
            foreach (int id in ids)
            {
                var m = members[id];
                Console.WriteLine($"[{id}] {m.LastName}, {m.FirstName} ({CalculateAge(m.BirthDate)}) - broj posudbi: {m.Loans.Count}");
            }
        }
        else if (choice == 2)
        {
            bool found = false;
            foreach (var item in members)
            {
                int age = CalculateAge(item.Value.BirthDate);
                if (age < AdultAge)
                {
                    Console.WriteLine($"[{item.Key}] {item.Value.LastName}, {item.Value.FirstName} ({age}) - broj posudbi: {item.Value.Loans.Count}");
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("Nema maloljetnih članova.");
            }
        }
        else if (choice == 3)
        {
            bool found = false;
            foreach (var item in members)
            {
                if (HasActiveLoans(item.Key))
                {
                    int age = CalculateAge(item.Value.BirthDate);
                    Console.WriteLine($"[{item.Key}] {item.Value.LastName}, {item.Value.FirstName} ({age}) - broj posudbi: {item.Value.Loans.Count}");
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("Nema članova s nevraćenim knjigama.");
            }
        }

        WaitForContinue();
    }
}

List<int> GetSortedMemberIds()
{
    List<int> ids = new List<int>(members.Keys);

    for (int i = 0; i < ids.Count - 1; i++)
    {
        for (int j = i + 1; j < ids.Count; j++)
        {
            string last1 = members[ids[i]].LastName;
            string last2 = members[ids[j]].LastName;

            if (string.Compare(last1, last2, true) > 0)
            {
                int temp = ids[i];
                ids[i] = ids[j];
                ids[j] = temp;
            }
        }
    }

    return ids;
}

int CalculateAge(DateTime birthDate)
{
    DateTime today = DateTime.Today;
    int age = today.Year - birthDate.Year;

    if (today.Month < birthDate.Month || (today.Month == birthDate.Month && today.Day < birthDate.Day))
    {
        age--;
    }

    return age;
}

bool IsReturned(DateTime? returnDate)
{
    return returnDate.HasValue;
}

bool HasActiveLoans(int memberId)
{
    foreach (var loan in members[memberId].Loans.Values)
    {
        if (!IsReturned(loan.ReturnDate))
        {
            return true;
        }
    }

    return false;
}

void AddMember()
{
    Console.Clear();
    Console.WriteLine("UNOS NOVOG ČLANA\n");

    string firstName = ReadText("Ime: ");
    string lastName = ReadText("Prezime: ");

    DateTime birthDate = ReadDate("Datum rođenja (YYYY-MM-DD): ");
    while (birthDate > DateTime.Today)
    {
        Console.WriteLine("Datum rođenja ne može biti u budućnosti!");
        birthDate = ReadDate("Datum rođenja (YYYY-MM-DD): ");
    }

    int id = nextMemberId;
    nextMemberId++;

    var loans = new Dictionary<int, (string Title, string Author, DateTime LoanDate, DateTime? ReturnDate, int LateDays, decimal LateFee)>();
    members[id] = (firstName, lastName, birthDate, loans);

    Console.WriteLine($"\nČlan uspješno dodan! ID člana je: {id}");
}

string ReadText(string message)
{
    while (true)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        Console.WriteLine(" Ej! Šta je s glavon!? Polje ne smije biti prazno!");
    }
}

DateTime ReadDate(string message)
{
    while (true)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (input != null && input.Length == 10 && input[4] == '-' && input[7] == '-')
        {
            string[] parts = input.Split('-');
            bool okYear = int.TryParse(parts[0], out int year);
            bool okMonth = int.TryParse(parts[1], out int month);
            bool okDay = int.TryParse(parts[2], out int day);

            if (okYear && okMonth && okDay && year >= 1900 && year <= 2100 && month >= 1 && month <= 12 && day >= 1 && day <= DateTime.DaysInMonth(year, month))
            {
                return new DateTime(year, month, day);
            }
        }

        Console.WriteLine("Krivi format datuma! Sredi glavu i unesi lipo kao YYYY-MM-DD (npr. 2002-05-14).");
    }
}