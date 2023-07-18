using Demonixis.InMoovSharp.Services;

namespace InMoovCmd
{
    public class CliVoiceRecognition : VoiceRecognitionService
    {
        public enum CliRecoActions
        {
            None = 0,
            Exit,
            EnableJawSystem,
            DisableJawSystem,
            EnableRandomAnimationSystem,
            DisableRandomAnimationSystem
        }

        protected override void SafeInitialize()
        {
            base.SafeInitialize();
            WordTrigger = string.Empty;
        }

        public CliRecoActions CheckCli()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("You: ");

            var str = Console.ReadLine();
            if (!string.IsNullOrEmpty(str))
            {
                str = str.ToLower();

                if (str == "exit")
                    return CliRecoActions.Exit;
                else if (str == "jaw=1")
                    return CliRecoActions.EnableJawSystem;
                else if (str == "jaw=0")
                    return CliRecoActions.DisableJawSystem;
                else if (str == "anim=1")
                    return CliRecoActions.EnableRandomAnimationSystem;
                else if (str == "anim=0")
                    return CliRecoActions.DisableRandomAnimationSystem;
            }

            NotifyPhraseDetected(str);
            return CliRecoActions.None;
        }
    }
}
