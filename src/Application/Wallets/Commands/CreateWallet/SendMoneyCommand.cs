using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Wallets.Commands
{
    public class SendMoneyCommand : IRequest<string>
    {
        public Guid SenderUserId { get; set; }
        public Guid ReceiverUserId { get; set; }
        public decimal Amount { get; set; }

        public class SendMoneyCommandHandler : IRequestHandler<SendMoneyCommand, string>
        {
            private readonly IApplicationDbContext _context;

            public SendMoneyCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(SendMoneyCommand request, CancellationToken cancellationToken)
            {
                if (request.Amount <= 0)
                {
                    throw new Exception("Amount must be greater than zero.");
                }

                var senderWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserID == request.SenderUserId, cancellationToken);

                var receiverWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserID == request.ReceiverUserId, cancellationToken);

                if (senderWallet == null)
                    throw new Exception("Sender's wallet not found.");

                if (receiverWallet == null)
                    throw new Exception("Receiver's wallet not found.");

                if (senderWallet.Balance < request.Amount)
                    throw new Exception("Insufficient balance.");

                // Deduct from sender
                senderWallet.Balance -= request.Amount;
                senderWallet.UpdatedDate = DateTime.Now;
                _context.Wallets.Update(senderWallet);

                // Add to receiver
                receiverWallet.Balance += request.Amount;
                receiverWallet.UpdatedDate = DateTime.Now;
                _context.Wallets.Update(receiverWallet);

                await _context.SaveChangesAsync(cancellationToken);

                return $"Sent {request.Amount:C} to {request.ReceiverUserId}. New Sender Balance: {senderWallet.Balance:C}";
            }
        }
    }
}
