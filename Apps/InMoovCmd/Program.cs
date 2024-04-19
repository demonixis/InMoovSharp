using AIMLbot;
using Demonixis.InMoovSharp;
using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Systems;

namespace InMoovCmd
{
    public class InMoovCmdAppTest
    {
        private const int RefreshRate = 30;
        private const string AppVersion = "0.2.0";
        private static Robot Robot;
        private static Thread RobotThread;
        private static bool Running;
        private static CliVoiceRecognition.CliRecoActions LastResult;

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
            Console.WriteLine($"* InMoov CLI {AppVersion}          *");
            Console.WriteLine("*****************************");
            Console.WriteLine("* Robot Initialized!        *");
            Console.WriteLine("*****************************");
            Console.WriteLine();

            RobotThread = new Thread(RobotLoop);
            RobotThread.Start();

            while (Running)
            {
                LastResult = cliVoiceReco.CheckCli();

                if (LastResult == CliVoiceRecognition.CliRecoActions.Exit)
                    Running = false;

                Thread.Sleep(250);
            }

            //if (RobotThread.IsAlive)
            //RobotThread.Join();

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
                switch (LastResult)
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

                LastResult = CliVoiceRecognition.CliRecoActions.None;

                Robot.UpdateRobot();
                Thread.Sleep(refreshRate);
            }
        }
    }
}