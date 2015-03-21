using System;
using System.Collections.Generic;
using System.Linq;
using CityWebServer.Extensibility;
using CityWebServer.Models;
using ColossalFramework;
using ICities;

namespace CityWebServer.Retrievers
{
    public class ChirpRetriever : ILogAppender
    {
        public event EventHandler<LogAppenderEventArgs> LogMessage;

        private void OnLogMessage(String message)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogAppenderEventArgs(message));
            }
        }

        private readonly MessageManager _manager;
        private List<ChirperMessage> _messages;

        public ChirperMessage[] Messages
        {
            get { return _messages.ToArray(); }
            set { _messages = value.ToList(); }
        }

        public ChirpRetriever()
        {
            _manager = Singleton<MessageManager>.instance;
            _manager.m_messagesUpdated += ManagerOnMMessagesUpdated;
            _manager.m_newMessages += ManagerOnMNewMessages;
            _messages = new List<ChirperMessage>();
        }

        private void ManagerOnMNewMessages(IChirperMessage message)
        {
            try
            {
                var msg = new ChirperMessage
                {
                    SenderID = (int)message.senderID,
                    SenderName = message.senderName,
                    Text = message.text
                };
                _messages.Add(msg);
            }
            catch (Exception ex)
            {
                OnLogMessage(ex.ToString());
            }
        }

        private void ManagerOnMMessagesUpdated()
        {
            try
            {
                var messages = _manager.GetRecentMessages();
                _messages = messages.Select(obj => new ChirperMessage
                {
                    SenderID = (int)obj.GetSenderID(),
                    SenderName = obj.GetSenderName(),
                    Text = obj.GetText(),
                }).ToList();
            }
            catch (Exception ex)
            {
                OnLogMessage(ex.ToString());
            }
        }
    }
}