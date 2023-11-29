using Demo.Api.Users.Domain;
using Demo.Api.Users.DTOs;
using Demo.Api.Users.Infrastructure.Database;
using MediatR;

namespace Demo.Api.Users.UseCases;

public sealed record CreateUserCommand(UserRequest Request) : IRequest<Guid>
{
    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = User.Create(
                command.Request.Name,
                command.Request.Email,
                command.Request.Phone);

            await _repository.AddAsync(user, cancellationToken);

            return user.Id;
        }
    }
}
