using log4net;

namespace TOKS_lab1.backend
{
    /// <summary>
    /// log4net wrapper
    /// </summary>
    public static class InternalLogger
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Program));
    }
}