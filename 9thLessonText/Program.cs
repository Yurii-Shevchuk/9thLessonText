using System.Text;
using System.Text.RegularExpressions;

namespace Lesson_9_Text_And_Files
{
    internal static class Program
    {
        static string database = "./../../../db.csv";
        static (string name, string phone, DateTime birth)[] contacts;

        static void Main(string[] args)
        {
            string[] records = ReadDatabaseAllTextLines(database);
            contacts = ConvertStringsToContacts(records);
            bool isExit = false;
            while (!isExit)
            {
                UserInteraction(ref isExit);
            }
            Console.WriteLine("Bye-bye!");
        }

        static void UserInteraction(ref bool isExit)
        {
            Console.WriteLine("1. Write all contacts");
            Console.WriteLine("2. Add new contact");
            Console.WriteLine("3. Edit contact");
            Console.WriteLine("4. Search by name or phone number"); 
            Console.WriteLine("5. Save");
            Console.WriteLine("6. Sort by name");
            Console.WriteLine("7. Exit the program");
            int input;
            try
            {
             input = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Your input was incorrect, please try again");
                UserInteraction(ref isExit);
                return;
            }
            catch (OverflowException)
            {
                Console.WriteLine("There is no possible way this program could have THIS many functions, try better");
                UserInteraction(ref isExit);
                return;
            }
            switch (input)
            {
                case 1:
                    WriteAllContactsToConsole();
                    break;
                case 2:
                    AddNewContact();
                    break;
                case 3:
                    EditContact();
                    break;
                case 4:
                    SearchContact();
                    break;
                case 5:
                    SaveContactsToFile();
                    break;
                case 6:
                    SortContacts();
                    break;
                case 7:
                    isExit = true;
                    return;
                default:
                    Console.WriteLine("No such operation.");
                    break;
            }
        }

        static void AddNewContact()
        {
            Console.Write("Please, enter your name: ");
            string newName = Console.ReadLine().Trim();
            newName = newName == "" ? "John" : newName;

            Console.Write("Please, enter your phone number (starting from 380): ");
            string newPhoneNumber = FormatPhoneNumber(Console.ReadLine());

            Console.Write("Please, enter your date of birth (dd.mm.yyyy format): ");
            DateTime newDate;
            try
            {
                newDate = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("You made a mistake during date input, we'll set up placeholder date so that you can change it via edit function");
            }
            finally
            {
                newDate = DateTime.ParseExact("01.01.2000", "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            }

            var newEntry = (newName, newPhoneNumber, newDate);
            var newContacts = new (string, string, DateTime)[contacts.Length + 1];
            Array.Copy(contacts, newContacts, contacts.Length);
            newContacts[newContacts.Length - 1] = newEntry;
            contacts = newContacts;
        }

        static void EditContact()
        {
            int contactRowTochange;
            do
            {
                contactRowTochange = SearchContact();
            }while( contactRowTochange < 0 );

            Console.Write("Please, enter new name: ");
            string newName = Console.ReadLine().Trim();
            newName = newName != "" ? newName : contacts[contactRowTochange].name;

            Console.Write("Please, enter new phone number (starting from 380): ");
            string newPhoneNumber = FormatPhoneNumber(Console.ReadLine());

            Console.Write("Please, enter new date of birth (dd.mm.yyyy format): ");
            DateTime newDate;
            try
            {
                newDate = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("You made a mistake during date input, we'll set up placeholder date so that you can change it via edit function");
            }
            finally
            {
                newDate = DateTime.ParseExact("01.01.2000", "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            }

            var newEntry = (newName, newPhoneNumber, newDate);
            contacts[contactRowTochange] = newEntry;
        }

        static int SearchContact()
        {
            if(contacts.Length == 0)
            {
                Console.WriteLine("There are no contacts, go add a new one");
                AddNewContact();
            }

            int rowNumber;
            string userInput;

            Console.Write("Enter the name or phone number you're looking for:");
            userInput = Console.ReadLine().Trim();
            for (rowNumber = 0; rowNumber < contacts.Length; rowNumber++)
            {
                if (contacts[rowNumber].name.Contains(userInput, StringComparison.OrdinalIgnoreCase) || contacts[rowNumber].phone.Contains(userInput))
                {
                    var contact = contacts[rowNumber];
                    WriteOneContactToConsole(contact);
                    return rowNumber;
                }
            }
            return -1;
        }

        static void SortContacts()
        {
            for (int i = 0; i < contacts.Length - 1; i++)
            {
                int minIdx = i;
                for(int j = i + 1; j < contacts.Length; j++)
                {
                    if (String.Compare(contacts[j].name, contacts[minIdx].name, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        minIdx = j;
                    }
                }
                (contacts[i], contacts[minIdx]) = (contacts[minIdx], contacts[i]);
            }
            Console.WriteLine("Here's sorted list of contacts");
            WriteAllContactsToConsole();
        }

        static void WriteOneContactToConsole((string name, string phone, DateTime date) contact)
        {
            int age = DateTime.Now.Year - contact.date.Year;
            Console.WriteLine($"Name: {contact.name}, Phone: {contact.phone}, Age: {age}");
        }
        static void WriteAllContactsToConsole()
        {
            for (int i = 0; i < contacts.Length; i++)
            {
                int age = DateTime.Now.Year - contacts[i].birth.Year;
                Console.WriteLine($"#{i + 1}: Name: {contacts[i].Item1}, Phone: {contacts[i].Item2}, Age: {age}");
            }
        }

        static (string name, string phone, DateTime date)[] ConvertStringsToContacts(string[] records)
        {
            var contacts = new (string name, string phone, DateTime date)[records.Length];
            for (int i = 0; i < records.Length; ++i)
            {
                string[] array = records[i].Split(','); // "Oleksii", "+38090873928", "30.03.1993"
                if (array.Length != 3)
                {
                    Console.WriteLine($"Line #{i + 1}: '{records[i]}' cannot be parsed");
                    continue;
                }
                contacts[i].name = array[0];
                contacts[i].phone = FormatPhoneNumber(array[1]);
                contacts[i].date = DateTime.Parse(array[2]);
            }
            return contacts;
        }

        static void SaveContactsToFile()
        {
            string[] lines = new string[contacts.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = $"{contacts[i].Item1},{contacts[i].Item2},{contacts[i].Item3}";
            }
            File.WriteAllLines(database, lines);
        }

        static string[] ReadDatabaseAllTextLines(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
            }
            return File.ReadAllLines(file);
        }

        static string FormatPhoneNumber(string phoneNumber)
        {
            if(!phoneNumber.StartsWith('+'))
            {
                phoneNumber = "+" + phoneNumber;
            }

            string formattedPattern = @"\+*\(\d{3}\)\d{2}-\d{3}-\d{4}";

            if ((phoneNumber.Length > 13 && !Regex.IsMatch(phoneNumber, formattedPattern)) || phoneNumber.Length <13)
            {
                Console.WriteLine("This app supports numbers that fit into 12 numbers, for now have the placeholder number and edit it, if you need");
                phoneNumber = "+380111111111";
            }

            if(Regex.IsMatch(phoneNumber, formattedPattern))
            {
                return phoneNumber;
            }
            string pattern = @"(\d{3})(\d{2})(\d{3})(\d{4})";
            
            string formattedPhoneNumber = Regex.Replace(phoneNumber, pattern, "($1)$2-$3-$4");
            return formattedPhoneNumber;
        }
    }
}