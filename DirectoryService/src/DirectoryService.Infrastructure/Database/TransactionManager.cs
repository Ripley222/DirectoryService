using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions.Database;
using Shared.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.Database;

public class TransactionManager(
    DirectoryServiceDbContext dbContext,
    ILogger<TransactionManager> logger,
    ILoggerFactory loggerFactory) : ITransactionManager
{
    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var transactionScopeLogger = loggerFactory.CreateLogger<TransactionScope>();
            
            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), transactionScopeLogger);

            return transactionScope;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to begin transaction");
            return Error.Failure("database", "Failed to begin transaction");
        }
    }
    
    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save changes");
            return Error.Failure("database", "Failed to save changes");
        }
    }
}