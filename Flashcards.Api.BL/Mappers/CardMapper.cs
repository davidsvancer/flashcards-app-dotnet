namespace Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;

public class CardMapper(UserMapper userMapper)
{
    public CardEntity ToEntity(CardCreateModel model, UserEntity owner)
    {
        return new CardEntity
        {
            Id = Guid.NewGuid(),
            QuestionType = model.QuestionType,
            AnswerType = model.AnswerType,
            QuestionContent = model.QuestionContent,
            AnswerContent = model.AnswerContent,
            Created = DateTime.UtcNow,
            OwnerId = owner.Id,
            Owner = owner,
            Description = model.Description
        };
    }
    public CardEntity ToEntity(CardDetailModel model, UserEntity owner)
    {
        return new CardEntity
        {
            Id = model.Id,
            QuestionType = model.QuestionType,
            AnswerType = model.AnswerType,
            QuestionContent = model.QuestionContent,
            AnswerContent = model.AnswerContent,
            Created = model.DateCreated,
            OwnerId = owner.Id,
            Owner = owner,
            Description = model.Description
        };
    }

    public CardCreateModel ToCreateModel(CardDetailModel model)
    {
        return new CardCreateModel
        {
            QuestionType = model.QuestionType,
            AnswerType = model.AnswerType,
            QuestionContent = model.QuestionContent,
            AnswerContent = model.AnswerContent,
            Description = model.Description
        };
    }

    public CardDetailModel ToDetailModel(CardEntity entity)
    {
        return new CardDetailModel
        {
            Id = entity.Id,
            QuestionType = entity.QuestionType,
            AnswerType = entity.AnswerType,
            QuestionContent = entity.QuestionContent,
            AnswerContent = entity.AnswerContent,
            DateCreated = entity.Created,
            Description = entity.Description,
            Owner = entity.Owner is not null ? 
                userMapper.ToListModelSingle(entity.Owner) : 
                null
        };
    }

    public List<CardListModel> ToListModel(IEnumerable<CardEntity> entities)
    {
        return entities.Select(e => new CardListModel
        {
            Id = e.Id,
            QuestionContent = e.QuestionContent,
            Description = e.Description,
            QuestionType = e.QuestionType,
            AnswerType = e.AnswerType,
            OwnerName = e.Owner?.Name,
        }).ToList();
    }
}