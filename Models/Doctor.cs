using System;
using System.Threading.Tasks;

namespace Task3.Models
{
    public class Doctor : IDoctor
    {
        public string Name { get; }
        public event EventHandler<string>? TreatmentCompleted;

        public Doctor(string name)
        {
            Name = name;
        }

        public async Task TreatAthlete(Athlete athlete)
        {
            if (!athlete.IsInjured) return;
            await Task.Delay(1000);
            athlete.Heal();
            TreatmentCompleted?.Invoke(this, $"{Name} вылечил {athlete.Name}!");
        }
    }
} 