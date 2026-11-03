using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using StudentManager.Data;
using StudentManager.Models;

namespace StudentManager.ViewModels;

public class StudentViewModel : INotifyPropertyChanged
{
    private readonly StudentRepository _repository;
    public ObservableCollection<Student> Students { get; } = new();
    public ObservableCollection<StudyProgram> AvailablePrograms { get; } = new();
    public List<string> GenderOptions { get; } = new() { "Laki-laki", "Perempuan" };

    private StudyProgram? _selectedProgram;
    public StudyProgram? SelectedProgram
    {
        get => _selectedProgram;
        set
        {
            _selectedProgram = value;
            if (_selectedStudent != null && value != null)
            {
                _selectedStudent.ProgramId = value.ProgramId;
                _selectedStudent.Jurusan = value.ProgramName;
            }
            OnPropertyChanged();
        }
    }

    private Student? _selectedRow;
    public Student? SelectedRow
    {
        get => _selectedRow;
        set
        {
            if (ReferenceEquals(_selectedRow, value)) return;
            _selectedRow = value;
            SelectedStudent = value;
            OnPropertyChanged();
        }
    }

    private Student? _selectedStudent;
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            if (value != null)
            {
                _selectedStudent = new Student
                {
                    Id = value.Id,
                    NIM = value.NIM,
                    Nama = value.Nama,
                    ProgramId = value.ProgramId,
                    Jurusan = value.Jurusan,
                    Gender = value.Gender,
                    Email = value.Email,
                    BirthDate = value.BirthDate,
                    Address = value.Address,
                    PhoneNumber = value.PhoneNumber
                };
                SelectedProgram = AvailablePrograms.FirstOrDefault(p => p.ProgramId == value.ProgramId);
            }
            else
            {
                _selectedStudent = null;
                SelectedProgram = null;
            }
            OnPropertyChanged();
        }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public int TotalStudents => Students.Count;
    public int TotalInformatika => Students.Count(x => x.Jurusan == "Teknik Informatika");
    public int TotalSistemInformasi => Students.Count(x => x.Jurusan == "Sistem Informasi");
    public int TotalLakiLaki => Students.Count(x => x.Gender == "Laki-laki");
    public int TotalPerempuan => Students.Count(x => x.Gender == "Perempuan");

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SearchCommand { get; }

    public StudentViewModel()
    {
        _repository = new StudentRepository();

        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete);
        ResetCommand = new RelayCommand(Reset);
        SearchCommand = new RelayCommand(Search);

        LoadPrograms();
        LoadData();
        Reset();
    }

    private void LoadPrograms()
    {
        AvailablePrograms.Clear();
        foreach (var p in _repository.GetPrograms())
            AvailablePrograms.Add(p);
    }

    private void LoadData()
    {
        Students.Clear();
        foreach (var student in _repository.GetAll())
            Students.Add(student);

        RefreshStatistics();
    }

    private void Save()
    {
        if (SelectedStudent == null) return;
        if (SelectedProgram != null)
        {
            SelectedStudent.ProgramId = SelectedProgram.ProgramId;
            SelectedStudent.Jurusan = SelectedProgram.ProgramName;
        }

        if (SelectedStudent.Id == 0)
            _repository.Insert(SelectedStudent);
        else
            _repository.Update(SelectedStudent);

        LoadData();
        Reset();
    }

    private void Delete()
    {
        if (SelectedStudent == null || SelectedStudent.Id == 0) return;
        _repository.Delete(SelectedStudent.Id);
        LoadData();
        Reset();
    }

    private void Search()
    {
        var result = string.IsNullOrWhiteSpace(SearchText)
            ? _repository.GetAll()
            : _repository.Search(SearchText);

        Students.Clear();
        foreach (var student in result)
            Students.Add(student);

        RefreshStatistics();
    }

    private void Reset()
    {
        SelectedRow = null;
        SelectedStudent = new Student
        {
            Gender = "Laki-laki"
        };
        SelectedProgram = AvailablePrograms.FirstOrDefault();
        if (SelectedProgram != null)
        {
            SelectedStudent.ProgramId = SelectedProgram.ProgramId;
            SelectedStudent.Jurusan = SelectedProgram.ProgramName;
        }
    }

    private void RefreshStatistics()
    {
        OnPropertyChanged(nameof(TotalStudents));
        OnPropertyChanged(nameof(TotalInformatika));
        OnPropertyChanged(nameof(TotalSistemInformasi));
        OnPropertyChanged(nameof(TotalLakiLaki));
        OnPropertyChanged(nameof(TotalPerempuan));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
