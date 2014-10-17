using ShareLink.Services.Interfaces;
using System.Collections.Generic;
using Windows.Storage;

namespace ShareLink.Services
{
    public class PlatformApplicationDataContainer : IApplicationDataContainer
    {
        private const string RoamingSettingsContainerName = "RoamingSettings";

        private ApplicationDataContainer _settingsContainer;

        private ApplicationDataContainer SettingsContainer
        {
            get
            {
                if (_settingsContainer == null)
                {
                    _settingsContainer = ApplicationData.Current.RoamingSettings.CreateContainer(RoamingSettingsContainerName, ApplicationDataCreateDisposition.Always);
                }

                return _settingsContainer;
            }
        }

        public IDictionary<string, object> Values
        {
            get { return SettingsContainer.Values; }
        }

        public void Delete()
        {
            ApplicationData.Current.LocalSettings.DeleteContainer(RoamingSettingsContainerName);
        }
    }
}