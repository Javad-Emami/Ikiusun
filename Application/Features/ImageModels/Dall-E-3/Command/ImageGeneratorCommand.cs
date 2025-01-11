using Application.Features.ImageModels.Dall_E_3.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Application.Features.ImageModels.Dall_E_3.Query;

public class ImageGeneratorCommand : IRequest<ImageResponseDto>
{
    public ImageGeneratorCommand(ImageRequestDto data, string mobile)
    {
        Data = data;
        Mobile = mobile;
    }
    public ImageRequestDto Data { get; }
    public string Mobile { get; }
}

public class ImageGeneratorQueryHandler : IRequestHandler<ImageGeneratorCommand, ImageResponseDto>
{
    private readonly IOpenAI_ImageModelDalle3 _openAI_ImageModel;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ISqlDbContext _appDbContext;
    private readonly IUserRequestService _userRequestService;
    private readonly IWalletService _walletService;
    private readonly ICostCalculationService _costCalculationService;
    public ImageGeneratorQueryHandler(IOpenAI_ImageModelDalle3 openAI_ImageModel, IConversationService conversationService,
                                      IMessageService messageService, IMapper mapper, IUserService userService,
                                      ISqlDbContext appDbContext, IUserRequestService userRequestService, IWalletService walletService, ICostCalculationService costCalculationService)
    {
        _openAI_ImageModel = openAI_ImageModel;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _appDbContext = appDbContext;
        _userRequestService = userRequestService;
        _walletService = walletService;
        _costCalculationService = costCalculationService;
    }
    public async Task<ImageResponseDto> Handle(ImageGeneratorCommand request, CancellationToken cancellationToken)
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
                var newMessage = new Message(conversation.Id, request.Data.ImagePrompt, (int)SenderTypeEnum.user)
                {                   
                    SequenceNumber = messages.Select(s => s.SequenceNumber).LastOrDefault() + 1,
                };

                messagesList.Add(newMessage);
                //await _messageService.AddAsync(newMessage);

                var openAIResult = await _openAI_ImageModel.GenerateImegeAsync(request.Data);


                var costDto = await _costCalculationService.ImageModelCostCalculationAsync(ServiceModelEnum.dalle3,
                                                                                                   (int)openAIResult.ImageResolution,
                                                                                                   (int)openAIResult.ImageQuality,
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

                var newUserRequest = new UserRequest(conversation.UserId, conversation.Id,null,null, costDto.CostUsage,costDto.PricingId);

                conversation.AddUserRequest(newUserRequest);

                var newAssistantResult = new Message(conversation.Id, openAIResult.ImageBase64, (int)SenderTypeEnum.assistant)
                {
                    SequenceNumber = newMessage.SequenceNumber + 1
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
                var newConversation = new Domain.Entites.Conversation(user.Id, (int)ServiceModelEnum.dalle3);
                var messagesList = new List<Message>();

                var newMessage = new Message(newConversation.Id, request.Data.ImagePrompt, (int)SenderTypeEnum.user)
                {
                    SequenceNumber = 1
                };
                messagesList.Add(newMessage);

                if (request.Data.ImageSize == (int)ImageResolutionEnum.W256xH256 || request.Data.ImageSize == (int)ImageResolutionEnum.W512xH512)
                    throw new CustomException(500, "سایز عکس با مدل سازگار نیست");

                var openAIResult = await _openAI_ImageModel.GenerateImegeAsync(request.Data);

                openAIResult.ConversationId = newConversation.Id;

                var costDto = await _costCalculationService.ImageModelCostCalculationAsync(ServiceModelEnum.dalle3,
                                                                                        (int)openAIResult.ImageResolution,
                                                                                        (int)openAIResult.ImageQuality,
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
                var newUserRequest = new UserRequest(user.Id, newConversation.Id, null, null, costDto.CostUsage, costDto.PricingId);                

                newConversation.AddUserRequest(newUserRequest);

                var newAssistantResult = new Message(newConversation.Id, openAIResult.ImageBase64, (int)SenderTypeEnum.assistant)
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
            throw new CustomException(500, "Dall-E-3 occured an exception!" + "=>" + ex.Message);
        }

    }
}
