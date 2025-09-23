using Hybrsoft.DTOs;
using Hybrsoft.FoundationAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hybrsoft.FoundationAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SubscriptionController(ISubscriptionService subscriptionService) : ControllerBase
	{
		private readonly ISubscriptionService _subscriptionService = subscriptionService;

		[Authorize]
		[HttpPost("activate")]
		public async Task<ActionResult<LicenseResponse>> Activate([FromBody] ActivationRequest request)
		{
			var subscription = await _subscriptionService.Activate(request.LicenseKey);
			if (!subscription.IsActivated)
			{
				return BadRequest(new LicenseResponse
				{
					IsActivated = subscription.IsActivated,
					Message = subscription.Message,
					Uid = subscription.Uid
				});
			}
			return Ok(new LicenseResponse
			{
				IsActivated = subscription.IsActivated,
				StartDate = subscription.StartDate,
				ExpirationDate = subscription.ExpirationDate,
				Message = subscription.Message,
				Uid = subscription.Uid
			});
		}

		[Authorize]
		[HttpPost("validate")]
		public async Task<ActionResult<LicenseResponse>> Validate([FromBody] ValidationRequest request)
		{
			var subscription = await _subscriptionService.Validate(request.Email, request.ProductType);
			if (!subscription.IsActivated)
			{
				return BadRequest(new LicenseResponse
				{
					IsActivated = subscription.IsActivated,
					Message = subscription.Message
				});
			}
			return Ok(new LicenseResponse
			{
				IsActivated = subscription.IsActivated,
				StartDate = subscription.StartDate,
				ExpirationDate = subscription.ExpirationDate,
				LicenseData = subscription.LicenseData,
				LicensedTo = subscription.LicensedTo,
				Message = subscription.Message
			});
		}
	}
}
