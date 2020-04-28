using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.ApiClient;
using TestServer.Models;

namespace TestServer
{
    [Api("test")]
    public interface ITestService
    {
        [Get]
        Task<ClaimModel[]> GetClaims();

        [Get("req-headers")]
        Task<string> GetWithRequiredHeaders(
            [Header("X-Claim-User-Id")] string userId,
            [Header("X-Claim-Account-Id")] string accountId);

        [Get("authorized")]
        Task GetAuthorized();

        [Get("is-in-role")]
        Task<bool> IsInRole([JsonContent]string role);
    }
}
