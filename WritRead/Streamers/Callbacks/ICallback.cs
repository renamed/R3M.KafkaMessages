using WritRead.Model;

namespace WritRead.Streamers.Callbacks
{
    public interface ICallback
    {
         void ProcessReceivedMessage(MessageModel messageModel);
    }
}