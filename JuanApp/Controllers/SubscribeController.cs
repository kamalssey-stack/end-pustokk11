using JuanApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Controllers;

public class SubscribeController(ISubscriberService subscribers) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Index(string email)
    {
        var result = await subscribers.subscribeAsync(email);
        return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
    }
}
