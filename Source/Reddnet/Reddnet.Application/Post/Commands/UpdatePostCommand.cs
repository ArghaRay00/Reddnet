﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Reddnet.Application.Interfaces;
using Reddnet.Application.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reddnet.Application.Post.Commands
{
    public record UpdatePostCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; init; }
        #pragma warning disable CS8632
        public byte[]? Image { get; init; }
        public string? Title { get; init; }
        public string? Content { get; init; }
        #pragma warning restore CS8632
    }

    internal class UpdatePostHandler : IRequestHandler<UpdatePostCommand, Result<Guid>>
    {
        private readonly IDataContext context;
        private readonly ICurrentUserAccessor userAccessor;

        public UpdatePostHandler(IDataContext context, ICurrentUserAccessor userAccessor)
        {
            this.context = context;
            this.userAccessor = userAccessor;
        }

        public async Task<Result<Guid>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await this.context.Posts
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (post == null || (userAccessor.IsAuthenticated && post.UserId != userAccessor.Id))
            {
                return Result<Guid>.Failed("Not found");
            }

            if (request.Image != null)
            {
                post.Image = request?.Image;
            }

            if (request.Title != null)
            {
                post.Title = request?.Title;
            }

            if (request.Content != null)
            {
                post.Content = request?.Content;
            }

            await this.context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Ok(post.Id);
        }
    }
}
