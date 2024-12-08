using Application.Features.ChatModels.GPT_4o.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ChatModels.GPT_4o.Command;

public class GPT4oVisionCapabilityCommand:IRequest<Gpt4oResponseDto>
{
    public GPT4oVisionCapabilityCommand(Gpt4oRequestDto data)
    {
        Data = data;
    }
    public Gpt4oRequestDto Data { get; }
}

public class GPT4oVisionCapabilityCommandHandler : IRequestHandler<GPT4oVisionCapabilityCommand, Gpt4oResponseDto>
{
    private readonly IOpenAI_ChatGPT4oVisionCapability _openAi_ChatGpt4oVision;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IAppDbContext _appDbContext;
    public GPT4oVisionCapabilityCommandHandler(IOpenAI_ChatGPT4oVisionCapability openAi_ChatGpt4oVision, IConversationService conversationService, 
                                               IMessageService messageService, IMapper mapper, IUserService userService, IAppDbContext appDbContext)
    {
        _openAi_ChatGpt4oVision = openAi_ChatGpt4oVision;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _appDbContext = appDbContext;
    }
    public async Task<Gpt4oResponseDto> Handle(GPT4oVisionCapabilityCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _appDbContext.datbase.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.Data.Id != null)
            {
                var conversation = await _conversationService.GetAsync(c => c!.Id == request.Data.Id);

                var messages = await _messageService.BaseQuery.Where(m => m.ConversationId == conversation!.Id)
                                                              .OrderBy(s => s.SequenceNumber)
                                                              .ToListAsync(cancellationToken);
                var newMessage = new Message()
                {
                    ConversationId = conversation!.Id,
                    SenderType = (int)SenderTypeEnum.user,
                    Content = request.Data.Text,
                    CreationDate = DateTime.Now,
                    SequenceNumber = messages.Select(s => s.SequenceNumber).LastOrDefault() + 1,
                };
                await _messageService.AddAsync(newMessage);

                var MessagesDtoList = new List<ChatGpt4oMessagesDto>();

                foreach (var item in messages)
                {
                    var dto = new ChatGpt4oMessagesDto()
                    {
                        Content = item.Content,
                        SenderTypeId = item.SenderType,
                    };
                    MessagesDtoList.Add(dto);
                }

                MessagesDtoList.Add(_mapper.Map<ChatGpt4oMessagesDto>(newMessage));

                var openAIResult = await _openAi_ChatGpt4oVision.GetChatCompletion(MessagesDtoList);
                openAIResult.ConversationId = conversation.Id;

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = conversation.Id,
                    UserId = conversation.UserId,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.gpt4o,
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
                    await _openAi_ChatGpt4oVision.GetChatCompletionWithVision(request.Data.Images,request.Data.Text) :
                    await _openAi_ChatGpt4oVision.GetChatCompletion(request.Data.Text);

                openAIResult.ConversationId = newConversation.Id;

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = newConversation.Id,
                    UserId = user.Id,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.gpt4o,
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
            throw new CustomException(500, "Gpt-4o occured an exception!" + "=>" + ex.Message);
        }
    }
}
