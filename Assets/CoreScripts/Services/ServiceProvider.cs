using System;
using System.Collections.Generic;

namespace Asteroid.Services
{
    /// <summary>
    /// A simple service configuration system. It can be instantiated by using the Builder class
    /// Limitations: Only one object can be registered per type. Should maybe handle collections too
    /// </summary>
    public sealed class ServiceProvider
    {
        public static ServiceProviderBuilder Build() => new ServiceProviderBuilder();

        private ServiceProvider()
        {
        }

        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Finds and returns the service registered with the specified type
        /// </summary>
        /// <typeparam name="T">The type the service is registered with</typeparam>
        /// <returns>The found service</returns>
        /// <exception cref="KeyNotFoundException">Thrown if there is no service segistered with the specified type</exception>
        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            else
            {
                throw new KeyNotFoundException($"No service registered for type {typeof(T)}");
            }
        }

        /// <summary>
        /// The builder class to create ServiceProvider instances
        /// </summary>
        public sealed class ServiceProviderBuilder
        {
            private ServiceProvider _serviceProvider = new ServiceProvider();
            private bool _isInitialised = false;

            /// <summary>
            /// Adds a service to the builder config.
            /// </summary>
            /// <typeparam name="T">The type for the service to register with</typeparam>
            /// <param name="service">The service to register</param>
            /// <returns>The builder</returns>
            /// <exception cref="InvalidOperationException">Thrown if the builder is already initialised</exception>
            /// <exception cref="ArgumentNullException">Thrown if the service is null</exception>
            /// <exception cref="ArgumentException">Thrown if the service is already registered</exception>
            public ServiceProviderBuilder RegisterService<T>(T service)
            {
                if (_isInitialised)
                    throw new InvalidOperationException("Builder is already initialized before!");

                if (service == null)
                    throw new ArgumentNullException(nameof(service));
                if (_serviceProvider._services.ContainsKey(typeof(T)))
                    throw new ArgumentException($"There is a service already registered for class {typeof(T)}");

                _serviceProvider._services.Add(typeof(T), service);

                return this;
            }

            /// <summary>
            /// Initialises all the registered services (if they are implementing IInitialisableService) and returns the service provider
            /// </summary>
            /// <returns>The finished service provider</returns>
            /// <exception cref="InvalidOperationException">Thrown if the builder is already initialised</exception>
            public ServiceProvider Initialise()
            {
                if (_isInitialised)
                    throw new InvalidOperationException("Builder is already initialized before!");

                foreach (var service in _serviceProvider._services.Values)
                {
                    if(service is IInitialisableService init)
                    {
                        init.Initialise(_serviceProvider);
                    }
                }

                _isInitialised = true;
                return _serviceProvider;
            }

        }

        /// <summary>
        /// Initialisation function for services. Can be used to connect the registered services with each other.
        /// </summary>
        public interface IInitialisableService
        {
            void Initialise(ServiceProvider serviceProvider);
        }

    }


}
