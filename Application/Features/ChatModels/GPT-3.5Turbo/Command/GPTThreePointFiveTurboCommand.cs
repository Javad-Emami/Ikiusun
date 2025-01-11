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
    public GPTThreePointFiveTurboCommand(ChatRequestDto data,string mobile)
    {
        Data = data;
        Mobile = mobile;
    }
    public ChatRequestDto Data { get; }
    public string Mobile { get; }
}

public class GPTThreePointFiveTurboQueryHandler : IRequestHandler<GPTThreePointFiveTurboCommand, ChatResponseDto>
{
    private readonly IOpenAi_ChatGPT3Point5Turbo _openAi_ChatGpt;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ISqlDbContext _appDbContext;
    private readonly IWalletService _walletService;
    private readonly ICostCalculationService _costCalculationService;
    public GPTThreePointFiveTurboQueryHandler(IOpenAi_ChatGPT3Point5Turbo openAi_ChatGpt, IConversationService conversationService,
                                              IMessageService messageService, IMapper mapper, IUserService userService,
                                              ISqlDbContext appDbContext, IWalletService walletService, ICostCalculationService costCalculationService)
    {
        _openAi_ChatGpt = openAi_ChatGpt;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _appDbContext = appDbContext;
        _walletService = walletService;
        _costCalculationService = costCalculationService;
    }
    public async Task<ChatResponseDto> Handle(GPTThreePointFiveTurboCommand request, CancellationToken cancellationToken)
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
                var newMessage = new Message(conversation!.Id,request.Data.Text, (int)SenderTypeEnum.user)
                {
                    SequenceNumber = messages.Select(s => s.SequenceNumber).LastOrDefault() + 1,
                };

                messagesList.Add(newMessage);
                
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

                var openAIResult = await _openAi_ChatGpt.GetChatCompletionAsync(MessagesDtoList);

                openAIResult.ConversationId = conversation.Id;

                var costDto = await _costCalculationService.ChatModelCostCalculationAsync(ServiceModelEnum.GptThreePointFiveTurbo,
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
                                                     openAIResult.InputToken, openAIResult.OutputToken,costDto.CostUsage, costDto.PricingId);

                conversation.AddUserRequest(newUserRequest);

                var newAssistantResult = new Message(conversation.Id, openAIResult.Content, (int)SenderTypeEnum.assistant)
                {                    
                    SequenceNumber = newMessage.SequenceNumber + 1,
                };
                messagesList.Add(newAssistantResult);

                conversation.AddNewMessage(messagesList);

                await _conversationService.UpdateAsync(conversation,cancellationToken);

                await transaction.CommitAsync();
                return openAIResult;
            }
            else
            {
                var user = await _userService.GetAsync(u => u.Mobile == request.Mobile);
                var newConversation = new Domain.Entites.Conversation(user!.Id, (int)ServiceModelEnum.GptThreePointFiveTurbo);

                var messagesList = new List<Message>();
                var newMessage = new Message(newConversation.Id, request.Data.Text, (int)SenderTypeEnum.user)
                {                    
                    SequenceNumber = 1
                };
                messagesList.Add(newMessage);               

                var openAIResult = await _openAi_ChatGpt.GetChatCompletionAsync(request.Data.Text);
                openAIResult.ConversationId = newConversation.Id;

                var costDto = await _costCalculationService.ChatModelCostCalculationAsync(ServiceModelEnum.GptThreePointFiveTurbo,
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
                                                    openAIResult.InputToken, openAIResult.OutputToken,costDto.CostUsage, costDto.PricingId);

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
            throw new CustomException(500, "Gpt-3.5 Turbo occured an exception!" + "=>" + ex.Message);
        }
       
    }
}
