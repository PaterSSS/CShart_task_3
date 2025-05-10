using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Task3.Models;
using System.Collections.Generic;
using Avalonia.Threading;
using Task3.Models;
using Avalonia.Controls;
using Task3.Views;

namespace Task3.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _statusMessage = string.Empty;
        private readonly IDoctor _doctor;
        private bool _isCompetitionRunning;
        private int _athleteCount;
        private readonly Random _random = new();
        private bool _hasWinner;
        private readonly List<AthleteControl> _athleteControls = new();
        public ObservableCollection<Athlete> Athletes { get; } = new();
        

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


        public MainWindowViewModel(IDoctor doctor)
        {
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            _doctor.TreatmentCompleted += OnTreatmentCompleted;
        }

        public void RegisterAthleteControl(AthleteControl control)
        {
            _athleteControls.Add(control);
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
            _hasWinner = false;
            
            // Сброс позиций и состояний всех спортсменов
            for (int i = 0; i < Athletes.Count; i++)
            {
                var athlete = Athletes[i];
                var control = _athleteControls[i];
                
                athlete.Reset();
                control.MoveTo(0);
                control.SetInjured(false);
                control.SetWinner(false);
            }

            _ = RunCompetition();
        }

        private async Task RunCompetition()
        {
            while (IsCompetitionRunning)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var positions = new List<double>();
                    
                    for (int i = 0; i < Athletes.Count; i++)
                    {
                        var athlete = Athletes[i];
                        var control = _athleteControls[i];
                        var currentPosition = Canvas.GetLeft(control);

                        // Если спортсмен уже на финише, пропускаем его
                        if (currentPosition >= 750)
                        {
                            positions.Add(currentPosition);
                            continue;
                        }

                        // Двигаем спортсмена
                        if (!athlete.IsInjured)
                        {
                            var newPosition = currentPosition + _random.Next(5, 15);
                            if (newPosition > 750)
                            {
                                newPosition = 750;
                            }

                            control.MoveTo(newPosition);

                            // Проверка на получение травмы
                            if (await athlete.TryGetInjured())
                            {
                                control.SetInjured(true);
                                _ = Task.Run(async () =>
                                {
                                    await TreatAthlete(athlete);
                                    await Dispatcher.UIThread.InvokeAsync(() =>
                                    {
                                        athlete.IsInjured = false;
                                        control.SetInjured(false);
                                    });
                                });
                            }

                            // Отмечаем победителя
                            if (newPosition >= 750 && !_hasWinner)
                            {
                                _hasWinner = true;
                                athlete.HasWon = true;
                                control.SetWinner(true);
                                StatusMessage = $"Победитель: {athlete.Name}!";
                            }
                        }

                        positions.Add(Canvas.GetLeft(control));
                    }

                    // Проверяем завершение соревнования
                    if (CheckCompetitionFinished(positions))
                    {
                        EndCompetition();
                    }
                });

                await Task.Delay(50);
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

        private bool CheckCompetitionFinished(List<double> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (positions[i] < 750)
                {
                    return false;
                }
            }
            return true;
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