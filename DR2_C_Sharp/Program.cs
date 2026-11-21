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
                RunLoansMenu ();
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
                DeleteMember();
                WaitForContinue();
                break;
            case 3:
                EditMember();
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

void DeleteMember()
{
    Console.Clear();
    Console.WriteLine("BRISANJE ČLANA\n");

    if (members.Count == 0)
    {
        Console.WriteLine("Nema upisanih članova.");
        return;
    }

    Console.WriteLine("1 - Brisanje po ID-u");
    Console.WriteLine("2 - Brisanje po imenu i prezimenu");
    Console.WriteLine("0 - Odustani\n");

    int choice = ReadMenuChoice(0, 2);
    if (choice == 0)
    {
        return;
    }

    int targetId = -1;

    if (choice == 1)
    {
        PrintSimpleMembersList();
        Console.Write("\nUnesite ID člana: ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int id) || !members.ContainsKey(id))
        {
            Console.WriteLine("Član s tim ID-om ne postoji.");
            return;
        }

        targetId = id;
    }
    else if (choice == 2)
    {
        string firstName = ReadText("Ime: ");
        string lastName = ReadText("Prezime: ");

        List<int> foundIds = new List<int>();
        foreach (var item in members)
        {
            if (item.Value.FirstName.ToLower() == firstName.ToLower() &&
                item.Value.LastName.ToLower() == lastName.ToLower())
            {
                foundIds.Add(item.Key);
            }
        }

        if (foundIds.Count == 0)
        {
            Console.WriteLine("Nije pronađen nijedan član s tim imenom i prezimenom.");
            return;
        }
        else if (foundIds.Count == 1)
        {
            targetId = foundIds[0];
        }
        else
        {
            Console.WriteLine("\nPronađeno je više članova s tim imenom:");
            foreach (int foundId in foundIds)
            {
                var m = members[foundId];
                string dateText = m.BirthDate.ToString("yyyy-MM-dd");
                Console.WriteLine($"[{foundId}] {m.FirstName} {m.LastName} ({dateText})");
            }

            Console.Write("\nUnesite ID člana kojeg želite obrisati: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int id) || !foundIds.Contains(id))
            {
                Console.WriteLine("Neispravan ID.");
                return;
            }

            targetId = id;
        }
    }

    if (HasActiveLoans(targetId))
    {
        Console.WriteLine("\nČlan se ne može obrisati jer ima barem jednu nevraćenu knjigu!");
        return;
    }

    var memberToDelete = members[targetId];
    if (AskConfirmation($"\nJeste li sigurni da želite obrisati člana {memberToDelete.FirstName} {memberToDelete.LastName}? (da/ne): "))
    {
        members.Remove(targetId);
        Console.WriteLine("Član je uspješno obrisan.");
    }
    else
    {
        Console.WriteLine("Brisanje je otkazano.");
    }
}

void EditMember()
{
    Console.Clear();
    Console.WriteLine("UREĐIVANJE ČLANA\n");

    if (members.Count == 0)
    {
        Console.WriteLine("Nema upisanih članova.");
        return;
    }

    PrintSimpleMembersList();
    Console.Write("\nUnesite ID člana za uređivanje: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int id) || !members.ContainsKey(id))
    {
        Console.WriteLine("Član s tim ID-om ne postoji.");
        return;
    }

    var current = members[id];
    Console.WriteLine("\nOstavi prazno i stisni Enter ako ne želiš mijenjati polje.\n");

    Console.Write($"Ime ({current.FirstName}): ");
    string? newFirstName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newFirstName))
    {
        newFirstName = current.FirstName;
    }
    else
    {
        newFirstName = newFirstName.Trim();
    }

    Console.Write($"Prezime ({current.LastName}): ");
    string? newLastName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newLastName))
    {
        newLastName = current.LastName;
    }
    else
    {
        newLastName = newLastName.Trim();
    }

    DateTime newBirthDate = current.BirthDate;
    string currentBirthText = current.BirthDate.ToString("yyyy-MM-dd");
    while (true)
    {
        Console.Write($"Datum rođenja ({currentBirthText}): ");
        string? dateInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(dateInput))
        {
            break;
        }

        if (dateInput.Length == 10 && dateInput[4] == '-' && dateInput[7] == '-')
        {
            string[] parts = dateInput.Split('-');
            bool okYear = int.TryParse(parts[0], out int year);
            bool okMonth = int.TryParse(parts[1], out int month);
            bool okDay = int.TryParse(parts[2], out int day);

            if (okYear && okMonth && okDay && year >= 1900 && year <= 2100 && month >= 1 && month <= 12 && day >= 1 && day <= DateTime.DaysInMonth(year, month))
            {
                DateTime parsedDate = new DateTime(year, month, day);
                if (parsedDate <= DateTime.Today)
                {
                    newBirthDate = parsedDate;
                    break;
                }
                else
                {
                    Console.WriteLine("Datum rođenja ne može biti u budućnosti!");
                    continue;
                }
            }
        }

        Console.WriteLine("Krivi format datuma! Unesi kao YYYY-MM-DD.");
    }

    if (AskConfirmation("\nSpremi promjene? (da/ne): "))
    {
        members[id] = (newFirstName, newLastName, newBirthDate, current.Loans);
        Console.WriteLine("Podaci člana su ažurirani.");
    }
    else
    {
        Console.WriteLine("Uređivanje je otkazano.");
    }
}

void PrintSimpleMembersList()
{
    Console.WriteLine("Popis članova:");
    foreach (var item in members)
    {
        string dateText = item.Value.BirthDate.ToString("yyyy-MM-dd");
        Console.WriteLine($"[{item.Key}] {item.Value.FirstName} {item.Value.LastName} ({dateText})");
    }
}

bool AskConfirmation(string message)
{
    while (true)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (input != null)
        {
            string answer = input.Trim().ToLower();
            if (answer == "da")
            {
                return true;
            }
            if (answer == "ne")
            {
                return false;
            }
        }

        Console.WriteLine("Unesite 'da' ili 'ne'.");
    }
}
void RunLoansMenu()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("POSUDBE\n");
        Console.WriteLine("1 - Unos nove posudbe");
        Console.WriteLine("2 - Vraćanje knjige");
        Console.WriteLine("3 - Brisanje posudbe");
        Console.WriteLine("4 - Uređivanje postojeće posudbe");
        Console.WriteLine("5 - Pregled svih posudbi");
        Console.WriteLine("6 - Izvještaji i analize");
        Console.WriteLine("0 - Povratak na glavni izbornik\n");

        int choice = ReadMenuChoice(0, 6);

        switch (choice)
        {
            case 1:
                AddLoan();
                WaitForContinue();
                break;
            case 2:
                ReturnBook();
                WaitForContinue();
                break;
            case 3:
                DeleteLoan();
                WaitForContinue();
                break;
            case 4:
                EditLoan();
                WaitForContinue();
                break;
            case 5:
                Console.WriteLine("Pregled posudbi - još nije dodano.");
                WaitForContinue();
                break;
            case 6:
                Console.WriteLine("Izvještaji - još nije dodano.");
                WaitForContinue();
                break;
            case 0:
                return;
        }
    }
}

void AddLoan()
{
    Console.Clear();
    Console.WriteLine("UNOS NOVE POSUDBE\n");

    if (members.Count == 0)
    {
        Console.WriteLine("Nema upisanih članova.");
        return;
    }

    PrintSimpleMembersList();
    Console.Write("\nUnesite ID člana: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int memberId) || !members.ContainsKey(memberId))
    {
        Console.WriteLine("Član s tim ID-om ne postoji.");
        return;
    }

    string title = ReadText("Naslov knjige: ");
    string author = ReadText("Autor knjige: ");

    DateTime loanDate = ReadDate("Datum posudbe (YYYY-MM-DD): ");
    while (loanDate > DateTime.Today)
    {
        Console.WriteLine("Datum posudbe ne može biti u budućnosti!");
        loanDate = ReadDate("Datum posudbe (YYYY-MM-DD): ");
    }

    int loanId = nextLoanId;
    nextLoanId++;

    int lateDays = CalculateLateDays(loanDate, null);
    decimal lateFee = CalculateLateFee(lateDays);

    members[memberId].Loans[loanId] = (title, author, loanDate, null, lateDays, lateFee);

    Console.WriteLine($"\nPosudba je uspješno dodana! ID posudbe je: {loanId}");
}

void ReturnBook()
{
    Console.Clear();
    Console.WriteLine("VRAĆANJE KNJIGE\n");

    bool hasActive = false;
    Console.WriteLine("Aktivne (nevraćene) posudbe:");
    foreach (var memberEntry in members)
    {
        foreach (var loanEntry in memberEntry.Value.Loans)
        {
            if (!IsReturned(loanEntry.Value.ReturnDate))
            {
                string loanDateText = loanEntry.Value.LoanDate.ToString("yyyy-MM-dd");
                Console.WriteLine($"[{loanEntry.Key}] {loanEntry.Value.Title} - {loanEntry.Value.Author} | Član: {memberEntry.Value.FirstName} {memberEntry.Value.LastName} | Posuđeno: {loanDateText}");
                hasActive = true;
            }
        }
    }

    if (!hasActive)
    {
        Console.WriteLine("Nema aktivnih posudbi za vraćanje.");
        return;
    }

    Console.Write("\nUnesite ID posudbe koju želite vratiti: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int targetLoanId))
    {
        Console.WriteLine("Neispravan ID posudbe.");
        return;
    }

    int foundMemberId = -1;
    foreach (var memberEntry in members)
    {
        if (memberEntry.Value.Loans.ContainsKey(targetLoanId))
        {
            foundMemberId = memberEntry.Key;
            break;
        }
    }

    if (foundMemberId == -1)
    {
        Console.WriteLine("Posudba s tim ID-om ne postoji.");
        return;
    }

    var loan = members[foundMemberId].Loans[targetLoanId];
    if (IsReturned(loan.ReturnDate))
    {
        Console.WriteLine("Ova knjiga je već vraćena.");
        return;
    }

    DateTime returnDate = ReadDate("Datum vraćanja (YYYY-MM-DD): ");
    while (returnDate < loan.LoanDate || returnDate > DateTime.Today)
    {
        Console.WriteLine("Datum vraćanja ne može biti prije datuma posudbe niti u budućnosti!");
        returnDate = ReadDate("Datum vraćanja (YYYY-MM-DD): ");
    }

    int lateDays = CalculateLateDays(loan.LoanDate, returnDate);
    decimal lateFee = CalculateLateFee(lateDays);

    members[foundMemberId].Loans[targetLoanId] = (loan.Title, loan.Author, loan.LoanDate, returnDate, lateDays, lateFee);

    string feeText = lateFee.ToString("0.00");
    Console.WriteLine("\nKnjiga je uspješno vraćena!");
    Console.WriteLine($"Dani kašnjenja: {lateDays}");
    Console.WriteLine($"Zakasnina: {feeText} €");
}
bool PrintSimpleLoansList()
{
    bool hasAny = false;
    foreach (var memberEntry in members)
    {
        foreach (var loanEntry in memberEntry.Value.Loans)
        {
            if (!hasAny)
            {
                Console.WriteLine("Popis posudbi:");
                hasAny = true;
            }

            var loan = loanEntry.Value;
            string loanDateText = loan.LoanDate.ToString("yyyy-MM-dd");
            string returnText = "nije vraćeno";
            if (loan.ReturnDate.HasValue)
            {
                returnText = loan.ReturnDate.Value.ToString("yyyy-MM-dd");
            }

            Console.WriteLine($"[{loanEntry.Key}] {loan.Title} - {loan.Author} | Član: {memberEntry.Value.FirstName} {memberEntry.Value.LastName} | Posuđeno: {loanDateText} | Vraćeno: {returnText}");
        }
    }

    return hasAny;
}

void DeleteLoan()
{
    Console.Clear();
    Console.WriteLine("BRISANJE POSUDBE\n");

    if (!PrintSimpleLoansList())
    {
        Console.WriteLine("Nema nijedne posudbe, sve je prazno.");
        return;
    }

    Console.Write("\nUpiši ID posudbe za brisanje: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int loanId))
    {
        Console.WriteLine("Krivi ID, aj lipo broj unesi.");
        return;
    }

    int foundMemberId = -1;
    foreach (var memberEntry in members)
    {
        if (memberEntry.Value.Loans.ContainsKey(loanId))
        {
            foundMemberId = memberEntry.Key;
            break;
        }
    }

    if (foundMemberId == -1)
    {
        Console.WriteLine("Nema posudbe s tim ID-om, krivo si.");
        return;
    }

    var loan = members[foundMemberId].Loans[loanId];
    if (AskConfirmation($"\nJesi siguran da želiš izbrisat '{loan.Title}'? (da/ne): "))
    {
        members[foundMemberId].Loans.Remove(loanId);
        Console.WriteLine("Obrisano, nema je više.");
    }
    else
    {
        Console.WriteLine("Ništa onda");
    }
}

void EditLoan()
{
    Console.Clear();
    Console.WriteLine("UREĐIVANJE POSUDBE\n");

    if (!PrintSimpleLoansList())
    {
        Console.WriteLine("Nema nijedne posudbe, ne može se uređivati.");
        return;
    }

    Console.Write("\nUpiši ID posudbe koju ćeš uređivati: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int loanId))
    {
        Console.WriteLine("Krivi ID, aj lipo broj unesi.");
        return;
    }

    int foundMemberId = -1;
    foreach (var memberEntry in members)
    {
        if (memberEntry.Value.Loans.ContainsKey(loanId))
        {
            foundMemberId = memberEntry.Key;
            break;
        }
    }

    if (foundMemberId == -1)
    {
        Console.WriteLine("Nema posudbe s tim ID-om, krivo si.");
        return;
    }

    var current = members[foundMemberId].Loans[loanId];
    Console.WriteLine("\nSamo stisni Enter ako nećeš mjenjati polje.\n");

    Console.Write($"Naslov ({current.Title}): ");
    string? newTitle = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newTitle))
    {
        newTitle = current.Title;
    }
    else
    {
        newTitle = newTitle.Trim();
    }

    Console.Write($"Autor ({current.Author}): ");
    string? newAuthor = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newAuthor))
    {
        newAuthor = current.Author;
    }
    else
    {
        newAuthor = newAuthor.Trim();
    }

    DateTime newLoanDate = current.LoanDate;
    string currentLoanText = current.LoanDate.ToString("yyyy-MM-dd");
    if (AskConfirmation($"Želiš li mjenjati datum posudbe ({currentLoanText})? (da/ne): "))
    {
        newLoanDate = ReadDate("Novi datum posudbe (YYYY-MM-DD): ");
        while (newLoanDate > DateTime.Today)
        {
            Console.WriteLine("Ne može datum posudbe bit u budućnosti!");
            newLoanDate = ReadDate("Novi datum posudbe (YYYY-MM-DD): ");
        }
    }

    DateTime? newReturnDate = current.ReturnDate;
    string currentReturnText = "nije vraćeno";
    if (current.ReturnDate.HasValue)
    {
        currentReturnText = current.ReturnDate.Value.ToString("yyyy-MM-dd");
    }

    bool mustChangeReturn = false;
    if (newReturnDate.HasValue && newReturnDate.Value < newLoanDate)
    {
        Console.WriteLine("Stari datum vraćanja ti je sad prije novog datuma posudbe, mora se mjenjati.");
        mustChangeReturn = true;
    }

    if (mustChangeReturn || AskConfirmation($"Želiš li mjenjati datum vraćnja ({currentReturnText})? (da/ne): "))
    {
        if (AskConfirmation("Je li knjiga vraćena? (da/ne): "))
        {
            DateTime tempReturn = ReadDate("Datum vraćanja (YYYY-MM-DD): ");
            while (tempReturn < newLoanDate || tempReturn > DateTime.Today)
            {
                Console.WriteLine("Ne može datum vraćanja bit prije posudbe ni u budućnosti!");
                tempReturn = ReadDate("Datum vraćanja (YYYY-MM-DD): ");
            }
            newReturnDate = tempReturn;
        }
        else
        {
            newReturnDate = null;
        }
    }

    int newLateDays = CalculateLateDays(newLoanDate, newReturnDate);
    decimal newLateFee = CalculateLateFee(newLateDays);

    if (AskConfirmation("\n Želiš li spremiti ove promjene? (da/ne): "))
    {
        members[foundMemberId].Loans[loanId] = (newTitle, newAuthor, newLoanDate, newReturnDate, newLateDays, newLateFee);
        Console.WriteLine("Spremljeno i updateano.");
    }
    else
    {
        Console.WriteLine("Ništa onda, ostalo je sve po starom.");
    }
}