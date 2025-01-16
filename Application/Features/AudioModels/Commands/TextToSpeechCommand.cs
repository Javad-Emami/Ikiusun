using Application.Features.AudioModels.Dto;
using Application.Interfaces;
using Application.Interfaces.Audio;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections;

namespace Application.Features.AudioModels.Commands;

public class TextToSpeechCommand: IRequest<TextToSpeechResponseDto>
{
    public TextToSpeechCommand(TextToSpeechRequestDto data,string mobile)
    {
        Data = data;
        Mobile = mobile;
    }
    public TextToSpeechRequestDto Data { get; }
    public string Mobile { get; }
}

public class TextToSpeechCommandHandler : IRequestHandler<TextToSpeechCommand, TextToSpeechResponseDto>
{
    private readonly IWalletService _walletService;
    private readonly ISqlDbContext _sqlDbContext;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IOpenAi_AudioModels _openAi_AudioModels;
    private readonly ICostCalculationService _costCalculationService;
    private readonly IUserService _userService;
    public TextToSpeechCommandHandler(IWalletService walletService, IConversationService conversationService, ISqlDbContext sqlDbContext, IMessageService messageService, IOpenAi_AudioModels openAi_AudioModels, ICostCalculationService costCalculationService, IUserService userService)
    {
        _walletService = walletService;
        _conversationService = conversationService;
        _sqlDbContext = sqlDbContext;
        _messageService = messageService;
        _openAi_AudioModels = openAi_AudioModels;
        _costCalculationService = costCalculationService;
        _userService = userService;
    }
    public async Task<TextToSpeechResponseDto> Handle(TextToSpeechCommand request, CancellationToken cancellationToken)
    {
        //TODO: set wallet service for audio model
        var hasEnoughValue = await _walletService.HasMinumumBalanceValueForChatModelAsync(request.Mobile, cancellationToken);
        if (!hasEnoughValue)
            throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");
        await using var transaction = await _sqlDbContext.datbase.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.Data.ModelNameId == 3)
                throw new CustomException(500, "Invalid model for converting text to speech");
            if (request.Data.Id != null)
            {
                var conversation = await _conversationService.GetAsync((Guid)request.Data.Id);
                var messagesList = new List<Message>();
                var messages = await _messageService.BaseQuery.Where(m => m.ConversationId == conversation!.Id)
                                                              .OrderBy(s => s.SequenceNumber)
                                                              .ToListAsync(cancellationToken);
                var newMessage = new Message(conversation!.Id, request.Data.InputText, (int)SenderTypeEnum.user)
                {
                    SequenceNumber = messages.Select(s => s.SequenceNumber).LastOrDefault() + 1,
                };
                messagesList.Add(newMessage);

                var openAIResult = await _openAi_AudioModels.TextToSpeechAsync(request.Data,cancellationToken);

                var audioDuration = await _openAi_AudioModels.AudioLengthDurationCalculator(openAIResult.AudioFile,cancellationToken);

                var costDto = await _costCalculationService.TextToSpeechAsync(ServiceModelEnum.textServices,
                                                                              request.Data.ModelNameId,
                                                                              request.Data.InputText.Length,
                                                                              null, cancellationToken);
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
                                                     null,null,
                                                     costDto.CostUsage, costDto.PricingId);
                conversation.AddUserRequest(newUserRequest);
                var newAssistantResult = new Message(conversation.Id, Convert.ToBase64String(openAIResult.AudioFile), (int)SenderTypeEnum.assistant)
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
                var newConversation = new Domain.Entites.Conversation(user.Id, (int)ServiceModelEnum.textServices);
                var messagesList = new List<Message>();

                var newMessage = new Message(newConversation.Id, request.Data.InputText, (int)SenderTypeEnum.user)
                {
                    SequenceNumber = 1
                };

                messagesList.Add(newMessage);

                var openAIResult = await _openAi_AudioModels.TextToSpeechAsync(request.Data, cancellationToken);
                var audioDuration = await _openAi_AudioModels.AudioLengthDurationCalculator(openAIResult.AudioFile, cancellationToken);

                var costDto = await _costCalculationService.TextToSpeechAsync(ServiceModelEnum.textServices,
                                                                              request.Data.ModelNameId,
                                                                              request.Data.InputText.Length,
                                                                              null, cancellationToken);
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
                                                     null, null,
                                                     costDto.CostUsage, costDto.PricingId);
                newConversation.AddUserRequest(newUserRequest);
                var newAssistantResult = new Message(newConversation.Id, Convert.ToBase64String(openAIResult.AudioFile), (int)SenderTypeEnum.assistant)
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
            throw new CustomException(500, "Audio model occured an exception!" + "=>" + ex.Message); 
        }

    }
}
