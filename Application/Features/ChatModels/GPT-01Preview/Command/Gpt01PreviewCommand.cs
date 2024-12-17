using Application.Features.ChatModels.GPT_01Preview.Dto;
using Application.Interfaces;
using Application.Interfaces.Gpt_01Preview;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ChatModels.GPT_01Preview.Command;

public class Gpt01PreviewCommand: IRequest<Gpt01PreviewResponseDto>
{
    public Gpt01PreviewCommand(Gpt01PreviewRequestDto data, string mobile)
    {
        Data = data;
        Mobile = mobile;
    }
    public Gpt01PreviewRequestDto Data { get; }
    public string Mobile { get; }   
}

public class Gpt01PreviewCommandHandler : IRequestHandler<Gpt01PreviewCommand, Gpt01PreviewResponseDto>
{
    private readonly IAppDbContext _appDbContext;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IOpenAi_ChatGPT01Preview _openAi_ChatGPT01Preview;
    private readonly IUserService _userService;
    public Gpt01PreviewCommandHandler(IAppDbContext appDbContext, IConversationService conversationService, IMessageService messageService, IMapper mapper, IOpenAi_ChatGPT01Preview openAi_ChatGPT01Preview, IUserService userService)
    {
        _appDbContext = appDbContext;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _openAi_ChatGPT01Preview = openAi_ChatGPT01Preview;
        _userService = userService;
    }
    public async Task<Gpt01PreviewResponseDto> Handle(Gpt01PreviewCommand request, CancellationToken cancellationToken)
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

                var MessagesDtoList = new List<ChatGpt01PreviewMessagesDto>();

                foreach (var item in messages)
                {
                    var dto = new ChatGpt01PreviewMessagesDto()
                    {
                        Content = item.Content,
                        SenderTypeId = item.SenderType,
                    };
                    MessagesDtoList.Add(dto);
                }
                MessagesDtoList.Add(_mapper.Map<ChatGpt01PreviewMessagesDto>(newMessage));

                var openAIResult = await _openAi_ChatGPT01Preview.GetChatCompletionAsync(MessagesDtoList);
                openAIResult.ConversationId = conversation.Id;

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = conversation.Id,
                    UserId = conversation.UserId,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.gpt01Preview,
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
                var mobile = request.Mobile;
                var user = await _userService.GetAsync(u => u.Mobile == mobile);
                var newConversation = new Domain.Entites.Conversation()
                {
                    UserId = user.Id,
                    ServiceModelId = (int)ServiceModelEnum.gpt01Preview,
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

                var openAIResult = await _openAi_ChatGPT01Preview.GetChatCompletionAsync(request.Data.Text);

                openAIResult.ConversationId = newConversation.Id;

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = newConversation.Id,
                    UserId = user.Id,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.gpt01Preview,
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
            throw new CustomException(500, "Gpt-01 Preview occured an exception!" + "=>" + ex.Message);
        }
    }
}