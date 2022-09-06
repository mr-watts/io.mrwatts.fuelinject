using System;
using System.Runtime.Serialization;

namespace MrWatts.Internal.FuelInject.Testing.Utility
{
    public sealed class GameObjectFinderException : Exception
    {
        public GameObjectFinderException()
        {
        }

        public GameObjectFinderException(string message) : base(message)
        {
        }

        public GameObjectFinderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GameObjectFinderException(SerializationInfo info, in StreamingContext context) : base(info, context)
        {
        }
    }
}