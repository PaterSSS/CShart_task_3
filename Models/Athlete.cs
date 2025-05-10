using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Task3.Models
{
    public class Athlete : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _hasWon;
        private bool _isInjured;
        private readonly Random _random = new();
        private double _position;
        private readonly Random _speedRandom = new();
        private readonly SemaphoreSlim _injurySemaphore = new SemaphoreSlim(1, 1);

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool HasWon
        {
            get => _hasWon;
            set
            {
                _hasWon = value;
                OnPropertyChanged();
            }
        }

        public bool IsInjured
        {
            get => _isInjured;
            set
            {
                _isInjured = value;
                OnPropertyChanged();
            }
        }

        public double Position
        {
            get => _position;
            private set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<string>? InjuryOccurred;
        public event EventHandler<string>? Healed;

        public void Move()
        {
            if (!IsInjured)
            {
                Position += _speedRandom.Next(5, 15);
            }
        }

        public void Reset()
        {
            HasWon = false;
            IsInjured = false;
            Position = 0;
        }

        public async Task<bool> TryGetInjured()
        {
            try
            {
                await _injurySemaphore.WaitAsync();
                
                if (_random.Next(100) < 5)
                {
                    IsInjured = true;
                    InjuryOccurred?.Invoke(this, $"{Name} получил травму!");
                    return true;
                }
                return false;
            }
            finally
            {
                _injurySemaphore.Release();
            }
        }

        public void Heal()
        {
            IsInjured = false;
            Healed?.Invoke(this, $"{Name} вылечен и может продолжать соревнование!");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 