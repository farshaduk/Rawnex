using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Chat.Commands;
using Rawnex.Application.Features.Chat.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class ChatController : BaseApiController
{
    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateChatConversationCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendChatMessageCommand command)
    {
        if (conversationId != command.ConversationId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("messages/{messageId:guid}")]
    public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] EditChatMessageCommand command)
    {
        if (messageId != command.MessageId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpDelete("messages/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        var result = await Mediator.Send(new DeleteChatMessageCommand(messageId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("conversations/{conversationId:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid conversationId)
    {
        var result = await Mediator.Send(new MarkChatMessagesReadCommand(conversationId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetMyConversationsQuery(page, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var result = await Mediator.Send(new GetConversationMessagesQuery(conversationId, page, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
