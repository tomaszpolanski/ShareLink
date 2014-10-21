using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using ShareLink.Services.Interfaces;

namespace ShareLink.Services
{
    public class ApplicationSettingsService
    {
        private readonly IApplicationDataContainer _dataContainer;

        public ApplicationSettingsService(IApplicationDataContainer dataContainer)
        {
            _dataContainer = dataContainer;

            LoadSettings();
        }


        public IObservable<bool> IsSpeechEnabledObservable { get { return _isSpeechEnabledObservable.StartWith(IsSpeechEnabled); } }
        private readonly Subject<bool> _isSpeechEnabledObservable = new Subject<bool>();

        private bool _isSpeechEnabled;

        [PersistentPropertyAttribute]
        public bool IsSpeechEnabled
        {
            get { return _isSpeechEnabled; }
            set { SetSettingsProperty(ref _isSpeechEnabled, value, _isSpeechEnabledObservable); }
        }

        public void Reset()
        {
            _dataContainer.Delete();
        }

        #region Private members

        private void LoadSettings()
        {
            var persistentPropertiesQuery = GetType().GetRuntimeProperties()
                                                     .Where(singleProperty => singleProperty.GetCustomAttributes(inherit: true).OfType<PersistentPropertyAttribute>().Any())
                                                     .Select(singleProperty => singleProperty.Name);

            foreach (var singlePropertyName in persistentPropertiesQuery)
            {
                GetSettingsProperty(singlePropertyName);
            }
        }

        private void SetSettingsProperty<T>(ref T storage, T value, Subject<T> handler, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return;
            }

            storage = value;
            if (value is Enum)
            {
                _dataContainer.Values[propertyName] = value.ToString();
            }
            else if (value is DateTime?)
            {
                var d = value as DateTime?;
                _dataContainer.Values[propertyName] = d.Value.ToString("s", DateTimeFormatInfo.InvariantInfo);

            }
            else
            {
                _dataContainer.Values[propertyName] = value;
            }

            if (handler != null)
            {
                handler.OnNext(value);
            }
        }

        private void GetSettingsProperty<T>(Expression<Func<T>> expr)
        {
            var body = (MemberExpression)expr.Body;
            GetSettingsProperty(body.Member.Name);
        }

        private void GetSettingsProperty(String propertyName)
        {
            var property = GetType().GetRuntimeProperty(propertyName);

            if (property != null && _dataContainer.Values.ContainsKey(propertyName))
            {
                object value;
                if (property.PropertyType.GetTypeInfo().BaseType == typeof(Enum))
                {
                    value = Enum.Parse(property.PropertyType, _dataContainer.Values[propertyName] as string);
                }
                else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime) ||
                         property.PropertyType == typeof(DateTime))
                {
                    value = null;
                    if (_dataContainer.Values[propertyName] != null)
                    {
                        value = DateTime.Parse(_dataContainer.Values[propertyName] as string);
                    }
                }
                else
                {
                    value = Convert.ChangeType(_dataContainer.Values[propertyName], property.PropertyType);
                }
                property.SetValue(this, value, null);
            }
        #endregion
        }

        [AttributeUsage(AttributeTargets.Property)]
        private class PersistentPropertyAttribute : Attribute
        {
            public PersistentPropertyAttribute()
            {
            }
        }


    }
}