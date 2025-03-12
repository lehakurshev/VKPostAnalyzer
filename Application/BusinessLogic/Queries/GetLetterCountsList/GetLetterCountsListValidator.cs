using FluentValidation;

namespace Application.BusinessLogic.Queries.GetLetterCountsList;

public class GetLetterCountsListValidator : AbstractValidator<GetLetterCountsListQuery>
{
    public GetLetterCountsListValidator()
    {
        RuleFor(command => command.AccessToken)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]+$").WithMessage("AccessToken должен содержать только английские буквы и цифры.");

        RuleFor(command => command.UserId)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]+$").WithMessage("UserId должен содержать только английские буквы и цифры.");
    }
}