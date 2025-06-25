using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Autofac registration source that registers all services that are supported by another container. When asked to
    /// provide them, they are fetched from that container as well. This allows forwarding services to another container.
    /// </summary>
    public sealed class ContainerForwardingRegistrationSource : MonoBehaviour, IRegistrationSource
    {
        [Inject]
        private IComponentContext ComponentContext { get; set; } = default!;

        public bool IsAdapterForIndividualComponents => true;
        public bool IsInitialized => ComponentContext is not null;

        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            if (service is not TypedService ||
                registrationAccessor(service).Any())
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            TypedService serviceWithType = (TypedService)service;
            if (serviceWithType.ServiceType.IsGenericType && !ShouldRegisterGenericService(serviceWithType.ServiceType))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            if (IsInsideAutofac(serviceWithType.ServiceType))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            if (!ComponentContext.IsRegistered(serviceWithType.ServiceType))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            return new[]
            {
                new ComponentRegistration(
                    Guid.NewGuid(),
                    new DelegateActivator(
                        serviceWithType.ServiceType,
                        (_, _) =>
                        {
                            if (ComponentContext.TryResolveService(service, out object? result))
                            {
                                return result;
                            }

                            return default!;
                        }),
                    new CurrentScopeLifetime(),
                    InstanceSharing.Shared,
                    InstanceOwnership.ExternallyOwned,
                    new Service[] { service },
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                ),
            };
        }

        private static bool ShouldRegisterGenericService(Type type)
        {
            Type genericType = type.GetGenericTypeDefinition();

            return genericType != typeof(Lazy<>) && !IsInsideAutofac(genericType);
        }

        private static bool IsInsideAutofac(Type type)
        {
            return typeof(IRegistrationSource).Assembly == type.Assembly;
        }
    }
}