using System.Text;
using System.Text.RegularExpressions;

namespace Lesson_9_Text_And_Files
{
    internal class Program
    {
        static string database = "./../../../db.csv";
        static (string name, string phone, DateTime birth)[] contacts;

        static void Main(string[] args)
        {
            // string absolute_path_to_file = @"C:\Users\alexl\source\repos\C_Sharp_107\Lesson_9_Text_And_Files\document.txt";
            // string relative_path_to_file = @"./../../../document_2.txt";

            // string text = File.ReadAllText(absolute_path_to_file, Encoding.UTF8);
            // string[] lines = File.ReadAllLines(relative_path_to_file);

            // 0. SAVE IT TO THE FILE WITH ".CSV"
            // 1. Writes to console currently available contacts in the file
            // 2. Add new contact
            // 3. Edit contact
            // 4. Search contacts
            // 5. Calculates the contact age
            // 6. Save database

            string[] records = ReadDatabaseAllTextLines(database);
            contacts = ConvertStringsToContacts(records);

            while (true)
            {
                UserInteraction();
            }
        }

        static void UserInteraction()
        {
            Console.WriteLine("1. Write all contacts");
            Console.WriteLine("2. Add new contact");
            Console.WriteLine("3. Edit contact");
            Console.WriteLine("4. Search by row number"); //TODO search by name
            Console.WriteLine("6. Save");

            int input = int.Parse(Console.ReadLine());
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
                case 6:
                    SaveContactsToFile();
                    break;
                default:
                    Console.WriteLine("No such operation.");
                    break;
            }
        }

        static void AddNewContact()
        {
            Console.Write("Please, enter your name: ");
            string newName = Console.ReadLine();
            Console.Write("Please, enter your phone number (starting from 380): ");
            string newPhoneNumber = FormatPhoneNumber(Console.ReadLine());
            Console.Write("Please, enter your date of birth (dd.mm.yyyy format): ");
            DateTime newDate = DateTime.Parse(Console.ReadLine());
            var newEntry = (newName, newPhoneNumber, newDate);

            var newContacts = new (string, string, DateTime)[contacts.Length + 1];
            Array.Copy(contacts, newContacts, contacts.Length);
            newContacts[newContacts.Length - 1] = newEntry;
            contacts = newContacts;
        }

        static void EditContact()
        {
            int contactRowTochange = SearchContact();
            Console.Write("Please, enter new name: ");
            string newName = Console.ReadLine();
            Console.Write("Please, enter new phone number (starting from 380): ");
            string newPhoneNumber = FormatPhoneNumber(Console.ReadLine());
            Console.Write("Please, enter new date of birth (dd.mm.yyyy format): ");
            DateTime newDate = DateTime.Parse(Console.ReadLine());
            var newEntry = (newName, newPhoneNumber, newDate);
            contacts[contactRowTochange] = newEntry;
        }

        static int SearchContact()
        {
            int rowNumber;
            Console.Write("Enter the number of row you're looking for:");
            do
            {
                rowNumber = int.Parse(Console.ReadLine());
            } while (rowNumber < contacts.Length);
            var contact = contacts[rowNumber-1];
            WriteOneContactToConsole(contact);
            return rowNumber-1;
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
            // records:
            // "name,phone,date of birth"
            // Oleksii,+38090873928,30.03.1993
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

        static string FormatPhoneNumber(string phoneNumber )
        {
            if(!phoneNumber.StartsWith('+'))
            {
                phoneNumber = "+" + phoneNumber;
            }
            string pattern = @"(\d{3})(\d{2})(\d{3})(\d{4})";

            string formattedPhoneNumber = Regex.Replace(phoneNumber, pattern, "($1)$2-$3-$4");
            return formattedPhoneNumber;
        }
    }
}