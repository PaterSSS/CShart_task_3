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
                viewModel.RegisterAthleteControl(control);
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

    private void OnStartCompetitionClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.StartCompetition();
        }
    }
}