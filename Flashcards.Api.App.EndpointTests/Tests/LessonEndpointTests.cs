using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class LessonEndpointsTests : BaseEndpointTests<LessonEntity, LessonCreateModel, LessonDetailModel, LessonListModel>, IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly UserMapper UserMapper = new();
        private static readonly CardMapper CardMapper = new(UserMapper);
        private static readonly CardCollectionMapper CollectionMapper = new(CardMapper, UserMapper);
        private static readonly LessonMapper LessonMapper = new(CollectionMapper);

        protected override string EndpointBase => "/api/lesson";
        protected override string EndpointGetAll => "/api/lesson";
        protected override Guid GetEntityId(LessonEntity entity) => entity.Id;
        protected override Guid GetModelId(LessonDetailModel model) => model.Id;
        protected override IList<LessonEntity> GetCollection() => _storage.Lessons;
        protected override LessonCreateModel CreateNewModel() => new() // new model for testing insert
        {
            CardCollectionId = Guid.Parse("b72b54c4-9f35-4e3f-96a8-3fdcbbbc09ee"),
            Start = DateTime.UtcNow,
            Seen = 3,
            Correct = 2,
            Wrong = 0
        };
        protected override LessonDetailModel GetUpdatedModel(LessonEntity entity)
            => LessonMapper.ToDetailModel(entity); // the update test is not run for Lesson entity
        protected override bool VerifyUpdated(LessonDetailModel updatedModel) => true;

        [Fact]
        public override Task Put_OnExistingEntity_UpdatesExistingEntity()
        {
            // Ignore this test since Lesson does not have an Update endpoint
            Assert.True(true);
            return Task.CompletedTask;
        }
    }
}
