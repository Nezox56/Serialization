using System.Text.Json;
using System.Text.Json.Serialization;

namespace App
{
    internal class Program
    {
        public class Person
        {
            [JsonPropertyName ("id")]
            public Int32 Id { get; set; }

            [JsonPropertyName("transportId")]
            public Guid TransportId { get; set; }

            [JsonPropertyName("firstName")]
            public String FirstName { get; set; }

            [JsonPropertyName("lastName")]
            public String LastName { get; set; }

            [JsonPropertyName("sequenceId")]
            public Int32 SequenceId { get; set; }

            [JsonPropertyName("creditCardNumbers")]
            public String[] CreditCardNumbers { get; set; }

            [JsonPropertyName("age")]
            public Int32 Age { get; set; }

            [JsonPropertyName("phones")]
            public String[] Phones { get; set; }

            [JsonPropertyName("birthDate")]
            public Int64 BirthDate { get; set; }

            [JsonPropertyName("salary")]
            public Double Salary { get; set; }

            [JsonPropertyName("isMarred")]
            public Boolean IsMarred { get; set; }

            [JsonPropertyName("gender")]
            public Gender Gender { get; set; }

            [JsonPropertyName("children")]
            public Child[] Children { get; set; }
        }
        public class Child
        {
            [JsonPropertyName("id")]
            public Int32 Id { get; set; }

            [JsonPropertyName("firstName")]
            public String FirstName { get; set; }

            [JsonPropertyName("lastName")]
            public String LastName { get; set; }

            [JsonPropertyName("birthDate")]
            public Int64 BirthDate { get; set; }

            [JsonPropertyName("gender")]
            public Gender Gender { get; set; }
        }
        public enum Gender
        {
            Male,
            Female
        }

        private static void Main(string[] args)
        {
            List<Person> person = new List<Person>();
            Person[] personMas;

            person = GeneratePersonList(10000);

            var desctopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var file = "Persons.json";
            string filePath = $@"{desctopPath}\{file}";

            DoSerializeJson(person, filePath);

            person.Clear();

            personMas = DoDeserializeJson(filePath);

            Console.WriteLine($"Persons count is {personMas.Count()}");
            Console.WriteLine($"Credit cards count is {GetCreditCardsCount(personMas)}");
            Console.WriteLine($"Average children age is {GetChildAverage(personMas)}");
        }

        private static List<Person> GeneratePersonList(int count)
        {
            Random random = new Random();

            List<Person> people = new List<Person>();

            for (int i = 1; i <= count; i++)
            {
                people.Add(new Person
                {
                    Id = i,
                    TransportId = Guid.NewGuid(),
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    SequenceId = i,
                    CreditCardNumbers = new[] { $"{GetRandomCreditCardNumbers(random)}", $"{GetRandomCreditCardNumbers(random)}" },
                    Age = random.Next(18, 100),
                    Phones = new[] { $"{GetRandomPhone(random)}", $"{GetRandomPhone(random)}" },
                    BirthDate = GetRandomBirthDate(7000, 35000, random),  //7к-19лет 35к-96лет
                    Salary = random.NextDouble(),
                    IsMarred = random.Next(2) == 0 ? false : true,
                    Gender = random.Next(2) == 0 ? Gender.Male : Gender.Female,
                    Children = new[] {new Child
                    {
                        Id = i,
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        BirthDate = GetRandomBirthDate(0, 5000, random),
                        Gender =  random.Next(2) == 0 ? Gender.Male : Gender.Female
                    }
                }
                });
            }

            return people;
        }

        public static long GetRandomBirthDate(int min, int max, Random random)
        {
            DateTime day = DateTime.Today.AddDays(-random.Next(min, max));

            long unixTime = ((DateTimeOffset)day).ToUnixTimeSeconds();

            return unixTime;
        }

        public static string GetRandomPhone(Random random)
        {
            return random.NextInt64(79000000000, 79999999999).ToString();
        }

        public static long GetRandomCreditCardNumbers(Random random)
        {
            return random.NextInt64(1000000000000000, 9999999999999999);
        }

        public static void DoSerializeJson(List<Person> person, string filePath)
        {
            using (StreamWriter fwriter = new StreamWriter(filePath))
            {
                var options = new JsonSerializerOptions { WriteIndented = true};
                fwriter.WriteLine(System.Text.Json.JsonSerializer.Serialize(person, options));
            }
        }

        public static Person[] DoDeserializeJson( string filePath)
        {
            Person[] mas;

            using (FileStream freader = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                mas = JsonSerializer.Deserialize<Person[]>(freader);
                return mas;
            }
        }

        public static long GetChildAverage(Person[] personTwo)
        {
            long ageAvg, sum = 0;

            foreach (Person p in personTwo)
            {
                if (p.Children != null)
                {
                    foreach (Child child in p.Children)
                    {
                        sum += child.BirthDate;
                    }
                }
            }

            ageAvg = sum / personTwo.Length;

            return ageAvg;
        }

        public static int GetCreditCardsCount(Person[] personTwo)
        {
            int count = 0;

            foreach (Person p in personTwo)
            {
                if (p.CreditCardNumbers != null)
                {
                    count += p.CreditCardNumbers.Length;
                }
            }

            return count;
        }
    }
}