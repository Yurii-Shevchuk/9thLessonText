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

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Phone { get; init; }
        public DateTime BirthDate { get; init; }
    }
}