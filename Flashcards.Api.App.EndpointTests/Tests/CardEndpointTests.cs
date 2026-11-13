using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class CardEndpointTests : BaseEndpointTests<CardEntity, CardCreateModel, CardDetailModel, CardListModel>, IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly CardMapper CardMapper = new(new UserMapper());

        protected override string EndpointBase => "/api/card";
        protected override string EndpointGetAll => "/api/card";
        protected override Guid GetEntityId(CardEntity entity) => entity.Id;
        protected override Guid GetModelId(CardDetailModel model) => model.Id;
        protected override IList<CardEntity> GetCollection() => _storage.Cards;
        protected override CardCreateModel CreateNewModel() => new()
        {
            QuestionType = ContentType.Text,
            AnswerType = ContentType.Text,
            QuestionContent = "Kdo je prezident Ceske Republiky?",
            AnswerContent = "Petr Pavel",
            Description = "Bezna otazka"
        };
        protected override CardDetailModel GetUpdatedModel(CardEntity entity)
        {
            var model = CardMapper.ToDetailModel(entity);
            model.QuestionContent = "Jiny Content otazky";
            return model;
        }
        protected override bool VerifyUpdated(CardDetailModel updatedModel)
            => GetCollection().Any(c => c.Id == updatedModel.Id && c.QuestionContent == updatedModel.QuestionContent);
    }
}
