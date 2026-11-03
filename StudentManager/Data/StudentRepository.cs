using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using StudentManager.Models;

namespace StudentManager.Data;

public class StudentRepository
{
    private readonly string connectionString =
        Environment.GetEnvironmentVariable("STUDENT_MANAGER_CONNECTION_STRING")
        ?? throw new InvalidOperationException("Set STUDENT_MANAGER_CONNECTION_STRING before starting the application.");

    public List<StudyProgram> GetPrograms()
    {
        var list = new List<StudyProgram>();
        using SqlConnection conn = new(connectionString);
        string sql = "SELECT ProgramId, ProgramCode, ProgramName FROM Programs WHERE IsActive = 1 ORDER BY ProgramId";
        using SqlCommand cmd = new(sql, conn);
        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new StudyProgram
            {
                ProgramId = Convert.ToInt32(reader["ProgramId"]),
                ProgramCode = reader["ProgramCode"].ToString()!,
                ProgramName = reader["ProgramName"].ToString()!
            });
        }
        return list;
    }

    public List<Student> GetAll()
    {
        var students = new List<Student>();
        using SqlConnection conn = new(connectionString);
        string sql = @"SELECT s.StudentId AS Id, s.NIM, s.Name AS Nama, s.ProgramId,
                              p.ProgramName AS Jurusan, s.Gender, s.Email,
                              s.BirthDate, s.Address, s.PhoneNumber
                       FROM Students s
                       INNER JOIN Programs p ON s.ProgramId = p.ProgramId
                       ORDER BY s.StudentId DESC";
        using SqlCommand cmd = new(sql, conn);
        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            students.Add(MapStudent(reader));
        }
        return students;
    }

    public void Insert(Student s)
    {
        using SqlConnection conn = new(connectionString);
        string sql = @"INSERT INTO Students (NIM, Name, ProgramId, Gender, Email, BirthDate, Address, PhoneNumber)
                       VALUES (@NIM, @Nama, @ProgramId, @Gender, @Email, @BirthDate, @Address, @PhoneNumber)";
        using SqlCommand cmd = new(sql, conn);
        AddParameters(cmd, s);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Update(Student s)
    {
        using SqlConnection conn = new(connectionString);
        string sql = @"UPDATE Students
                       SET NIM = @NIM, Name = @Nama, ProgramId = @ProgramId,
                           Gender = @Gender, Email = @Email, BirthDate = @BirthDate,
                           Address = @Address, PhoneNumber = @PhoneNumber,
                           UpdatedAt = SYSDATETIME()
                       WHERE StudentId = @Id";
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@Id", s.Id);
        AddParameters(cmd, s);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlConnection conn = new(connectionString);
        using SqlCommand cmd = new("DELETE FROM Students WHERE StudentId = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public List<Student> Search(string keyword)
    {
        var students = new List<Student>();
        using SqlConnection conn = new(connectionString);
        string sql = @"SELECT s.StudentId AS Id, s.NIM, s.Name AS Nama, s.ProgramId,
                              p.ProgramName AS Jurusan, s.Gender, s.Email,
                              s.BirthDate, s.Address, s.PhoneNumber
                       FROM Students s
                       INNER JOIN Programs p ON s.ProgramId = p.ProgramId
                       WHERE s.NIM LIKE @Keyword OR s.Name LIKE @Keyword OR p.ProgramName LIKE @Keyword
                       ORDER BY s.StudentId DESC";
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            students.Add(MapStudent(reader));
        }
        return students;
    }

    private static Student MapStudent(SqlDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["Id"]),
        NIM = reader["NIM"].ToString()!,
        Nama = reader["Nama"].ToString()!,
        ProgramId = Convert.ToInt32(reader["ProgramId"]),
        Jurusan = reader["Jurusan"].ToString()!,
        Gender = reader["Gender"] is DBNull ? string.Empty : reader["Gender"].ToString()!,
        Email = reader["Email"] is DBNull ? string.Empty : reader["Email"].ToString()!,
        BirthDate = reader["BirthDate"] is DBNull ? null : Convert.ToDateTime(reader["BirthDate"]),
        Address = reader["Address"] is DBNull ? string.Empty : reader["Address"].ToString()!,
        PhoneNumber = reader["PhoneNumber"] is DBNull ? string.Empty : reader["PhoneNumber"].ToString()!
    };

    private static void AddParameters(SqlCommand cmd, Student s)
    {
        cmd.Parameters.AddWithValue("@NIM", s.NIM);
        cmd.Parameters.AddWithValue("@Nama", s.Nama);
        cmd.Parameters.AddWithValue("@ProgramId", s.ProgramId);
        cmd.Parameters.AddWithValue("@Gender", (object?)s.Gender ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Email", (object?)s.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BirthDate", (object?)s.BirthDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Address", (object?)s.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)s.PhoneNumber ?? DBNull.Value);
    }
}
