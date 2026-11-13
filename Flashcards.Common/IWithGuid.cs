using System;
namespace Flashcards.Common
{
    public interface IWithGuid
    {
        Guid Id { get; init; }
    }
}
