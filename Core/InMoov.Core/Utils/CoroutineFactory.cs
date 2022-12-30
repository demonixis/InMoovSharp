using InMoov.Core.Utils;
using System.Collections;

namespace Demonixis.InMoov.Utils
{
    public static class CoroutineFactory
    {
        public static IEnumerator WaitForSecondsUnscaled(float time)
        {
            var timer = TimeManager.Instance;
            var start = timer.RealtimeSinceStartup;

            while (timer.RealtimeSinceStartup < start + time)
            {
                yield return null;
            }
        }

        public static IEnumerator WaitForSeconds(float time)
        {
            var timer = TimeManager.Instance;
            var start = timer.RealtimeSinceStartup;

            while (timer.RealtimeSinceStartup * timer.TimeScale < start + time)
            {
                yield return null;
            }
        }
    }
}