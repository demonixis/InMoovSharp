using Demonixis.InMoovSharp.Services;

namespace InMoovCmd
{
    public class CliVoiceRecognition : VoiceRecognitionService
    {
        protected override void SafeInitialize()
        {
            base.SafeInitialize();
            WordTrigger = string.Empty;
        }

        public void CheckCli()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("You: ");

            var str = Console.ReadLine();
            if (!string.IsNullOrEmpty(str))
                NotifyPhraseDetected(str);
        }
    }
}
