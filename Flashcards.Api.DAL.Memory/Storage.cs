using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Memory
{
    public class Storage : IStorage
    {
        public IList<CardEntity> Cards { get; } = new List<CardEntity>();
        public IList<UserEntity> Users { get; } = new List<UserEntity>();
        public IList<CardCollectionEntity> Collections { get; } = new List<CardCollectionEntity>();
        public IList<LessonEntity> Lessons { get; } = new List<LessonEntity>();

        public Storage(bool createWithDummyData = true)
        {
            if (createWithDummyData)
            {
                Seed();
            }
        }

        public void Clear()
        {
            Cards.Clear();
            Users.Clear();
            Collections.Clear();
            Lessons.Clear();
        }

        public void Seed()
        {
            Clear();

            // Create Users 
            var adminJakub=  AddUser("Jakub Kovařík", "Jakub", RoleType.Admin, false, "fb4049db-148d-44c2-9be4-5abeffe971a2");
            var adminFilip=  AddUser("Filip Matějka", "User",  RoleType.Admin, false, "253837c3-cc2e-4e03-80d4-95aaf7be98c2"); 
            var userTomas =  AddUser("Tomáš Vrba",    "Tomas", RoleType.User); 
            var userDavid =  AddUser("David Kolařík", "David", RoleType.User);
            var userPetr  =  AddUser("Petr Novák",    string.Empty,  RoleType.User, isDeleted: true); // deleted user

            // --- 1: World Capitals (Geography) ---
            var cGeo1 = AddCard("Capital of France?", "Paris", "Geography");
            var cGeo2 = AddCard("Capital of Germany?", "Berlin", "Geography");
            var cGeo3 = AddCard("Capital of Japan?", "Tokyo", "Geography");
            var cGeo4 = AddCard("Capital of Italy?", "Rome", "Geography");
            var cGeo5 = AddCard("Capital of Spain?", "Madrid", "Geography");
            var cGeo6 = AddCard("Capital of UK?", "London", "Geography");
            var cGeo7 = AddCard("Capital of USA?", "Washington D.C.", "Geography");
            var capitalsDeck =
                AddCollection("World Capitals", adminJakub, null, null, cGeo1, cGeo2, cGeo3, cGeo4, cGeo5, cGeo6, cGeo7);

            // --- 2: Basic Math (Math) ---
            var cMath1 = AddCard("5 + 5 =", "10", "Math");
            var cMath2 = AddCard("10 - 3 =", "7", "Math");
            var cMath3 = AddCard("6 * 6 =", "36", "Math");
            var cMath4 = AddCard("20 / 4 =", "5", "Math");
            var cMath5 = AddCard("Square root of 81?", "9", "Math");
            var cMath6 = AddCard("2 + 2 * 2 =", "6", "Math"); // Order of operations check
            var mathDeck =
                AddCollection("Quick Math", adminJakub, null, null, cMath1, cMath2, cMath3, cMath4, cMath5, cMath6);

            // --- 3: Tech Trivia (Coding) ---
            var cCode1 = AddCard("HTML stands for?", "HyperText Markup Language", "IT");
            var cCode2 = AddCard("C# creator?", "Microsoft", "IT");
            var cCode3 = AddCard("Standard port for HTTP?", "80", "IT");
            var cCode4 = AddCard("Type for True/False?", "Boolean", "IT");
            var cCode5 = AddCard("Loop that runs at least once?", "Do-While", "IT");
            var cCode6 = AddCard("SQL command to fetch data?", "SELECT", "IT");
            var cCode7 = AddCard("First index of array?", "0", "IT");
            var cCode8 = AddCard("Language of the web?", "JavaScript", "IT");
            AddCollection("Tech Trivia", adminFilip, null, null, cCode1, cCode2, cCode3, cCode4, cCode5, cCode6, cCode7, cCode8);

            // --- 4: Simple Science (Science) ---
            var cSci1 = AddCard("H2O is?", "Water", "Chemistry");
            var cSci2 = AddCard("Planet we live on?", "Earth", "Astronomy");
            var cSci3 = AddCard("Freezing point of water (C)?", "0", "Physics");
            var cSci4 = AddCard("Center of atom?", "Nucleus", "Physics");
            var cSci5 = AddCard("Speed of light?", "299,792,458 m/s", "Physics");
            AddCollection("Science 101", userTomas, null, null, cSci1, cSci2, cSci3, cSci4, cSci5);

            // --- 5. Spanish Basics (Language) ---
            var cSpa1 = AddCard("Hello", "Hola", "Spanish");
            var cSpa2 = AddCard("Thank you", "Gracias", "Spanish");
            var cSpa3 = AddCard("Please", "Por favor", "Spanish");
            var cSpa4 = AddCard("Good morning", "Buenos días", "Spanish");
            var cSpa5 = AddCard("Friend", "Amigo", "Spanish");
            var cSpa6 = AddCard("Beer", "Cerveza", "Spanish");
            var cSpa7 = AddCard("One, Two, Three", "Uno, Dos, Tres", "Spanish");
            var colSpanish = AddCollection("Spanish Basics", userPetr, null, null, cSpa1, cSpa2, cSpa3, cSpa4, cSpa5, cSpa6, cSpa7);

            // --- 6. Animal Kingdom (Nature) ---
            var cAni1 = AddCard("Fastest land animal?", "Cheetah", "Nature");
            var cAni2 = AddCard("Largest mammal?", "Blue Whale", "Nature");
            var cAni3 = AddCard("King of the jungle?", "Lion", "Nature");
            var cAni4 = AddCard("Bird that can't fly?", "Penguin", "Nature");
            var cAni5 = AddCard("How many legs does a spider have?", "8", "Nature");
            var cAni6 = AddCard("Only flying mammal?", "Bat", "Nature");
            var colAnimals = AddCollection("Animal Kingdom", userDavid, null, null, cAni1, cAni2, cAni3, cAni4, cAni5, cAni6);

            // --- 7. Movie Quotes (Pop Culture) ---
            var cMov1 = AddCard("I'll be back", "Terminator", "Movies");
            var cMov2 = AddCard("May the Force be with you", "Star Wars", "Movies");
            var cMov3 = AddCard("Here's Johnny!", "The Shining", "Movies");
            var cMov4 = AddCard("I'm going to make him an offer...", "The Godfather", "Movies");
            var cMov5 = AddCard("You shall not pass!", "Lord of the Rings", "Movies");
            var cMov6 = AddCard("Houston, we have a problem", "Apollo 13", "Movies");
            var colMovies = AddCollection(
                "Famous Movie Quotes", 
                userDavid,
                DateTime.Parse("2027-10-01"),
                DateTime.Parse("2027-10-31"),
                cMov1, cMov2, cMov3, cMov4, cMov5, cMov6);

            // --- 8. Music Theory (Arts) ---
            var cMus1 = AddCard("Number of lines in a staff?", "5", "Music");
            var cMus2 = AddCard("Symbol for silence?", "Rest", "Music");
            var cMus3 = AddCard("Do Re Mi Fa ...", "Sol", "Music");
            var cMus4 = AddCard("Lowest male voice?", "Bass", "Music");
            var cMus5 = AddCard("Highest female voice?", "Soprano", "Music");
            var colMusic = AddCollection(
                "Music Theory", 
                userDavid, 
                DateTime.Parse("2025-10-01"), 
                DateTime.Parse("2025-10-31"), 
                cMus1, cMus2, cMus3, cMus4, cMus5);
            
            // Create Lessons
            AddLesson(adminJakub, capitalsDeck, seen: 3, correct: 2, wrong: 1);
            AddLesson(adminJakub, capitalsDeck, seen: 6, correct: 5, wrong: 1);
            AddLesson(adminJakub, capitalsDeck, seen: 7, correct: 7, wrong: 0);
            AddLesson(adminJakub, mathDeck,     seen: 6, correct: 5, wrong: 1);
            AddLesson(adminJakub, mathDeck,     seen: 6, correct: 6, wrong: 0);

            AddLesson(adminFilip, mathDeck, seen: 6, correct: 1, wrong: 5);
            AddLesson(adminFilip, mathDeck, seen: 5, correct: 2, wrong: 3);
            AddLesson(adminFilip, mathDeck, seen: 6, correct: 5, wrong: 1);
        }

        private UserEntity AddUser(string displayName, string identityName, RoleType role, bool isDeleted = false, string? specificId = null)
        {
            var user = new UserEntity
            {
                Id = specificId != null ? Guid.Parse(specificId) : Guid.NewGuid(),
                Name = displayName,
                IdentityOwnerId = identityName,
                Role = role,
                IsDeleted = isDeleted,
                PhotoUrl = $"https://i.pravatar.cc/150?u={displayName}"
            };
            Users.Add(user);
            return user;
        }

        private CardEntity AddCard(string question, string answer, string description)
        {
            var card = new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = question,
                AnswerContent = answer,
                Description = description,
                Created = DateTime.UtcNow
            };
            Cards.Add(card);
            return card;
        }

        private CardCollectionEntity AddCollection(string title, UserEntity owner, DateTime? start = null, DateTime? end = null, params CardEntity[] cards)
        {
            var collection = new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = title,
                OwnerId = owner.Id, // Linking ID automatically
                Owner = owner,
                AnswerStart = start ?? DateTime.UtcNow.AddDays(-60),
                AnswerEnd = end ?? DateTime.UtcNow.AddDays(60),
                Cards = cards.ToList() // Linking Objects automatically
            };
            Collections.Add(collection);
            foreach (var card in cards)
            {
                card.Owner = owner;
                card.OwnerId = owner.Id;
            }
            return collection;
        }

        private LessonEntity AddLesson(UserEntity user, CardCollectionEntity collection, uint seen, uint correct, uint wrong)
        {
            var lesson = new LessonEntity
            {
                Id = Guid.NewGuid(),
                User = user,
                CardCollectionId = collection.Id,
                CardCollection = collection,
                Start = DateTime.UtcNow,
                Seen = seen,
                Correct = correct,
                Wrong = wrong
            };
            Lessons.Add(lesson);
            return lesson;
        }
    }
}