using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Task3.Models;

namespace Task3.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _statusMessage = string.Empty;
        private readonly IDoctor _doctor;
        private bool _isCompetitionRunning;
        private int _athleteCount;

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsCompetitionRunning
        {
            get => _isCompetitionRunning;
            set
            {
                _isCompetitionRunning = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Athlete> Athletes { get; } = new();

        public MainWindowViewModel(IDoctor doctor)
        {
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            _doctor.TreatmentCompleted += OnTreatmentCompleted;
        }

        public Athlete AddAthlete(string name)
        {
            _athleteCount++;
            var athlete = new Athlete { Name = name };
            athlete.InjuryOccurred += OnAthleteEvent;
            athlete.Healed += OnAthleteEvent;
            Athletes.Add(athlete);
            return athlete;
        }

        public void StartCompetition()
        {
            if (IsCompetitionRunning || Athletes.Count == 0) return;

            IsCompetitionRunning = true;
            foreach (var athlete in Athletes)
            {
                athlete.Reset();
            }
        }

        public void EndCompetition()
        {
            IsCompetitionRunning = false;
        }

        public async Task TreatAthlete(Athlete athlete)
        {
            await _doctor.TreatAthlete(athlete);
        }

        private void OnAthleteEvent(object? sender, string message)
        {
            StatusMessage = message;
        }

        private void OnTreatmentCompleted(object? sender, string message)
        {
            StatusMessage = message;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}