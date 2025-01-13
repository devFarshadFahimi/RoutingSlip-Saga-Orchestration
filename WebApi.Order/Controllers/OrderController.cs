using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedContracts.Events;

namespace WebApi.Order.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OrderController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> FireEvent([FromServices] IBus publishEndpoint)
    {
        try
        {
            var settleRequestReceivedEvent = new SettleRequestReceivedEvent()
            {
                Amount = 194.59m,
                TrackingCode = Guid.NewGuid()
            };

            await publishEndpoint.Publish(settleRequestReceivedEvent);
            return Ok(settleRequestReceivedEvent);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
