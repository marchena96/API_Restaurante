using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class TurnService : ITurnService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public TurnService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<Turn> GetAll()
        {
            return _RestauranteApiDbContext.Turns.ToList();
        }

        public Turn? GetById(int id)
        {
            return _RestauranteApiDbContext.Turns.Find(id);
        }

        public Turn Create(string name, TimeSpan startTime, TimeSpan endTime)
        {
            var turn = new Turn
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime,
                IsActive = true
            };
            _RestauranteApiDbContext.Turns.Add(turn);
            _RestauranteApiDbContext.SaveChanges();
            return turn;
        }

        public Turn Update(int id, string name, TimeSpan startTime, TimeSpan endTime)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            turn.Name = name;
            turn.StartTime = startTime;
            turn.EndTime = endTime;
            _RestauranteApiDbContext.SaveChanges();
            return turn;
        }

        public void Delete(int id)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            _RestauranteApiDbContext.Turns.Remove(turn);
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Activate(int id)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            turn.IsActive = true;
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Deactivate(int id)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            turn.IsActive = false;
            _RestauranteApiDbContext.SaveChanges();
        }

        public Turn? FindActiveTurnForTime(TimeSpan time)
        {
            return _RestauranteApiDbContext.Turns
                .Where(t => t.IsActive && t.StartTime <= time && t.EndTime >= time)
                .FirstOrDefault();
        }
    }
}