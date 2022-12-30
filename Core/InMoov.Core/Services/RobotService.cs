﻿using Demonixis.InMoov;
using System.Collections;

namespace InMoov.Core.Services
{
    /// <summary>
    /// Base skeleton of a robot service.
    /// A service must have a type. By default it is supported on all platforms.
    /// </summary>
    public abstract class RobotService : IDisposable
    {
        public virtual RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.Android,
            RuntimePlatform.LinuxEditor,
            RuntimePlatform.LinuxPlayer,
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.OSXEditor,
            RuntimePlatform.OSXPlayer
        };

        public string ServiceName => GetType().Name;
        public bool Started { get; protected set; }
        public bool Paused { get; protected set; }

        public virtual void Initialize()
        {
            Started = true;
        }

        public virtual void SetPaused(bool paused)
        {
            Paused = paused;
        }

        public virtual void Dispose()
        {
            Started = false;
        }

        public bool IsSupported()
        {
            return Array.IndexOf(SupportedPlateforms, Application.platform) > -1;
        }

        protected void StartCoroutine(IEnumerator coroutine)
        {
            Robot.Instance.CoroutineManager.Start(this, coroutine);
        }

        protected void StopAllCoroutines()
        {
            Robot.Instance.CoroutineManager.StopAll(this);
        }
    }
}