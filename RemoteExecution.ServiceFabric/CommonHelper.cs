using System.Fabric;
using System.Fabric.Description;
using System.Net;
using System.Reflection;

namespace RemoteExecution.ServiceFabric
{
    internal static class CommonHelper
    {
        public static string Loopback = IPAddress.Loopback.ToString();

        public static string GetUriFromContext(ServiceContext context, string endpointName)
        {
            EndpointResourceDescription description = context.CodePackageActivationContext.GetEndpoint(endpointName);
            string portstr = description.Port == 0 ? "" : $":{description.Port}";
            string host = FabricRuntime.GetNodeContext().IPAddressOrFQDN;
            if (host == "localhost")
                host = Loopback;
            return
                $"net://{host}{portstr}/{description.Name}";
        }
        // todo: make these safe for arrays

        public static void SetField(this object me, string fieldName, object value)
        {
            me.GetType().GetRuntimeField(fieldName).SetValue(me, value);
        }

        public static void SetProperty(this object me, string propertyName, object value)
        {
            me.GetType().GetRuntimeProperty(propertyName).SetValue(me, value);
        }

        public static object GetField(this object me, string fieldName)
        {
            return me.GetType().GetRuntimeField(fieldName).GetValue(me);
        }

        public static T GetField<T>(this object me, string fieldName)
        {
            return (T)GetField(me, fieldName);
        }

        public static object GetProperty(this object me, string propertyName)
        {
            return me.GetType().GetRuntimeProperty(propertyName).GetValue(me);
        }

        public static T GetProperty<T>(this object me, string propertyName)
        {
            return (T)GetProperty(me, propertyName);
        }
    }
}