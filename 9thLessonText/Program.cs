using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Lesson_9_Text_And_Files
{

    class Person
    {
        private const string UnknownFirstName = "John";
        private const string UnknownLastName = "Doe";
        

        public Person(string firstName, string lastName, string phone, DateTime birthDate)
        {
            FirstName = (firstName == null || firstName == "") ? UnknownFirstName : firstName;
            LastName = (lastName == null || lastName == "") ? UnknownLastName : lastName;
            Phone = ContactBook.FormatPhoneNumber(phone);
            BirthDate = birthDate;
        }

        public Person(Person otherPerson, string newFirstName, string newLastName, string newPhone, DateTime? newBirthDate)
        {
            FirstName = (newFirstName == null || newFirstName == "") ? otherPerson.FirstName : newFirstName; // null coalescent operator
            LastName = (newLastName == null || newLastName == "") ? otherPerson.LastName : newLastName;
            Phone = newPhone ?? otherPerson.Phone;
            BirthDate = newBirthDate ?? otherPerson.BirthDate;
        }

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Phone { get; init; }
        public DateTime BirthDate { get; init; }
    }

    class ContactBook
    {
        private const string unknownPhone = "+380111111111";
        private const string database = "./../../../db.csv";
        private string[] records;
        public Person[] contacts;

        public ContactBook()
        {
            records = ReadDatabaseAllTextLines(database);
            contacts = ConvertStringsToContacts(records);
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (!phoneNumber.StartsWith('+'))
            {
                phoneNumber = "+" + phoneNumber;
            }

            string formattedPattern = @"\+{1}\(\d{3}\)\d{2}-\d{3}-\d{4}";

            if ((phoneNumber.Length > 13 && !Regex.IsMatch(phoneNumber, formattedPattern)) || phoneNumber.Length < 13)
            {
                Console.WriteLine("This app supports numbers that fit into 12 numbers, for now have the placeholder number and edit it, if you need");
                phoneNumber = unknownPhone;
            }

            if (Regex.IsMatch(phoneNumber, formattedPattern))
            {
                return phoneNumber;
            }
            string pattern = @"(\d{3})(\d{2})(\d{3})(\d{4})";

            string formattedPhoneNumber = Regex.Replace(phoneNumber, pattern, "($1)$2-$3-$4");
            return formattedPhoneNumber;
        }

        public void AddNewContact()
        {
            Console.Write("Please, enter your first name: ");
            string newFirstName = Console.ReadLine().Trim();

            Console.Write("Please, enter your last name: ");
            string newLastName = Console.ReadLine().Trim();

            Console.Write("Please, enter your phone number (starting from 380): ");
            string newPhoneNumber = ContactBook.FormatPhoneNumber(Console.ReadLine());

            Console.Write("Please, enter your date of birth (dd.mm.yyyy format): ");
            DateTime newDate = FormatBirthDate();
            

            Person person = new Person(newFirstName, newLastName, newPhoneNumber, newDate);

            Array.Resize(ref contacts, contacts.Length + 1);
            contacts[^1] = person;
        }

        public void EditContact()
        {
            int contactRowTochange;
            do
            {
                contactRowTochange = SearchContact();
            } while (contactRowTochange < 0);

            Console.Write("Please, enter your first name: ");
            string newFirstName = Console.ReadLine().Trim();

            Console.Write("Please, enter your last name: ");
            string newLastName = Console.ReadLine().Trim();

            Console.Write("Please, enter your phone number (starting from 380): ");
            string newPhoneNumber = ContactBook.FormatPhoneNumber(Console.ReadLine());

            Console.Write("Please, enter your date of birth (dd.mm.yyyy format): ");
            DateTime newDate = FormatBirthDate();

            Person editedPerson = new Person(contacts[contactRowTochange], newFirstName, newLastName, newPhoneNumber, newDate); 

            contacts[contactRowTochange] = editedPerson;
        }

        public int SearchContact()
        {
            if (contacts.Length == 0)
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
                if (contacts[rowNumber].FirstName.Contains(userInput, StringComparison.OrdinalIgnoreCase) || contacts[rowNumber].LastName.Contains(userInput, StringComparison.OrdinalIgnoreCase) || contacts[rowNumber].Phone.Contains(userInput))
                {
                    var contact = contacts[rowNumber];
                    WriteOneContactToConsole(contact);
                    return rowNumber;
                }
            }
            return -1;
        }

        public void WriteOneContactToConsole(Person contact)
        {
            int age = DateTime.Now.Year - contact.BirthDate.Year;
            Console.WriteLine($"First name: {contact.FirstName}, Last Name: {contact.LastName} Phone: {contact.Phone}, Age: {age}");
        }
        public void WriteAllContactsToConsole()
        {
            for (int i = 0; i < contacts.Length; i++)
            {
                int age = DateTime.Now.Year - contacts[i].BirthDate.Year;
                Console.WriteLine($"#{i + 1}: First name: {contacts[i].FirstName}, Last Name: {contacts[i].LastName} Phone: {contacts[i].Phone}, Age: {age}");
            }
        }

        public void SortContacts()
        {
            for (int i = 0; i < contacts.Length - 1; i++)
            {
                int minIdx = i;
                for (int j = i + 1; j < contacts.Length; j++)
                {
                    if (String.Compare(String.Join(" ", contacts[j].FirstName, contacts[j].LastName), String.Join(" ", contacts[minIdx].FirstName, contacts[minIdx].LastName), StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        minIdx = j;
                    }
                }
                (contacts[i], contacts[minIdx]) = (contacts[minIdx], contacts[i]);
            }
            Console.WriteLine("Here's sorted list of contacts");
            WriteAllContactsToConsole();
        }

        public void SaveContactsToFile()
        {
            string[] lines = new string[contacts.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = $"{contacts[i].FirstName},{contacts[i].LastName},{contacts[i].Phone},{contacts[i].BirthDate}";
            }
            File.WriteAllLines(database, lines);
        }

        private DateTime FormatBirthDate()
        {
            DateTime newDate;
            try
            {
                newDate = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("You made a mistake during date input, we'll set up placeholder date so that you can change it via edit function");
                newDate = DateTime.ParseExact("01.01.2000", "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            }
            return newDate;
        }

        private Person[] ConvertStringsToContacts(string[] records)
        {
            var contacts = new Person[records.Length];
            for (int i = 0; i < records.Length; ++i)
            {
                string[] array = records[i].Split(','); // "Oleksii", "+38090873928", "30.03.1993"
                if (array.Length != 4)
                {
                    Console.WriteLine($"Line #{i + 1}: '{records[i]}' cannot be parsed");
                    continue;
                }
                contacts[i] = new Person(array[0], array[1], ContactBook.FormatPhoneNumber(array[2]), DateTime.Parse(array[3]));
            }
            return contacts;
        }

        private string[] ReadDatabaseAllTextLines(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
            }
            return File.ReadAllLines(file);
        }

    }

    class UserInteraction
    {
        public ContactBook ContactBook;
        public UserInteraction(ContactBook contactBook)
        {
            ContactBook = contactBook;
        }

        public void UserMenu(ref bool isExit)
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
                UserMenu(ref isExit);
                return;
            }
            catch (OverflowException)
            {
                Console.WriteLine("There is no possible way this program could have THIS many functions, try better");
                UserMenu(ref isExit);
                return;
            }
            switch (input)
            {
                case 1:
                    ContactBook.WriteAllContactsToConsole();
                    break;
                case 2:
                    ContactBook.AddNewContact();
                    break;
                case 3:
                    ContactBook.EditContact();
                    break;
                case 4:
                    ContactBook.SearchContact();
                    break;
                case 5:
                    ContactBook.SaveContactsToFile();
                    break;
                case 6:
                    ContactBook.SortContacts();
                    break;
                case 7:
                    isExit = true;
                    return;
                default:
                    Console.WriteLine("No such operation.");
                    break;
            }
        }

    }
    internal static class Program
    {
        static void Main(string[] args)
        {
            bool isExit = false;
            ContactBook book = new ContactBook();
            UserInteraction userInteraction = new UserInteraction(book);
            while(!isExit)
            {
            userInteraction.UserMenu(ref isExit);
            }
            Console.WriteLine("Bye-bye!");
        }       
    }
}