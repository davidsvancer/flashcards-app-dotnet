using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class CardCollectionEndpointsTests : BaseEndpointTests<CardCollectionEntity, CardCollectionCreateModel, CardCollectionDetailModel, CardCollectionListModel>, IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly UserMapper UserMapper = new();
        private static readonly CardCollectionMapper CollectionMapper = new(new CardMapper(UserMapper), UserMapper);

        protected override string EndpointBase => "/api/cardCollection";
        protected override string EndpointGetAll => "/api/cardCollection";
        protected override Guid GetEntityId(CardCollectionEntity entity) => entity.Id;
        protected override Guid GetModelId(CardCollectionDetailModel model) => model.Id;
        protected override IList<CardCollectionEntity> GetCollection() => _storage.Collections;
        protected override CardCollectionCreateModel CreateNewModel() => new() // new model for testing insert
        {
            Title = "History Facts",
            AnswerStart = DateTime.UtcNow.AddMinutes(-10),
            AnswerEnd = DateTime.UtcNow.AddMinutes(10),
            Cards = new List<CardCreateModel>()
        };
        protected override CardCollectionDetailModel GetUpdatedModel(CardCollectionEntity entity)
        {
            var model = CollectionMapper.ToDetailModel(entity);
            model.Title = "Jiny Title kolekce karet";
            return model;
        }
        protected override bool VerifyUpdated(CardCollectionDetailModel updatedModel)
            => GetCollection().Any(c => c.Id == updatedModel.Id && c.Title == updatedModel.Title);
    }
}
