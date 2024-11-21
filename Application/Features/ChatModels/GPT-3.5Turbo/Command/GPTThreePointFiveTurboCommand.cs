using Application.Features.ChatModels.GPT_3._5Turbo.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ChatModels.GPT_3._5Turbo.Query;

public class GPTThreePointFiveTurboCommand: IRequest<ChatResponseDto>
{
    public GPTThreePointFiveTurboCommand(ChatRequestDto data)
    {
        Data = data;
    }
    public ChatRequestDto Data { get; }
}

public class GPTThreePointFiveTurboQueryHandler : IRequestHandler<GPTThreePointFiveTurboCommand, ChatResponseDto>
{
    private readonly IOpenAi_ChatModel _openAi_ChatGpt;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IAppDbContext _appDbContext;
    public GPTThreePointFiveTurboQueryHandler(IOpenAi_ChatModel openAi_ChatGpt, IConversationService conversationService,
                                              IMessageService messageService, IMapper mapper, IUserService userService, 
                                              IAppDbContext appDbContext)
    {
        _openAi_ChatGpt = openAi_ChatGpt;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _appDbContext = appDbContext;
    }
    public async Task<ChatResponseDto> Handle(GPTThreePointFiveTurboCommand request, CancellationToken cancellationToken)
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

                var MessagesDtoList = new List<ChatMessagesDto>();

                foreach (var item in messages)
                {
                    var dto = new ChatMessagesDto()
                    {
                        Content = item.Content,
                        SenderTypeId = item.SenderType,
                    };
                    MessagesDtoList.Add(dto);
                }

                MessagesDtoList.Add(_mapper.Map<ChatMessagesDto>(newMessage));

                var openAIResult = await _openAi_ChatGpt.GetChatCompletion(MessagesDtoList);

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = conversation.Id,
                    UserId = conversation.UserId,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.dalle3,
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
                var newConversation = new Conversation()
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

                var openAIResult = await _openAi_ChatGpt.GetChatCompletion(request.Data.Text);

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = newConversation.Id,
                    UserId = user.Id,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.dalle3,
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
            throw new CustomException(500, "Gpt-3.5 Turbo occured an exception!" + "=>" + ex.Message);
        }
       
    }
}
