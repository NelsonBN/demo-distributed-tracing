using Demo.Api.Users.Domain;
using Demo.Api.Users.DTOs;
using Demo.Api.Users.Infrastructure.Database;
using MediatR;

namespace Demo.Api.Users.UseCases;

public sealed record GetUserQuery(Guid Id) : IRequest<UserResponse>
{
    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<GetUserQuery, UserResponse>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task<UserResponse> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            var user = await _repository.GetAsync(query.Id, cancellationToken);
            if(user is null)
            {
                throw new UserNotFoundException(query.Id);
            }

            UserResponse result = user;

            return result;
        }
    }
}
