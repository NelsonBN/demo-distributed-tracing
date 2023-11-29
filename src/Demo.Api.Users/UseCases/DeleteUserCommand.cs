using Demo.Api.Users.Domain;
using Demo.Api.Users.Infrastructure.Database;
using MediatR;

namespace Demo.Api.Users.UseCases;

public sealed record DeleteUserCommand(Guid Id) : IRequest
{
    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            if(!await _repository.AnyAsync(command.Id, cancellationToken))
            {
                throw new UserNotFoundException(command.Id);
            }

            await _repository.DeleteAsync(command.Id, cancellationToken);
        }
    }
}
