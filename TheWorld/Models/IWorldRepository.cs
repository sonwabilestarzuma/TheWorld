using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        Trip GetTripByName(string tripName);
        void AddTrip(Trip trip);
        Task<bool> SaveChangesAsync();
        void AddStop(string tripName, Stop newStop);
    }
}
