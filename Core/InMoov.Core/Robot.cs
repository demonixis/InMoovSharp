using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Services.Speech;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Settings;
using Demonixis.InMoov.ComputerVision;
using Demonixis.InMoov.Navigation;
using InMoov.Core.Services;
using InMoov.Core.Utils;
using Demonixis.InMoov.Systems;

namespace Demonixis.InMoov
{
    public class Robot : IDisposable
    {
        private const string ServiceListFilename = "services.json";
        private const string SystemListFilename = "systems.json";
        private static Robot? _instance;

        private List<RobotService> _currentServices;
        private List<RobotService> _registeredServices;
        private List<RobotSystem> _registeredSystems;
        private List<Action> _waitingStartCallbacks;
        private bool _disposed;

        /// <summary>
        /// Gets a static instance of the robot.
        /// </summary>
        public static Robot Instance
        {
            get
            {
                _instance ??= new Robot();
                return _instance;
            }
        }

        public CoroutineManager CoroutineManager { get; private set; }
        public BrainSpeechProxy BrainSpeechProxy { get; private set; }

        public bool Started { get; private set; }

        public event Action<Robot> RobotInitialized;
        public event Action<RobotService, RobotService> ServiceChanged;
        public event Action<RobotService, bool> ServicePaused;

        public Robot()
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("Double Instance detected.");
                return;
            }

            _instance ??= this;
            _registeredServices = new List<RobotService>();
            _registeredSystems = new List<RobotSystem>();
            _currentServices = new List<RobotService>();
            _waitingStartCallbacks = new List<Action>();

            CoroutineManager = new CoroutineManager();
            BrainSpeechProxy = new BrainSpeechProxy();
        }

        protected virtual void RegisterServices()
        {
            AddService(new AIMLNetService());
            AddService(new OpenAIChatbot());
            AddService(new ServoMixerService());
            AddService(new ComputerVisionService());
            AddService(new NavigationService());
            AddService(new SpeechSynthesisService());
            AddService(new VoiceRecognitionService());
        }

        protected virtual void RegisterSystems()
        {
            AddSystem(new JawMechanism());
            AddSystem(new RandomAnimation());
        }

        public void InitializeRobot()
        {
            if (Started)
            {
                Debug.Log("The Robot was already started.");
                return;
            }

            RegisterServices();
            RegisterSystems();

            // TODO in prevision of deferred load.
            InitializeServices();

            Started = true;
            RobotInitialized?.Invoke(this);

            InitializeSystems();

            if (_waitingStartCallbacks.Count <= 0) return;
            foreach (var callback in _waitingStartCallbacks)
                callback?.Invoke();

            _waitingStartCallbacks.Clear();
        }

        public void WhenStarted(Action callback)
        {
            if (Started)
                callback?.Invoke();
            else
                _waitingStartCallbacks.Add(callback);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            var serviceList = new ServiceList();

            foreach (var service in _currentServices)
            {
                if (service is ChatbotService)
                    serviceList.Chatbot = service.ServiceName;
                else if (service is VoiceRecognitionService)
                    serviceList.VoiceRecognition = service.ServiceName;
                else if (service is SpeechSynthesisService)
                    serviceList.SpeechSynthesis = service.ServiceName;
                else if (service is ServoMixerService)
                    serviceList.ServoMixer = service.ServiceName;
                else if (service is NavigationService)
                    serviceList.Navigation = service.ServiceName;
                else if (service is ComputerVisionService)
                    serviceList.ComputerVision = service.ServiceName;
                else if (service is XRService)
                    serviceList.XR = service.ServiceName;
            }

            // Save the list of used services
            SaveGame.SaveRawData(serviceList, ServiceListFilename, "Config");

            ClearCurrentServices();

            // Save the list of used systems
            var activeSystems = new List<string>();
            foreach (var system in _registeredSystems)
            {
                if (!system.Started) continue;
                activeSystems.Add(system.GetType().Name);
                system.SetActive(false);
            }

            SaveGame.SaveRawData(activeSystems.ToArray(), SystemListFilename,
                "Config");

            GlobalSettings.Save();

            _disposed = true;
        }

        #region Service Management

        public void AddService(RobotService service)
        {
            if (_registeredServices.Contains(service)) return;
            _registeredServices.Add(service);
        }

        /// <summary>
        /// Initialize services to make the robot alive
        /// </summary>
        private void InitializeServices()
        {
            var serviceList =
                SaveGame.LoadRawData<ServiceList>(ServiceListFilename, "Config");

            if (!serviceList.IsValid())
                serviceList = ServiceList.New();

            // Select service selected by the user
            var chatbotService = SelectService<ChatbotService>(serviceList.Chatbot);
            var voiceRecognition = SelectService<VoiceRecognitionService>(serviceList.VoiceRecognition);
            var speechSynthesis = SelectService<SpeechSynthesisService>(serviceList.SpeechSynthesis);
            SelectService<ServoMixerService>(serviceList.ServoMixer);
            SelectService<XRService>(serviceList.XR);
            SelectService<NavigationService>(serviceList.Navigation);
            SelectService<ComputerVisionService>(serviceList.ComputerVision);

            BrainSpeechProxy.Setup(chatbotService, voiceRecognition, speechSynthesis);
        }

        /// <summary>
        /// Select the service T by its name and use a fallback if not available
        /// </summary>
        /// <param name="targetService">The service name</param>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>It returns a RobotService</returns>
        private T SelectService<T>(string targetService) where T : RobotService
        {
            var services = GetServicesOfType<T>();
            T? selectedService = null;

            foreach (T service in services)
            {
                if (service.ServiceName == targetService && service.IsSupported())
                    selectedService = service;

                if (selectedService == null && service.IsSupported())
                    selectedService = service;
            }

            if (selectedService == null)
            {
                throw new Exception($"Service {targetService} is not available and there is no suitable service found.");
            }

            _currentServices.Add(selectedService);

            selectedService.Initialize();

            Debug.Log($"[{typeof(T)} service: {selectedService}");

            return selectedService;
        }

        public RobotService[] GetServicesOfType<T>() where T : RobotService
        {
            var list = new List<T>();

            foreach (var service in _registeredServices)
            {
                if (service is T)
                    list.Add((T)service);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Shutdown loaded services and free the resources.
        /// </summary>
        private void ClearCurrentServices()
        {
            if (_currentServices.Count <= 0) return;

            foreach (var service in _currentServices)
                service.Dispose();

            _currentServices.Clear();
        }

        /// <summary>
        /// Gets the active service of type T.
        /// </summary>
        /// <typeparam name="T">The type of serice.</typeparam>
        /// <returns>It returns a service.</returns>
        public T GetService<T>() where T : RobotService
        {
            var type = typeof(T);
            foreach (var current in _currentServices)
            {
                if (current is T service)
                    return service;
            }

            Debug.Log($"Service {type} was not found. It's not really possible...");
            return null;
        }

        public bool TryGetService<T>(out T outService) where T : RobotService
        {
            var type = typeof(T);
            foreach (var current in _currentServices)
            {
                if (current is not T service) continue;
                outService = service;
                return true;
            }

            Debug.Log($"Service {type} was not found. It's not really possible...");
            outService = null;
            return false;
        }

        /// <summary>
        /// Change a service by another one using a name.
        /// </summary>
        /// <typeparam name="T">The type of service</typeparam>
        /// <param name="serviceName">The new service name</param>
        public void ReplaceService<T>(string serviceName) where T : RobotService
        {
            if (TryFindServiceByName(serviceName, out T newService))
            {
                var oldService = GetService<T>();
                oldService.Dispose();
                _currentServices.Remove(oldService);

                newService.Initialize();
                _currentServices.Add(newService);

                ServiceChanged?.Invoke(oldService, newService);
            }
            else
                Debug.Log($"Robot::ReplaceService - I wasn't able to find the service {serviceName}.");
        }

        /// <summary>
        /// Try to find a service using its name.
        /// </summary>
        /// <param name="serviceName">The sercice name</param>
        /// <param name="outService">The service instance</param>
        /// <typeparam name="T">It returns true if the service was found, otherwise it returns false.</typeparam>
        /// <returns></returns>
        private bool TryFindServiceByName<T>(string serviceName, out T? outService) where T : RobotService
        {
            var services = GetServicesOfType<T>();

            foreach (T newService in services)
            {
                if (newService.ServiceName != serviceName) continue;
                outService = newService;
                return true;
            }

            outService = null;
            return false;
        }

        /// <summary>
        /// Pause a service by its type.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="paused">Set to true to pause the service and false to unpause it.</param>
        public void SetServicePaused<T>(bool paused) where T : RobotService
        {
            if (!TryGetService(out T service)) return;
            service.SetPaused(paused);
            ServicePaused?.Invoke(service, paused);
        }

        #endregion

        #region Systems Management

        public void AddSystem(RobotSystem system)
        {
            if (_registeredSystems.Contains(system)) return;
            _registeredSystems.Add(system);
        }

        private void InitializeSystems()
        {
            var systemsList =
                SaveGame.LoadRawData<string[]>(SystemListFilename, "Config");

            if (systemsList == null || systemsList.Length == 0) return;

            foreach (var system in _registeredSystems)
            {
                if (Array.IndexOf(systemsList, system.GetType().Name) == -1) continue;
                system.SetActive(true);
            }
        }

        #endregion        
    }
}