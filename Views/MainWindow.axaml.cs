using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task3.ViewModels;
using Task3.Models;
using Task3.Views;
using Avalonia.VisualTree;
using Avalonia.Interactivity;

namespace Task3.Views;

public partial class MainWindow : Window
{
    private readonly List<AthleteControl> _athleteControls = new();
    private int _athleteCount = 0;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnAddAthleteClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            _athleteCount++;
            var athlete = viewModel.AddAthlete($"Спортсмен {_athleteCount}");
            
            var raceTrack = this.FindControl<Canvas>("RaceTrack");
            if (raceTrack != null)
            {
                var control = new AthleteControl
                {
                    DataContext = athlete
                };
                
                Canvas.SetLeft(control, 0);
                Canvas.SetTop(control, 70 + (_athleteControls.Count * 40));
                
                raceTrack.Children.Add(control);
                _athleteControls.Add(control);
            }
        }
    }

    private void ClearAthletes()
    {
        var raceTrack = this.FindControl<Canvas>("RaceTrack");
        if (raceTrack == null) return;

        foreach (var control in _athleteControls)
        {
            raceTrack.Children.Remove(control);
        }
        _athleteControls.Clear();
        _athleteCount = 0;
    }

    private async void OnStartCompetitionClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            // Сброс позиций и состояний всех спортсменов
            for (int i = 0; i < viewModel.Athletes.Count; i++)
            {
                var athlete = viewModel.Athletes[i];
                var control = _athleteControls[i];
                
                athlete.Reset();
                control.MoveTo(0);
                control.SetInjured(false);
                control.SetWinner(false);
            }

            viewModel.StartCompetition();
            var random = new Random();
            bool hasWinner = false;

            while (viewModel.IsCompetitionRunning)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    for (int i = 0; i < viewModel.Athletes.Count; i++)
                    {
                        var athlete = viewModel.Athletes[i];
                        var control = _athleteControls[i];
                        var currentPosition = Canvas.GetLeft(control);

                        // Если спортсмен уже на финише, пропускаем его
                        if (currentPosition >= 750)
                        {
                            continue;
                        }

                        // Если спортсмен не травмирован, двигаем его
                        if (!athlete.IsInjured)
                        {
                            var newPosition = currentPosition + random.Next(5, 15);
                            if (newPosition > 750)
                            {
                                newPosition = 750;
                            }
                            control.MoveTo(newPosition);

                            // Проверка на получение травмы
                            if (await athlete.TryGetInjured())
                            {
                                control.SetInjured(true);
                                if (DataContext is MainWindowViewModel vm)
                                {
                                    _ = Task.Run(async () =>
                                    {
                                        await vm.TreatAthlete(athlete);
                                        await Dispatcher.UIThread.InvokeAsync(() =>
                                        {
                                            athlete.IsInjured = false;
                                            control.SetInjured(false);
                                        });
                                    });
                                }
                            }

                            // Отмечаем победителя
                            if (newPosition >= 750 && !hasWinner)
                            {
                                hasWinner = true;
                                athlete.HasWon = true;
                                control.SetWinner(true);
                                viewModel.StatusMessage = $"Победитель: {athlete.Name}!";
                            }
                        }
                    }

                    // Проверяем, все ли спортсмены достигли финиша
                    bool allFinished = true;
                    for (int i = 0; i < viewModel.Athletes.Count; i++)
                    {
                        var athlete = viewModel.Athletes[i];
                        var control = _athleteControls[i];
                        var position = Canvas.GetLeft(control);
                        
                        if (position < 750)
                        {
                            allFinished = false;
                            break;
                        }
                    }
                    
                    if (allFinished)
                    {
                        viewModel.EndCompetition();
                    }
                });

                await Task.Delay(50);
            }
        }
    }
}