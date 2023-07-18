using Demonixis.InMoovSharp;
using Demonixis.InMoovSharp.Services;

namespace InMoovCmd
{
    public class InMoovCmdAppTest
    {
        private static int RefreshRate = 30;

        static void Main(string[] args)
        {
            var robot = new Robot();
            robot.LogEnabled = false;

            var cliSpeech = new CliSpeechSynthesis();
            var cliVoiceReco = new CliVoiceRecognition();

            robot.AddService(cliSpeech);
            robot.AddService(cliVoiceReco);
            robot.InitializeRobot();

            robot.SwapServices<SpeechSynthesisService>(cliSpeech);
            robot.SwapServices<VoiceRecognitionService>(cliVoiceReco);
            robot.LogEnabled = true;

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("*****************************");
            Console.WriteLine("* InMoov Robot Initialized! *");
            Console.WriteLine("* ***************************");

            var refreshRate = (int)((1.0f / RefreshRate) * 1000);

            while (robot.Started)
            {
                robot.CoroutineManager.Update();
                cliVoiceReco.CheckCli();
                Thread.Sleep(refreshRate);
            }

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("*****************************");
            Console.WriteLine("* InMoov Robot Stopped      *");
            Console.WriteLine("* ***************************");
        }
    }
}