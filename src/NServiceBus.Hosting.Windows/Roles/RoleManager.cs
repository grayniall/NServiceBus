﻿namespace NServiceBus
{
    using System;
    using System.Linq;
    using NServiceBus.Features;

    class RoleManager
    {

        public static void TweakConfigurationBuilder(IConfigureThisEndpoint specifier, ConfigurationBuilder config)
        {
            if (specifier is AsA_Server)
            {
            }
            else if (specifier is AsA_Client)
            {
                config.PurgeOnStartup(true); 
            }
        }

        public static void TweakConfig(IConfigureThisEndpoint specifier,Configure config)
        {
            if (specifier is AsA_Server)
            {
                config.ScaleOut(s => s.UseSingleBrokerQueue());
            }
            else if (specifier is AsA_Client)
            {
                config.DisableFeature<Features.SecondLevelRetries>()
                    .DisableFeature<StorageDrivenPublishing>()
                    .DisableFeature<TimeoutManager>()
                    .Transactions(t => t.Disable());
            }

            Type transportDefinitionType;
            if (TryGetTransportDefinitionType(specifier, out transportDefinitionType))
            {
                config.UseTransport(transportDefinitionType);
            }
        }

        static bool TryGetTransportDefinitionType(IConfigureThisEndpoint specifier, out Type transportDefinitionType)
        {
            var transportType= specifier.GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .SingleOrDefault(x => x.GetGenericTypeDefinition() == typeof(UsingTransport<>));
            if (transportType != null)
            {
                transportDefinitionType = transportType.GetGenericArguments().First();
                return true;
            }
            transportDefinitionType = null;
            return false;
        }
    }


}