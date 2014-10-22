using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using Services;
using Services.Interfaces;
using ShareLink.Services;
using ShareLink.Services.Interfaces;
using ShareLink.Views;
#if WINDOWS_APP
using Windows.UI.ApplicationSettings;
#endif
namespace ShareLink
{
    public sealed partial class App : MvvmAppBase
    {
        private readonly IUnityContainer _container = new UnityContainer();

        public App()
        {
            InitializeComponent();
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Main", null);
            Window.Current.Activate();
            return Task.FromResult<object>(null);
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {

            _container.RegisterInstance(NavigationService);
            _container.RegisterInstance(SessionStateService);
            _container.RegisterInstance(new ApplicationSettingsService(new PlatformApplicationDataContainer()));
            _container.RegisterType<IWindowService, WindowService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDataTransferService, DataTransferService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IHttpService, HttpService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISchedulerProvider, SchedulerProvider>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ITextToSpeechService, TextToSpeechService>(new ContainerControlledLifetimeManager());
#if WINDOWS_APP
            _container.RegisterType<IClipboardService, Services.Windows.ClipboardService>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<ISettingsService>(new UiServices.Windows.SettingService());
#elif WINDOWS_PHONE_APP
            _container.RegisterType<IClipboardService, Services.WindowsPhone.ClipboardService>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<ISettingsService>(new UiServices.WindowsPhone.SettingService(NavigationService));
#endif

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture,
                    "ShareLink.ViewModels.ViewModels.{0}ViewModel, ShareLink.ViewModels, Version=1.0.0.0, Culture=neutral",
                    viewType.Name);

                var viewModelType = Type.GetType(viewModelTypeName);

                return viewModelType;
            });
            return base.OnInitializeAsync(args);
        }

        protected override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

#if WINDOWS_APP
        protected override IList<SettingsCommand> GetSettingsCommands()
        {
            var settingsCommands = new List<SettingsCommand>();
            settingsCommands.Add(new SettingsCommand(Guid.NewGuid().ToString(), "Settings", (c) => new SettingsPage().Show()));

            return settingsCommands;
        }
#endif
    }
}