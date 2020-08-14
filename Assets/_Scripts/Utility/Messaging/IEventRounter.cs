using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.Messaging
{
    enum MessageType
    {
        GameModes,
        Encounter,
        Exploration,
        Level,
        Story,
        Appearance
    }

    static class MessagingConstants
    {
        public const int MESSAGING_ID_OFFSET = 100000;    
    }

    abstract class MessageInfoBase
    {
        public int MessageId;
        protected object mArg;

        protected MessageInfoBase(int messageId, object arg = null)
        {
            MessageId = messageId;
            mArg = arg;
        }

        public T Arg<T>()
        {
            T retVal = default;

            if (mArg == null)
            {
                Logger.Fail(LogTag.Messaging, "Arg()", "Attempt to cast null arg");
            }
            else
            {
                T cast = (T)mArg;
                if (cast == null)
                {
                    Logger.Fail(LogTag.Messaging, "Arg()", "Casting arg to T resulted in null");
                }
                else
                {
                    retVal = cast;
                }
            }
            
            return retVal;
        }
    }

    interface IMessageHandler
    {
        void HandleMessage(MessageInfoBase eventInfoBase);
    }

    class MessageRouter : MonoBehaviour
    {
        private HashSet<IMessageHandler> mMessageHandlers = new HashSet<IMessageHandler>();
        private List<MessageInfoBase> mPendingMessages = new List<MessageInfoBase>();

        public static MessageRouter Instance;

        public void RegisterHandler(IMessageHandler listener)
        {
            mMessageHandlers.Add(listener);
        }

        public void UnRegisterHandler(IMessageHandler listener)
        {
            mMessageHandlers.Remove(listener);
        }

        public void NotifyMessage(MessageInfoBase eventInfo, bool async = true)
        {
            if (async)
            {
                mPendingMessages.Add(eventInfo);
            }
            else
            {
                NotifyListeners(eventInfo);
            }
        }

        private void NotifyListeners(MessageInfoBase eventInfo)
        {
            HashSet<IMessageHandler> handlerCopy = new HashSet<IMessageHandler>(mMessageHandlers);
            foreach (IMessageHandler handler in handlerCopy)
            {
                handler.HandleMessage(eventInfo);
            }
        }

        private void LateUpdate()
        {
            if (mPendingMessages.Count > 0)
            {
                List<MessageInfoBase> pendingMessages = new List<MessageInfoBase>(mPendingMessages);
                mPendingMessages.Clear();
                foreach (MessageInfoBase pending in pendingMessages)
                {
                    NotifyListeners(pending);
                }
            }
        }
    }
}

