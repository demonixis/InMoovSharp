using Demonixis.InMoovSharp;
using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Systems;

namespace InMoovCmd
{
    public class InMoovCmdAppTest
    {
        private static int RefreshRate = 30;
        private static Robot Robot;
        private static Thread RobotThread;
        private static bool Running;

        static void Main(string[] args)
        {
            Robot = new Robot();
            Robot.LogEnabled = false;

            var cliSpeech = new CliSpeechSynthesis();
            var cliVoiceReco = new CliVoiceRecognition();

            Robot.AddService(cliSpeech);
            Robot.AddService(cliVoiceReco);
            Robot.InitializeRobot();

            Robot.SwapServices<SpeechSynthesisService>(cliSpeech);
            Robot.SwapServices<VoiceRecognitionService>(cliVoiceReco);
            Robot.LogEnabled = true;

            Running = true;

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("*****************************");
            Console.WriteLine("* InMoov Robot Initialized! *");
            Console.WriteLine("* ***************************");

            RobotThread = new Thread(RobotLoop);
            RobotThread.Start();

            while (Running)
            {
                var result = cliVoiceReco.CheckCli();

                switch (result)
                {
                    case CliVoiceRecognition.CliRecoActions.EnableJawSystem:
                        Robot.SetSystemEnabled<JawMechanism>(true);
                        break;
                    case CliVoiceRecognition.CliRecoActions.DisableJawSystem:
                        Robot.SetSystemEnabled<JawMechanism>(false);
                        break;
                    case CliVoiceRecognition.CliRecoActions.EnableRandomAnimationSystem:
                        Robot.SetSystemEnabled<RandomAnimation>(true);
                        break;
                    case CliVoiceRecognition.CliRecoActions.DisableRandomAnimationSystem:
                        Robot.SetSystemEnabled<RandomAnimation>(false);
                        break;
                    case CliVoiceRecognition.CliRecoActions.Exit:
                        Running = false;
                        break;
                }

                Thread.Sleep(250);
            }

            if (RobotThread.IsAlive)
                RobotThread.Join();

            Robot.Dispose();

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("*****************************");
            Console.WriteLine("* InMoov Robot Stopped      *");
            Console.WriteLine("* ***************************");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Press any key to exit.");

            Console.ReadKey();
        }

        private static void RobotLoop()
        {
            var refreshRate = (int)((1.0f / RefreshRate) * 1000);

            while (Running)
            {
                Robot.CoroutineManager.Update();
                Thread.Sleep(refreshRate);
            }
        }
    }
}