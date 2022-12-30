﻿using System;
using System.Collections.Generic;
using System.Text;
using InMoov.Core.Services;

namespace Demonixis.InMoov.Chatbots
{
    public abstract class ChatbotService : RobotService
    {
        [NonSerialized]
        public List<string> DetectedObjects = new();
        [NonSerialized]
        public List<string> DetectedPersons = new();
        
        public event Action<string> ResponseReady;

        public abstract void SetLanguage(string culture);

        public void SubmitResponse(string inputSpeech)
        {
            if (DetectedObjects.Count > 0 && CheckIfWhatDoYouSee(inputSpeech))
            {
                var message = new StringBuilder();
                message.Append($"I can see {DetectedObjects.Count} objects. ");

                foreach (var item in DetectedObjects)
                    message.Append($"{item}. ");

                NotifyResponseReady(message.ToString());
            }
            else
                SubmitResponseToBot(inputSpeech);
        }

        protected abstract void SubmitResponseToBot(string phrase);

        protected void NotifyResponseReady(string response)
        {
            ResponseReady?.Invoke(response);
        }
        
        private bool CheckIfWhatDoYouSee(string phrase)
        {
            return phrase.Contains("what") &&
                   phrase.Contains("you") &&
                   phrase.Contains("see");
        }
    }
}