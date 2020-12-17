﻿using MediatR;
using Reddnet.Application.Interfaces;
using Reddnet.Application.Validation;
using Reddnet.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reddnet.Application.Community.Commands
{
    public record CreateCommunityCommand : IRequest<Result<CommunityEntity>>
    {
        public Guid UserId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public byte[] Image { get; init; }
    }

    internal class CreateSubredditHandler : IRequestHandler<CreateCommunityCommand, Result<CommunityEntity>>
    {
        private readonly IDataContext context;

        public CreateSubredditHandler(IDataContext context)
            => this.context = context;

        public async Task<Result<CommunityEntity>> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
        {
            var community = this.context.Communities.Add(new CommunityEntity
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name,
                Description = request.Description,
                Image = request.Image
            });

            await this.context.SaveChangesAsync(cancellationToken);
            return Result<CommunityEntity>.Ok(community.Entity);
        }
    }
}
