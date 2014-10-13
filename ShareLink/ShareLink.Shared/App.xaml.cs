using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using Services;
using Services.Interfaces;
using ShareLink.ViewModels.ViewModels;

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
            _container.RegisterType<IWindowService, WindowService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDataTransferService, DataTransferService>(new ContainerControlledLifetimeManager());
#if WINDOWS_APP
            _container.RegisterType<IClipboardService, Services.Windows.ClipboardService>(new ContainerControlledLifetimeManager());
#elif WINDOWS_PHONE_APP
            _container.RegisterType<IClipboardService, Services.WindowsPhone.ClipboardService>(new ContainerControlledLifetimeManager());
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
    }
}