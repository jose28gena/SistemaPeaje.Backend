namespace SistemaPeaje.Application.Features.Reportes;

public class ReporteOperacionDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public int TotalTransacciones { get; set; }
    public decimal MontoTotalRecaudado { get; set; }
    public decimal PromedioTransaccionesPorHora { get; set; }
    public int TurnosRegistrados { get; set; }
    public int TurnosCerrados { get; set; }
    public Dictionary<string, int> TransaccionesPorTipoPago { get; set; } = new();
    public Dictionary<string, decimal> MontosPorTipoPago { get; set; } = new();
    public Dictionary<string, int> TransaccionesPorVehiculo { get; set; } = new();
    public DateTime FechaGeneracion { get; set; }
}

public class ReporteTraficoDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public int TotalVehiculos { get; set; }
    public decimal PromedioVehiculosPorDia { get; set; }
    public int PicoMaximoVehiculos { get; set; }
    public VolumenHorario? HorarioPico { get; set; }
    public List<VolumenHorario> VolumenPorHora { get; set; } = new();
    public Dictionary<string, int> VehiculosPorTipo { get; set; } = new();
    public Dictionary<DateTime, int> DistribucionPorDia { get; set; } = new();
    public DateTime FechaGeneracion { get; set; }
}

public class VolumenHorario
{
    public DateTime Fecha { get; set; }
    public int Hora { get; set; }
    public int Vehiculos { get; set; }
    public decimal MontoRecaudado { get; set; }
}

public class ReporteIngresosDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public decimal IngresoTotal { get; set; }
    public decimal IngresoPromedioDiario { get; set; }
    public Dictionary<string, decimal> IngresosPorTipoPago { get; set; } = new();
    public decimal TransaccionPromedio { get; set; }
    public decimal MetaPorcentaje { get; set; }
    public DateTime FechaGeneracion { get; set; }
}

public class ReporteEjecutivoDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public KPIsDto KPIs { get; set; } = new();
    public List<VolumenHorario> TendenciasTrafico { get; set; } = new();
    public Dictionary<string, decimal> DistribucionIngresos { get; set; } = new();
    public List<string> AlertasOperativas { get; set; } = new();
    public DateTime FechaGeneracion { get; set; }
}

public class KPIsDto
{
    public int TotalTransacciones { get; set; }
    public decimal IngresoTotal { get; set; }
    public int TotalVehiculos { get; set; }
    public decimal PromedioTransaccionesPorHora { get; set; }
    public decimal PromedioVehiculosPorDia { get; set; }
    public decimal TasaEficiencia { get; set; }
    public decimal IndiceCumplimiento { get; set; }
}
