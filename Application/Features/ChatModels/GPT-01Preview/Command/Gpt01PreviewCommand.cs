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
    private readonly ISqlDbContext _appDbContext;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IOpenAi_ChatGPT01Preview _openAi_ChatGPT01Preview;
    private readonly IUserService _userService;
    private readonly IWalletService _walletService;
    public Gpt01PreviewCommandHandler(ISqlDbContext appDbContext, IConversationService conversationService, IMessageService messageService, IMapper mapper, IOpenAi_ChatGPT01Preview openAi_ChatGPT01Preview, IUserService userService, IWalletService walletService)
    {
        _appDbContext = appDbContext;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _openAi_ChatGPT01Preview = openAi_ChatGPT01Preview;
        _userService = userService;
        _walletService = walletService;
    }
    public async Task<Gpt01PreviewResponseDto> Handle(Gpt01PreviewCommand request, CancellationToken cancellationToken)
    {
        var hasEnoughValue = await _walletService.HasMinumumBalanceValueForImageModelAsync(request.Mobile, cancellationToken);
        if (!hasEnoughValue)
            throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");

        await using var transaction = await _appDbContext.datbase.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.Data.Id != null)
            {
                var conversation = await _conversationService.GetAsync((Guid)request.Data.Id);
                var messagesList = new List<Message>();
                var messages = await _messageService.BaseQuery.Where(m => m.ConversationId == conversation.Id)
                                                              .OrderBy(s => s.SequenceNumber)
                                                              .ToListAsync(cancellationToken);
                var newMessage = new Message(conversation.Id, request.Data.Text, (int)SenderTypeEnum.user)
                {
                    SequenceNumber = messages.Select(s => s.SequenceNumber).LastOrDefault() + 1,
                };

                messagesList.Add(newMessage);

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

                var newUserRequest = new UserRequest(conversation.UserId, conversation.Id, 
                                                    (int)ServiceModelEnum.gpt01Preview,
                                                    openAIResult.InputToken, openAIResult.OutputToken)
                {
                    //Cost = 
                };
                conversation.AddUserRequest(newUserRequest);

                var newAssistantResult = new Message(conversation.Id, openAIResult.Content, (int)SenderTypeEnum.assistant)
                {
                    SequenceNumber = newMessage.SequenceNumber + 1,
                };
                messagesList.Add(newAssistantResult);

                conversation.AddNewMessage(messagesList);

                await _conversationService.UpdateAsync(conversation, cancellationToken);

                await transaction.CommitAsync();
                return openAIResult;
            } 
            else
            {
                var user = await _userService.GetAsync(u => u.Mobile == request.Mobile);
                var newConversation = new Domain.Entites.Conversation(user.Id, (int)ServiceModelEnum.gpt01Preview);

                var messagesList = new List<Message>();
                var newMessage = new Message(newConversation.Id, request.Data.Text, (int)SenderTypeEnum.user)
                {
                    SequenceNumber = 1
                };
                messagesList.Add(newMessage);

                var openAIResult = await _openAi_ChatGPT01Preview.GetChatCompletionAsync(request.Data.Text);

                openAIResult.ConversationId = newConversation.Id;

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest(user.Id, newConversation.Id, 
                                                    (int)ServiceModelEnum.gpt01Preview, 
                                                    openAIResult.InputToken, openAIResult.OutputToken)
                {                    
                    //Cost = 
                };

                newConversation.AddUserRequest(newUserRequest);

                var newAssistantResult = new Message(newConversation.Id, openAIResult.Content, (int)SenderTypeEnum.assistant)
                {                    
                    SequenceNumber = newMessage.SequenceNumber + 1,
                };
                messagesList.Add(newAssistantResult);

                newConversation.AddNewMessage(messagesList);

                await _conversationService.AddAsync(newConversation, cancellationToken);

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