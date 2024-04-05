using CustomEd.Discussion.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using FluentValidation;

namespace CustomEd.Discussion.Service.DTOs.Validtion;

public class UpdateMessageDtoValidator: AbstractValidator<UpdateMessageDto>
{
    private readonly IGenericRepository<Message> _messageRepositry;
    public UpdateMessageDtoValidator(IGenericRepository<Message> messageRepositry)
    {
        _messageRepositry = messageRepositry;

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters");
       
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("An existing message is required")
            .MustAsync(async (id, token) => {
                var message = await _messageRepositry.GetAsync(id);
                return message != null;
            }).WithMessage("MessageId does not exist");
    }
}
