using Application.Features.ChatModels.Gpt_4oMini.Dto;
using Application.Interfaces;
using Application.Interfaces.Gpt_4oMini;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ChatModels.Gpt_4oMini.Command;

public class ChatGpt4oMiniVisionCapibilityCommand:IRequest<Gpt4oMiniResponseDto>
{
    public ChatGpt4oMiniVisionCapibilityCommand(Gpt4oMiniRequestDto data)
    {
        Data = data;
    }
    public Gpt4oMiniRequestDto Data { get; }
}

public class ChatGpt4oMiniVisionCapibilityCommandHandler : IRequestHandler<ChatGpt4oMiniVisionCapibilityCommand, Gpt4oMiniResponseDto>
{
    private readonly IOpenAI_ChatGPT4oMiniVisionCapability _openAi_ChatGpt4oMiniVision;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IAppDbContext _appDbContext;
    public ChatGpt4oMiniVisionCapibilityCommandHandler(IOpenAI_ChatGPT4oMiniVisionCapability openAi_ChatGpt4oMiniVision, 
                                                       IConversationService conversationService, IMessageService messageService, 
                                                       IMapper mapper, IUserService userService, IAppDbContext appDbContext)
    {
        _openAi_ChatGpt4oMiniVision = openAi_ChatGpt4oMiniVision;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _appDbContext = appDbContext;
    }
    public async Task<Gpt4oMiniResponseDto> Handle(ChatGpt4oMiniVisionCapibilityCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _appDbContext.datbase.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.Data.Id != null)
            {
                var conversation = await _conversationService.GetAsync(c => c.Id == request.Data.Id);

                var messages = await _messageService.BaseQuery.Where(m => m.ConversationId == conversation.Id)
                                                              .OrderBy(s => s.SequenceNumber)
                                                              .ToListAsync(cancellationToken);
                var newMessage = new Message()
                {
                    ConversationId = conversation.Id,
                    SenderType = (int)SenderTypeEnum.user,
                    Content = request.Data.Text,
                    CreationDate = DateTime.Now,
                    SequenceNumber = messages.Select(s => s.SequenceNumber).LastOrDefault() + 1,
                };
                await _messageService.AddAsync(newMessage);

                var MessagesDtoList = new List<ChatGpt4oMiniMessagesDto>();

                foreach (var item in messages)
                {
                    var dto = new ChatGpt4oMiniMessagesDto()
                    {
                        Content = item.Content,
                        SenderTypeId = item.SenderType,
                    };
                    MessagesDtoList.Add(dto);
                }

                MessagesDtoList.Add(_mapper.Map<ChatGpt4oMiniMessagesDto>(newMessage));

                var openAIResult = await _openAi_ChatGpt4oMiniVision.GetChatCompletion(MessagesDtoList);
                openAIResult.ConversationId = conversation.Id;
                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = conversation.Id,
                    UserId = conversation.UserId,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.gpt4omini,
                    InputToken = openAIResult.InputToken,
                    OutputTokent = openAIResult.OutputToken,
                    //Cost = 
                };

                //await _userRequestService.AddAsync(newUserRequest);

                var newAssistantResult = new Message()
                {
                    ConversationId = conversation.Id,
                    SenderType = (int)SenderTypeEnum.assistant,
                    Content = openAIResult.Content,
                    CreationDate = DateTime.Now,
                    SequenceNumber = newMessage.SequenceNumber + 1,
                };
                await _messageService.AddAsync(newAssistantResult);

                await transaction.CommitAsync();
                return openAIResult;
            }
            else
            {
                var mobile = "09024335424";
                var user = await _userService.GetAsync(u => u.Mobile == mobile);
                var newConversation = new Domain.Entites.Conversation()
                {
                    UserId = user.Id,
                    ServiceModelId = (int)ServiceModelEnum.GptThreePointFiveTurbo,
                    CreatedAt = DateTime.Now,
                };
                await _conversationService.AddAsync(newConversation);

                var newMessage = new Message()
                {
                    ConversationId = newConversation.Id,
                    SenderType = (int)SenderTypeEnum.user,
                    Content = request.Data.Text,
                    CreationDate = DateTime.Now,
                    SequenceNumber = 1
                };

                await _messageService.AddAsync(newMessage);


                var openAIResult = request.Data.Images != null ?
                    await _openAi_ChatGpt4oMiniVision.GetChatCompletionWithVision(request.Data.Images, request.Data.Text) :
                    await _openAi_ChatGpt4oMiniVision.GetChatCompletion(request.Data.Text);

                openAIResult.ConversationId = newConversation.Id;

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = newConversation.Id,
                    UserId = user.Id,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.gpt4omini,
                    InputToken = openAIResult.InputToken,
                    OutputTokent = openAIResult.OutputToken,
                    //Cost = 
                };

                //await _userRequestService.AddAsync(newUserRequest);

                var newAssistantResult = new Message()
                {
                    ConversationId = newConversation.Id,
                    SenderType = (int)SenderTypeEnum.assistant,
                    Content = openAIResult.Content,
                    CreationDate = DateTime.Now,
                    SequenceNumber = newMessage.SequenceNumber + 1,
                };
                await _messageService.AddAsync(newAssistantResult);

                await transaction.CommitAsync();
                return openAIResult;
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new CustomException(500, "Gpt-4o-mini occured an exception!" + "=>" + ex.Message);
        }
    }
}

