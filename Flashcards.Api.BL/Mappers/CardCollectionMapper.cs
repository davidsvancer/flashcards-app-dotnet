using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Mappers;
public class CardCollectionMapper(CardMapper cardMapper, UserMapper userMapper)
{
    public CardCollectionEntity ToEntity(CardCollectionDetailModel model, UserEntity owner)
    {
        return new CardCollectionEntity
        {
            Id = model.Id,
            Title = model.Title,
            AnswerStart = model.AnswerStart,
            AnswerEnd = model.AnswerEnd,
            Cards = model.Cards            
                .Select(card => cardMapper.ToEntity(card, owner))
                .ToList(),
            OwnerId = owner.Id,
            Owner = owner
        };
    }

    public CardCollectionEntity ToEntity(CardCollectionCreateModel model, UserEntity owner)
    {
        return new CardCollectionEntity
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            AnswerStart = model.AnswerStart,
            AnswerEnd = model.AnswerEnd,
            Cards = model.Cards
                .Select(c => cardMapper.ToEntity(c, owner))
                .ToList(),
            OwnerId = owner.Id,
            Owner = owner
        };
    }

    public CardCollectionCreateModel ToCreateModel(CardCollectionDetailModel model)
    {
        return new CardCollectionCreateModel()
        {
            Title = model.Title,
            AnswerStart = model.AnswerStart,
            AnswerEnd = model.AnswerEnd,
            Cards = model.Cards
                .Select(cardMapper.ToCreateModel)
                .ToList(),
        };
    }

    public CardCollectionDetailModel ToDetailModel(CardCollectionEntity entity)
    {
        return new CardCollectionDetailModel
        {
            Id = entity.Id,
            Title = entity.Title,
            AnswerStart = entity.AnswerStart,
            AnswerEnd = entity.AnswerEnd,
            Cards = entity.Cards
                .Select(cardMapper.ToDetailModel)
                .ToList(),
            Owner = entity.Owner is not null ?
                userMapper.ToDetailModel(entity.Owner) :
                null
        };
    }

    public List<CardCollectionListModel> ToListModel(IEnumerable<CardCollectionEntity> entities)
    {
        return entities.Select(e => new CardCollectionListModel
        {
            Id = e.Id,
            Title = e.Title,
            AnswerStart = e.AnswerStart,
            AnswerEnd = e.AnswerEnd,
            Owner = e.Owner is not null ?
                userMapper.ToListModelSingle(e.Owner) :
                null,
            CardCount = e.Cards.Count
        }).ToList();
    }
}