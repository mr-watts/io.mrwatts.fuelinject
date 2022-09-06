using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Metadata;

namespace MrWatts.Internal.FuelInject
{
    public class OrderedRegistrationSource : IRegistrationSource
    {
        public bool IsAdapterForIndividualComponents => false;

        public override string ToString() => "Source for IOrderedEnumerable to allow sorting enumerations of services";

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            if (service is not IServiceWithType)
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            IServiceWithType serviceWithType = (IServiceWithType)service;

            if (!IsSpecializationOf(serviceWithType.ServiceType, typeof(IOrderedEnumerable<>)))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            return new[] {
                (IComponentRegistration)typeof(OrderedRegistrationSource).GetRuntimeMethods().Single(x => x.Name == nameof(CreateRegistration))
                    .MakeGenericMethod(serviceWithType.ServiceType.GenericTypeArguments.Single())
                    .Invoke(null, new object[0])
            };
        }

        private static bool IsSpecializationOf(Type type, Type genericTypeDefinition)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
        }

        private static IComponentRegistration CreateRegistration<TService>()
        {
            return RegistrationBuilder
                .ForDelegate((context, parameters) => Resolve<TService>(context, parameters))
                .ExternallyOwned()
                .CreateRegistration();
        }

        private static IOrderedEnumerable<TService> Resolve<TService>(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(typeof(Meta<>).MakeGenericType(typeof(TService)));
            var serviceMetaItems = (Meta<TService>[])context.Resolve(type, parameters);

            return serviceMetaItems
                .OrderBy(x => x.Metadata.ContainsKey(OrderingMetadataKey.MAIN_KEY) ? (IComparable)x.Metadata[OrderingMetadataKey.MAIN_KEY]! : 0)
                .Select(x => x.Value)
                .OrderBy(x => 1);
        }
    }
}