using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EthereumBalance.POCOs;
using EthereumBalance.Services;
using EthereumBalance.Constants;
using EthereumBalance.Extensions;
using EthereumBalance.POCOs.Responses;

namespace EthereumBalance.Controllers
{
    [Route("api/balance")]
    public class BalanceController : Controller
    {
        private BalanceService balanceService;

        public BalanceController(BalanceService balanceService)
        {
            this.balanceService = balanceService;
        }

        [HttpGet]
        public async Task<GenericAPIResponse<CheckBalanceResult>> CheckBalance([FromQuery] string address, [FromQuery] long timestamp)
        {
            // Checks input
            if (string.IsNullOrEmpty(address) || timestamp <= 0)
                return new GenericAPIResponse<CheckBalanceResult>(null, ResponseCode.InvalidInput);
            byte[] addressBytes = address.ToBytes();
            if (addressBytes.Length != 20)
                return new GenericAPIResponse<CheckBalanceResult>(null, ResponseCode.InvalidInput);

            var result = await balanceService.CheckBalance(addressBytes, timestamp);
            if (result == null)
                return new GenericAPIResponse<CheckBalanceResult>(null, ResponseCode.InternalError);

            return new GenericAPIResponse<CheckBalanceResult>(result, ResponseCode.Success);
        }
    }
}
