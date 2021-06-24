using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Api.Domain.Aggregators;
using Sample.Api.Domain.Entities;
using Sample.Api.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Api.Application.UserCases.Deposit
{
    [Route("api/v1/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly IMongoBaseRepository<Account> _accountRepository;


        public AccountsController(
            ILogger<AccountsController> logger,
            IMongoBaseRepository<Account> accountRepository)
        {
            _logger = logger;
            _accountRepository = accountRepository;
        }

        /// <summary>
        ///     Deposit to an account
        /// </summary>
        [HttpPost("{accountId}/deposit")]
        [SwaggerResponse(204, "The account amount was deposit made", typeof(Account))]
        [SwaggerResponse(404, "The account not found")]
        [SwaggerResponse(400, "The account not be deposit")]
        [SwaggerResponse(500, "Internal Server Error")]
        public async Task<IActionResult> Deposit(Guid accountId, [FromBody] DepositRequest request)
        {
            var source = new CancellationTokenSource();

            var token = source.Token;

            _logger.LogInformation($"Get Account by {accountId}");

            var account = await _accountRepository.GetByIdAsync(accountId, token);

            if (account is null)
            {
                _logger.LogWarning("Account not found !");
                return NotFound();
            }

            var credit = new Credit(account.Id, request.Amount);

            account.Deposit(credit);

            await _accountRepository.UpdateAsync(account, token);

            _logger.LogInformation($"Deposit made, new amount { account.CurrentBalance }");

            return NoContent();
        }

    }
}