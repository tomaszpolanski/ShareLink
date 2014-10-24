using System;
using Windows.Storage;

namespace ShareLink.Services.Universal
{
    public class RoamingCacheService : CacheService
    {
        public RoamingCacheService() :
            base(ApplicationData.Current.TemporaryFolder, TimeSpan.FromDays(30))
        {
                
        }
    }
}
