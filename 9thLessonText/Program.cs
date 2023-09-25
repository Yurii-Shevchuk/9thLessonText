using System;
using System.Text;

namespace Lesson_9_Text_And_Files
{

    class UserInteraction
    {
        public ContactBook _contactBook;
        public UserInteraction(ContactBook contactBook)
        {
            _contactBook = contactBook;
        }

        static Person ReadPersonFromConsole()
        {
            Console.Write("Please, enter your first name: ");
            string firstName = Console.ReadLine().Trim();

            Console.Write("Please, enter your last name: ");
            string lastName = Console.ReadLine().Trim();

            Console.Write("Please, enter your phone number (starting from 380): ");
            string phoneNumber = ContactBook.FormatPhoneNumber(Console.ReadLine());

            Console.Write("Please, enter your date of birth (dd.mm.yyyy format): ");
            DateTime birthDate = ContactBook.FormatBirthDate();

            return new Person(firstName, lastName, phoneNumber, birthDate);
        }

        public void UserMenu(ref bool isExit)
        {
            Console.WriteLine("1. Write all contacts");
            Console.WriteLine("2. Add new contact");
            Console.WriteLine("3. Edit contact");
            Console.WriteLine("4. Search by name or phone number");
            Console.WriteLine("5. Save");
            Console.WriteLine("6. Sort by name");
            Console.WriteLine("7. Delete a contact");
            Console.WriteLine("8. Exit the program");
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
                    var allContacts = _contactBook.GetAllContacts();
                    for (int i = 0; i < allContacts.Length; i++)
                    {
                        int age = DateTime.Now.Year - allContacts[i].BirthDate.Year;
                        Console.WriteLine($"#{i + 1}: First name: {allContacts[i].FirstName}, Last Name: {allContacts[i].LastName} Phone: {allContacts[i].Phone}, Age: {age}");
                    }
                    break;
                case 2:
                    _contactBook.AddNewContact(ReadPersonFromConsole());
                    break;
                case 3:
                    Console.Write("Enter name to edit: ");
                    string nameToEdit = Console.ReadLine();
                    Person newData = ReadPersonFromConsole();

                    _contactBook.EditContact(nameToEdit, newData);
                    break;
                    break;
                case 4:
                    Console.Write("Enter query: ");
                    string searchQuery = Console.ReadLine();
                    try
                    {
                        var foundContact = _contactBook.SearchContactByQuery(searchQuery);
                    Console.WriteLine($"First name: {foundContact.FirstName}, Last Name: {foundContact.LastName} Phone: {foundContact.Phone}, Age: {DateTime.Now.Year - foundContact.BirthDate.Year}");
                    }
                    catch
                    {
                        Console.WriteLine("No such contact, try again");
                    }
                    break;
                case 5:
                    _contactBook.SaveContactsToFile();
                    break;
                case 6:
                    _contactBook.SortContacts();
                    break;
                case 7:
                    Console.Write("Enter name or last name to delete: ");
                    string deleteQuery = Console.ReadLine();
                    _contactBook.DeleteContactByQuery(deleteQuery);
                    break;
                case 8:
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
        static ContactBook _contactBook;
        static void Main(string[] args)
        {
            bool isExit = false;
            _contactBook = ContactBook.ReadContactBookFromFile(args.Length == 0 ? "db.csv" : args[0]);
            UserInteraction userInteraction = new UserInteraction(_contactBook);
            while(!isExit)
            {
            userInteraction.UserMenu(ref isExit);
            }
            Console.WriteLine("Bye-bye!");
        }       
    }
}