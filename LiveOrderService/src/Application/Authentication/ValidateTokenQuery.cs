using LanguageExt.Common;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Authentication
{
    public record ValidateTokenQuery(string Token, string PersonalKey) : IRequest<Result<string>>;

    public class ValidateTokenQueryHandler(IAuthRepository _repository) : IRequestHandler<ValidateTokenQuery, Result<string>>
    {
        public async Task<Result<string>> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.ValidateTokenAsync(request.Token, request.PersonalKey);

            result.Match(
                token => token,
                error => new Result<string>(error)
            );

            return new Result<string>(new Exception("Internal Server Error"));
        }
    }
}