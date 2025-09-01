using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Database;

public class TransactionScope(
    IDbTransaction dbTransaction,
    ILogger<TransactionScope> logger) : ITransactionScope
{
    public UnitResult<Error> Commit()
    {
        try
        {
            dbTransaction.Commit();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to commit transaction");
            return UnitResult.Failure(
                Error.Failure("transaction.commit.failed", "Failed to commit transaction"));
        }
    }
    
    public UnitResult<Error> Rollback()
    {
        try
        {
            dbTransaction.Rollback();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rollback transaction");
            return UnitResult.Failure(
                Error.Failure("transaction.rollback.failed", "Failed to rollback transaction"));
        }
    }

    public void Dispose()
    {
        dbTransaction.Dispose();
    }
}