namespace SistemaPeaje.Infrastructure.Workers
{
    /// <summary>
    /// Configuración para el monitor del PLC (usado por compatibilidad)
    /// </summary>
    public class PlcMonitorOptions
    {
        public const string SectionName = "PlcMonitor";
        
        public string PlcIp { get; set; } = "192.168.1.103";
        public int PlcPort { get; set; } = 502;
        public byte UnitId { get; set; } = 1;
        public ushort StartAddress { get; set; } = 1000;
        public ushort CoilCount { get; set; } = 4;
        public TimeSpan MonitorInterval { get; set; } = TimeSpan.FromSeconds(2);
        public bool EnablePeriodicLogging { get; set; } = true;
        public int EstacionId { get; set; } = 1; // ID de la estación de peaje
        public int CarrilId { get; set; } = 1; // ID del carril a monitorear
        public Dictionary<int, string> CoilNames { get; set; } = new()
        {
            { 0, "Presencia" },
            { 1, "BarreraAbierta" },
            { 2, "SentidoAB" },
            { 3, "Alarma" }
        };
    }
}
