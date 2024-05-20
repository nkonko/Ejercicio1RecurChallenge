public class Schedule
{
    public string EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

Crea un programa o función que reciba una lista de turnos y devuelva una lista de solapes de dichos turnos (Dos turnos A y B solapan durante un periodo de tiempo si éstos comparten alguna porción del tiempo y son del mismo empleado)
