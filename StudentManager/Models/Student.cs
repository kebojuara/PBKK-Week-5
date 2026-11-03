using System;

namespace StudentManager.Models;

public class Student
{
    public int Id { get; set; }
    public string NIM { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public int ProgramId { get; set; }
    public string Jurusan { get; set; } = string.Empty; // Menampung ProgramName dari JOIN
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
