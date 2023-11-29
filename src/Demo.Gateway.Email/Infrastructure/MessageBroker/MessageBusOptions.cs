﻿using Microsoft.Extensions.Options;

namespace Demo.Gateway.Email.Infrastructure.MessageBroker;

internal sealed class MessageBusOptions
{
    public required string ExchangeName { get; init; }
    public required string QueueName { get; init; }

    internal sealed class Setup(IConfiguration Configuration) : IConfigureOptions<MessageBusOptions>
    {
        public const string SECTION_NAME = "MessageBus";

        private readonly IConfiguration _configuration = Configuration;

        public void Configure(MessageBusOptions options)
            => _configuration.GetSection(SECTION_NAME)
                             .Bind(options);
    }
}