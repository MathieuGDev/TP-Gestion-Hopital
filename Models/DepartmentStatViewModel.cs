namespace tp_hospital.Models;

public class DepartmentStatViewModel
{
    public int    DepartmentId       { get; set; }
    public string DepartmentName     { get; set; } = string.Empty;
    public string HeadDoctorName     { get; set; } = string.Empty;
    public int    DoctorCount        { get; set; }
    public int    ConsultationCount  { get; set; }
    public int    UpcomingCount      { get; set; }
}
