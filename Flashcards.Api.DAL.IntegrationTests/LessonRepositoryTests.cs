using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class LessonRepositoryTests
    {
        private readonly IFakeDatabase _fakeDb;
        private readonly ILessonRepository _repo;
        
        public  LessonRepositoryTests()
        {
            _fakeDb = new InMemoryFakeDatabase();
            _repo = _fakeDb.GetLessonRepository();
        }

        private bool AreLessonsEqual(LessonEntity left, LessonEntity right)
        {
            return left.Id == right.Id &&
                   left.User.Id == right.User.Id &&
                   left.CardCollection.Id == right.CardCollection.Id &&
                   left.Start == right.Start &&
                   left.Seen == right.Seen &&
                   left.Correct == right.Correct &&
                   left.Wrong == right.Wrong;
        }

        [Fact]
        public void GetById_Existing_Lesson()
        {
            // Arrange
            var requestedId = _fakeDb.LessonGuids[0];

            // Act
            var lesson = _repo.GetById(requestedId);

            // Assert
            Assert.NotNull(lesson);
            Assert.Equal(requestedId, lesson.Id);

            var expectedLesson = _fakeDb.GetLessonById(requestedId);
            Assert.True(AreLessonsEqual(lesson, expectedLesson));
        }

        [Fact]
        public void GetById_NonExisting_Lesson()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var lesson = _repo.GetById(nonExistingId);

            // Assert
            Assert.Null(lesson);
        }

        [Fact]
        public void GetAll_Returns_All_Lessons()
        {
            // Act
            var lessons = _repo.GetAll();

            // Assert
            Assert.Equal(_fakeDb.LessonGuids.Count, lessons.Count);
            foreach (var l in lessons)
            {
                var expected = _fakeDb.GetLessonById(l.Id);
                Assert.True(AreLessonsEqual(l, expected));
            }
        }

        [Fact]
        public void Insert_New_Lesson()
        {
            // Arrange
            var newLesson = new LessonEntity
            {
                Id = Guid.NewGuid(),
                User = _fakeDb.GetUserById(_fakeDb.UserGuids[0]),
                CardCollection = _fakeDb.GetCardCollectionById(_fakeDb.CardCollectionGuids[0]),
                Start = new DateTime(2025, 3, 1, 10, 0, 0),
                Seen = 1,
                Correct = 1,
                Wrong = 0
            };

            // Act
            var newId = _repo.Insert(newLesson);

            // Assert
            var inserted = _fakeDb.GetLessonById(newId);
            Assert.NotNull(inserted);
            Assert.True(AreLessonsEqual(inserted, newLesson));
        }

        [Fact]
        public void Update_Existing_Lesson()
        {
            // Arrange
            var lessonId = _fakeDb.LessonGuids[0];
            var stored = _repo.GetById(lessonId);
            Assert.NotNull(stored);

            var updatePayload = new LessonEntity
            {
                Id = stored.Id,
                User = stored.User,
                CardCollection = stored.CardCollection,
                Start = stored.Start,
                Seen = 10,
                Correct = 8,
                Wrong = stored.Wrong
            };

            // Act
            _repo.Update(updatePayload);

            // Assert
            var updated = _fakeDb.GetLessonById(lessonId);
            Assert.Equal(10u, updated.Seen);
            Assert.Equal(8u, updated.Correct);

            var expected = _fakeDb.GetLessonById(lessonId);
            Assert.True(AreLessonsEqual(updated, expected));
        }

        [Fact]
        public void Delete_Existing_Lesson()
        {
            // Arrange
            var idToRemove = _fakeDb.LessonGuids[0];

            // Act
            _repo.Remove(idToRemove);

            // Assert
            Assert.Null(_fakeDb.GetLessonById(idToRemove));
        }
    }
}
