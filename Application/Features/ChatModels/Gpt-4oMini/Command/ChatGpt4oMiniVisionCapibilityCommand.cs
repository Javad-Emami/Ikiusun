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
    public ChatGpt4oMiniVisionCapibilityCommand(Gpt4oMiniRequestDto data, string mobile)
    {
        Data = data;
        Mobile = mobile;
    }
    public Gpt4oMiniRequestDto Data { get; }
    public string Mobile { get; }
}

public class ChatGpt4oMiniVisionCapibilityCommandHandler : IRequestHandler<ChatGpt4oMiniVisionCapibilityCommand, Gpt4oMiniResponseDto>
{
    private readonly IOpenAI_ChatGPT4oMiniVisionCapability _openAi_ChatGpt4oMiniVision;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ISqlDbContext _appDbContext;
    private readonly IWalletService _walletService;
    private readonly ICostCalculationService _costCalculationService;
    public ChatGpt4oMiniVisionCapibilityCommandHandler(IOpenAI_ChatGPT4oMiniVisionCapability openAi_ChatGpt4oMiniVision,
                                                       IConversationService conversationService, IMessageService messageService,
                                                       IMapper mapper, IUserService userService, ISqlDbContext appDbContext, IWalletService walletService, ICostCalculationService costCalculationService)
    {
        _openAi_ChatGpt4oMiniVision = openAi_ChatGpt4oMiniVision;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _appDbContext = appDbContext;
        _walletService = walletService;
        _costCalculationService = costCalculationService;
    }
    public async Task<Gpt4oMiniResponseDto> Handle(ChatGpt4oMiniVisionCapibilityCommand request, CancellationToken cancellationToken)
    {
        var hasEnoughValue = await _walletService.HasMinumumBalanceValueForChatModelAsync(request.Mobile, cancellationToken);
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

                var openAIResult = await _openAi_ChatGpt4oMiniVision.GetChatCompletionAsync(MessagesDtoList);
                openAIResult.ConversationId = conversation.Id;

                var costDto = await _costCalculationService.ChatModelCostCalculationAsync(ServiceModelEnum.gpt4omini,
                                                                                                   openAIResult.InputToken,
                                                                                                   openAIResult.OutputToken,
                                                                                                   cancellationToken);
                var userWallet = await _walletService.BaseQuery
                                            .Where(w => w.UserId == conversation.UserId && w.TransactionTime >= DateTime.Now.AddDays(-10))
                                            .OrderByDescending(tt => tt.TransactionTime)
                                            .FirstOrDefaultAsync(cancellationToken) ??
                                 await _walletService.BaseQuery
                                            .Where(w => w.UserId == conversation.UserId)
                                            .OrderByDescending(tt => tt.TransactionTime)
                                            .FirstOrDefaultAsync(cancellationToken);

                var updatedUserWallet = new WalletTransaction(conversation.UserId, (int)TransactionType.Withdrawl, costDto.CostUsage, userWallet.BalanceAmount);
                await _walletService.AddAsync(updatedUserWallet, cancellationToken);

                var newUserRequest = new UserRequest(conversation.UserId, conversation.Id,                                                    
                                                     openAIResult.InputToken, openAIResult.OutputToken, costDto.CostUsage,costDto.PricingId);

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
                var newConversation = new Domain.Entites.Conversation(user.Id,(int)ServiceModelEnum.gpt4omini);

                var messagesList = new List<Message>();
                var newMessage = new Message(newConversation.Id, request.Data.Text, (int)SenderTypeEnum.user)
                {
                    SequenceNumber = 1
                };

                messagesList.Add(newMessage);

                var openAIResult = request.Data.Images != null ?
                    await _openAi_ChatGpt4oMiniVision.GetChatCompletionWithVisionAsync(request.Data.Images, request.Data.Text) :
                    await _openAi_ChatGpt4oMiniVision.GetChatCompletionAsync(request.Data.Text);

                openAIResult.ConversationId = newConversation.Id;

                var costDto = await _costCalculationService.ChatModelCostCalculationAsync(ServiceModelEnum.gpt4omini,
                                                                                                  openAIResult.InputToken,
                                                                                                  openAIResult.OutputToken,
                                                                                                  cancellationToken);
                var userWallet = await _walletService.BaseQuery
                                            .Where(w => w.UserId == user.Id && w.TransactionTime >= DateTime.Now.AddDays(-10))
                                            .OrderByDescending(tt => tt.TransactionTime)
                                            .FirstOrDefaultAsync(cancellationToken) ??
                                await _walletService.BaseQuery
                                            .Where(w => w.UserId == user.Id)
                                            .OrderByDescending(tt => tt.TransactionTime)
                                            .FirstOrDefaultAsync(cancellationToken);

                var updatedUserWallet = new WalletTransaction(user.Id, (int)TransactionType.Withdrawl, costDto.CostUsage, userWallet.BalanceAmount);
                await _walletService.AddAsync(updatedUserWallet, cancellationToken);
                var newUserRequest = new UserRequest(user.Id, newConversation.Id,                                                   
                                                    openAIResult.InputToken, openAIResult.OutputToken,costDto.CostUsage,costDto.PricingId);

                newConversation.AddUserRequest(newUserRequest);

                var newAssistantResult = new Message(newConversation.Id, openAIResult.Content, (int)SenderTypeEnum.assistant)
                {
                    SequenceNumber = newMessage.SequenceNumber + 1,
                };
                messagesList.Add(newAssistantResult);

                newConversation.AddNewMessage(messagesList);

                await _conversationService.AddAsync(newConversation,cancellationToken);

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

