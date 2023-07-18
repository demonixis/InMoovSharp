using Demonixis.InMoovSharp.Services;

namespace InMoovCmd
{
    public class CliSpeechSynthesis : SpeechSynthesisService
    {
        public override void Speak(string message)
        {
            if (Paused) return;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"InMoov: {message}");
        }
    }
}
