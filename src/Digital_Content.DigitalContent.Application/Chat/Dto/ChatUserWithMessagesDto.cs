using System.Collections.Generic;

namespace Digital_Content.DigitalContent.Chat.Dto
{
    public class ChatUserWithMessagesDto : ChatUserDto
    {
        public List<ChatMessageDto> Messages { get; set; }
    }
}