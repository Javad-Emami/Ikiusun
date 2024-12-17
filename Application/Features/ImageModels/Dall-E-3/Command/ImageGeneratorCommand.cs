using Application.Features.ImageModels.Dall_E_3.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ImageModels.Dall_E_3.Query;

public class ImageGeneratorCommand : IRequest<ImageResponseDto>
{
    public ImageGeneratorCommand(ImageRequestDto data)
    {
        Data = data;
    }
    public ImageRequestDto Data { get; }
}

public class ImageGeneratorQueryHandler : IRequestHandler<ImageGeneratorCommand, ImageResponseDto>
{
    private readonly IOpenAI_ImageModelDalle3 _openAI_ImageModel;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IAppDbContext _appDbContext;
    private readonly IUserRequestService _userRequestService;
    public ImageGeneratorQueryHandler(IOpenAI_ImageModelDalle3 openAI_ImageModel, IConversationService conversationService,
                                      IMessageService messageService, IMapper mapper, IUserService userService, 
                                      IAppDbContext appDbContext, IUserRequestService userRequestService)
    {
        _openAI_ImageModel = openAI_ImageModel;
        _conversationService = conversationService;
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
        _appDbContext = appDbContext;
        _userRequestService = userRequestService;
    }
    public async Task<ImageResponseDto> Handle(ImageGeneratorCommand request, CancellationToken cancellationToken)
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
                    Content = request.Data.ImagePrompt,
                    CreationDate = DateTime.Now,
                    SequenceNumber = messages.Select(s => s.SequenceNumber).LastOrDefault() + 1,
                };
                await _messageService.AddAsync(newMessage);

                //var MessagesDtoList = new List<MessagesDto>();

                //foreach (var item in messages)
                //{
                //    var dto = new MessagesDto()
                //    {
                //        Content = item.Content,
                //        SenderTypeId = item.SenderType,
                //    };
                //    MessagesDtoList.Add(dto);
                //}

                //MessagesDtoList.Add(_mapper.Map<MessagesDto>(newMessage));

                var openAIResult = await _openAI_ImageModel.GenerateImegeAsync(request.Data);


                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = conversation.Id,
                    UserId = conversation.UserId,
                    RequestTime = DateTime.Now,
                    ServiceModelId = (int)ServiceModelEnum.dalle3,
                    //Cost = 
                };

                //await _userRequestService.AddAsync(newUserRequest);
                var newAssistantResult = new Message()
                {
                    ConversationId = conversation.Id,
                    SenderType = (int)SenderTypeEnum.assistant,
                    //Content = openAIResult.ImageBase64,
                    CreationDate = DateTime.Now,
                    SequenceNumber = newMessage.SequenceNumber + 1,
                };
                await _messageService.AddAsync(newAssistantResult);

                await transaction.CommitAsync();
                return openAIResult;
            }
            else
            {
                // create new convrsation
                var mobile = "09024335424";
                var user = await _userService.GetAsync(u => u.Mobile == mobile);
                var newConversation = new Domain.Entites.Conversation()
                {
                    UserId = user.Id,
                    ServiceModelId = (int)ServiceModelEnum.dalle3,
                    CreatedAt = DateTime.Now,
                };
                await _conversationService.AddAsync(newConversation);

                var newMessage = new Message()
                {
                    ConversationId = newConversation.Id,
                    SenderType = (int)SenderTypeEnum.user,
                    Content = request.Data.ImagePrompt,
                    CreationDate = DateTime.Now,
                    SequenceNumber = 1
                };

                await _messageService.AddAsync(newMessage);

                if (request.Data.ImageSize == 1 || request.Data.ImageSize == 2)
                    throw new CustomException(500, "سایز عکس با مدل سازگار نیست");

                var openAIResult = await _openAI_ImageModel.GenerateImegeAsync(request.Data);

                openAIResult.ConversationId = newConversation.Id;

                //TODO: Calling Cost calculation Service

                var newUserRequest = new UserRequest()
                {
                    ConversationId = newConversation.Id,
                    UserId = user.Id,
                    RequestTime = DateTime.Now,
                    ServiceModelId= (int)ServiceModelEnum.dalle3,
                    //Cost = 
                };

                //await _userRequestService.AddAsync(newUserRequest);

                var newAssistantResult = new Message()
                {
                    ConversationId = newConversation.Id,
                    SenderType = (int)SenderTypeEnum.assistant,
                    Content = openAIResult.ImageUri.ToString(),
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
            throw new CustomException(500, "Dall-E-3 occured an exception!" + "=>" + ex.Message);
        }

    }
}
