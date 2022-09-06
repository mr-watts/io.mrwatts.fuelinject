using System;
using Autofac.Builder;

namespace MrWatts.Internal.FuelInject
{
    public static class RegistrationOrderingExtensions
    {
        /// <summary>
        /// Indicates the ordering/priority of this service when collections of its type are resolved.
        /// </summary>
        /// <typeparam name="TLimit"></typeparam>
        /// <typeparam name="TActivatorData"></typeparam>
        /// <typeparam name="TRegistrationStyle"></typeparam>
        /// <param name="registration"></param>
        /// <param name="order"></param>
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> WithOrder<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration,
            IComparable order
        ) {
            return registration.WithMetadata(OrderingMetadataKey.MAIN_KEY, order);
        }
    }
}