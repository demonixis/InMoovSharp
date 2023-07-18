using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Utils;
using System.Collections;

namespace InMoovCmd
{
    public class CliSpeechSynthesis : SpeechSynthesisService
    {
        public override void Speak(string message)
        {
            if (Paused) return;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"InMoov: {message}");

            StartCoroutine(SpeechLoop(message));
        }

        private IEnumerator SpeechLoop(string message)
        {
            NotifySpeechStarted(message);

            var waitTime = GetSpeakTime(message);

            yield return CoroutineFactory.WaitForSeconds(waitTime);

            NotifySpeechState(false);

            yield return CoroutineFactory.WaitForSeconds(DelayAfterSpeak);

            NotifySpeechState(true);
        }
    }
}
