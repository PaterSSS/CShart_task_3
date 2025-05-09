using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Task3.Models;

namespace Task3.Views;

public partial class AthleteControl : UserControl
{
    private readonly Ellipse _athleteCircle;
    private readonly TextBlock _athleteNameAbove;
    private readonly Ellipse _injuryIndicator;
    private readonly Path _trophyIcon;

    public AthleteControl()
    {
        InitializeComponent();
        
        _athleteCircle = this.FindControl<Ellipse>("AthleteCircle")!;
        _athleteNameAbove = this.FindControl<TextBlock>("AthleteNameAbove")!;
        // _injuryIndicator = this.FindControl<Ellipse>("InjuryIndicator")!;
        _trophyIcon = this.FindControl<Path>("TrophyIcon")!;

        if (DataContext is Athlete athlete)
        {
            _athleteNameAbove.Text = athlete.Name;
        }
    }

    public void MoveTo(double position)
    {
        Canvas.SetLeft(this, position);
    }

    public void SetInjured(bool isInjured)
    {
        // _injuryIndicator.IsVisible = isInjured;
        _athleteCircle.Fill = isInjured ? Brushes.Red : Brushes.Blue;
    }

    public void SetWinner(bool isWinner)
    {
        _trophyIcon.IsVisible = isWinner;
        _athleteCircle.Fill = isWinner ? Brushes.Gold : Brushes.Blue;
    }
} 