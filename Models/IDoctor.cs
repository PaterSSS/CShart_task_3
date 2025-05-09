using System;
using System.Threading.Tasks;

namespace Task3.Models
{
    public interface IDoctor
    {
        string Name { get; }
        event EventHandler<string>? TreatmentCompleted;
        Task TreatAthlete(Athlete athlete);
    }
} 