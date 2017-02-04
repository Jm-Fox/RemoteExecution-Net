using System.Reflection;

namespace RemoteExecution
{
    /// <summary>
    /// When paired with a class, IIncomplete indicates that the class cannot
    /// be fully deserialized without more context (the methodinfo that is being used)
    /// </summary>
    public interface IIncomplete
    {
        /// <summary>
        /// Provides the information needed to fully deserialize an IIncomplete class
        /// </summary>
        /// <param name="info">Method that is being invoked or was invoked</param>
        void Complete(MethodInfo info);
    }
}