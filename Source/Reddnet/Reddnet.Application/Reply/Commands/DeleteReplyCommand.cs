﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Reddnet.Application.Interfaces;
using Reddnet.Application.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reddnet.Application.Reply.Commands
{
    public record DeleteReplyCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
    }

    internal class DeleteReplyHandler : IRequestHandler<DeleteReplyCommand, Result>
    {
        private readonly IDataContext context;
        private readonly ICurrentUserAccessor userAccessor;

        public DeleteReplyHandler(IDataContext context, ICurrentUserAccessor userAccessor)
        {
            this.context = context;
            this.userAccessor = userAccessor;
        }

        public async Task<Result> Handle(DeleteReplyCommand request, CancellationToken cancellationToken)
        {
            var reply = await this.context.Replies
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (reply is null || (userAccessor.IsAuthenticated && reply.UserId != userAccessor.Id))
            {
                return Result.Failed("Not found");
            }

            this.context.Replies.Remove(reply);
            await this.context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
