using System.Text.RegularExpressions;

namespace Lesson_9_Text_And_Files
{
    class ContactBook
    {
        private const string _unknownPhone = "+380111111111";
        private string _contactBookFile;

        public Person[] _contacts;

        public ContactBook(string file)
        {
            _contactBookFile = file;
            _contacts = new Person[0];
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
                phoneNumber = _unknownPhone;
            }

            if (Regex.IsMatch(phoneNumber, formattedPattern))
            {
                return phoneNumber;
            }
            string pattern = @"(\d{3})(\d{2})(\d{3})(\d{4})";

            string formattedPhoneNumber = Regex.Replace(phoneNumber, pattern, "($1)$2-$3-$4");
            return formattedPhoneNumber;
        }

        public bool AddNewContact(Person person)
        {
            try
            {
            Array.Resize(ref _contacts, _contacts.Length + 1);
            _contacts[^1] = person;
                return true;
            }
            catch 
            {
                return false; 
            }
        }

        public bool EditContact(string query, Person newPerson)
        {
            int contactIndex = GetContactIndexByQuery(query);
           if(contactIndex <0)
            {
                return false;
            }    
            _contacts[contactIndex] = newPerson;
            return true;
        }
        public bool DeleteContactByQuery(string query)
        {
            int contactIndex = GetContactIndexByQuery(query);
            if(contactIndex < 0) 
            {
                return false;
            }
            Person[] contactsCopy = new Person[_contacts.Length];
            Array.Copy(_contacts, contactsCopy, _contacts.Length);

            _contacts = new Person[_contacts.Length - 1];
            Array.Copy(contactsCopy, 0, _contacts, 0, contactIndex);
            Array.Copy(contactsCopy, contactIndex + 1, _contacts, contactIndex, _contacts.Length - contactIndex);
            return true;
        }
        public Person[] GetAllContacts() => _contacts;

        public Person SearchContactByQuery(string query) => GetContactIndexByQuery(query) >=0 ? _contacts[GetContactIndexByQuery(query)] : null;


        private int GetContactIndexByQuery(string searchQuery)
        {

            int rowNumber;
            for (rowNumber = 0; rowNumber < _contacts.Length; rowNumber++)
            {
                if (_contacts[rowNumber].FirstName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || _contacts[rowNumber].LastName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || _contacts[rowNumber].Phone.Contains(searchQuery))
                {
                    return rowNumber;
                }
            }
            return -1;
        }

        public void SortContacts()
        {
            for (int i = 0; i < _contacts.Length - 1; i++)
            {
                int minIdx = i;
                for (int j = i + 1; j < _contacts.Length; j++)
                {
                    if (String.Compare(String.Join(" ", _contacts[j].FirstName, _contacts[j].LastName), String.Join(" ", _contacts[minIdx].FirstName, _contacts[minIdx].LastName), StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        minIdx = j;
                    }
                }
                (_contacts[i], _contacts[minIdx]) = (_contacts[minIdx], _contacts[i]);
            }
        }

        public bool SaveContactsToFile()
        {
            try
            {

            string[] lines = new string[_contacts.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = $"{_contacts[i].FirstName},{_contacts[i].LastName},{_contacts[i].Phone},{_contacts[i].BirthDate}";
            }
            File.WriteAllLines(_contactBookFile, lines);
                return true;
            }
            catch 
            { 
                return false;
            }
        }

        public static DateTime FormatBirthDate()
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
        public static ContactBook ReadContactBookFromFile(string filename)
        {
            string[] lines = ReadDatabaseAllTextLines(filename);
            return new ContactBook(filename)
            {
                _contacts = ConvertStringsToContacts(lines),
            };
        }


        private static Person[] ConvertStringsToContacts(string[] records)
        {
            var contacts = new Person[records.Length];
            for (int i = 0; i < records.Length; ++i)
            {
                string[] array = records[i].Split(','); // "Oleksii", "+38090873928", "30.03.1993"
                if (array.Length != 4)
                {
                    continue;
                }
                contacts[i] = new Person(array[0], array[1], ContactBook.FormatPhoneNumber(array[2]), DateTime.Parse(array[3]));
            }
            return contacts;
        }

        private static string[] ReadDatabaseAllTextLines(string file)
        {
            try
            {
                return File.ReadAllLines(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new string[0];
            }
        }

    }
}