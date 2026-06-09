using Backend.Hubs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IHubContext<LeaderBoardHub> _hubContext;
    
    public GameController(IGameService gameService, IHubContext<LeaderBoardHub> hubContext)
    {
        _gameService = gameService;
        _hubContext = hubContext;
    }

    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost("login")]
    public ActionResult Login([FromBody] AuthReq req) 
    {
        var token = _gameService.Authenticate(req.Username);
        if (token == null) return Unauthorized(new { message = "Player not registered" });
        return Ok(new { token });
    }

    [Authorize] [HttpPost("start")]
    public ActionResult Start() => Ok(new { sessionId = _gameService.StartGame(UserId).Id, leaderboard = _gameService.GetLeaderboard() });

    [Authorize] [HttpPost("move")]
    public async Task<ActionResult> Move([FromBody] MoveReq req)
    {
        var result = _gameService.ProcessMove(req.SessionId, req.Row, UserId);
        if (result.Status == "Won") await _hubContext.Clients.All.SendAsync("UpdateLeaderboard", _gameService.GetLeaderboard());
        return Ok(result);
    }

    [Authorize] [HttpGet("unfinished")] // REST 1
    public ActionResult GetUnfinished() => Ok(_gameService.GetUnfinishedGames(UserId));

    [Authorize] [HttpPut("config/{id}/position")] // REST 2
    public ActionResult UpdateConfig(int id, [FromBody] ConfigUpdateReq req) 
    { 
        _gameService.UpdateConfig(id, req); 
        return Ok(new { message = "Configuration updated successfully" }); 
    }
}
