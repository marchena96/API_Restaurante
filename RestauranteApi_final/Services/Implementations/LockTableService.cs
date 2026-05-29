using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;
using RestauranteApi.DataBase;

namespace RestauranteApi.Service.Implementations
{
    public class LockTableService : ILockTableService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public LockTableService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<LockTable> GetAllList()
        {
            return _RestauranteApiDbContext.LockTables.ToList();
        }

        public LockTable? GetById(int id)
        {
            return _RestauranteApiDbContext.LockTables.Find(id);
        }

        public List<LockTable> GetByTableId(int tableId)
        {
            return _RestauranteApiDbContext.LockTables
                .Where(l => l.TableId == tableId)
                .ToList();
        }
        public LockTable Create(int tableId, string reason, DateTime from, DateTime to)
        {
            // Validar que la mesa existe
            var table = _RestauranteApiDbContext.Tables.Find(tableId);
            if (table == null) throw new Exception($"Table with id {tableId} not found.");

            // Validar que el rango de fechas es coherente
            if (from >= to) throw new Exception("La fecha de inicio debe ser anterior a la fecha de fin.");

            // Regla R9: no puede superponerse con un bloqueo activo en la misma mesa
            var overlap = _RestauranteApiDbContext.LockTables
                .Where(l => l.TableId == tableId && l.IsActive)
                .Any(l => l.From < to && l.To > from);

            if (overlap) throw new Exception("Ya existe un bloqueo activo en ese intervalo para la mesa.");

            var lockTable = new LockTable
            {
                TableId = tableId,
                Reason = reason,
                From = from,
                To = to,
                IsActive = true
            };

            _RestauranteApiDbContext.LockTables.Add(lockTable);
            _RestauranteApiDbContext.SaveChanges();
            return lockTable;
        }

        public void Deactivate(int id)
        {
            var lockTable = _RestauranteApiDbContext.LockTables.Find(id);
            if (lockTable == null) throw new Exception($"LockTable with id {id} not found.");

            lockTable.IsActive = false;
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var lockTable = _RestauranteApiDbContext.LockTables.Find(id);
            if (lockTable == null) throw new Exception($"LockTable with id {id} not found.");

            _RestauranteApiDbContext.LockTables.Remove(lockTable);
            _RestauranteApiDbContext.SaveChanges();

        }

        

        public LockTable Update(LockTable lockTable)
        {
            var result = _RestauranteApiDbContext.LockTables.Find(lockTable.Id);
            if (result == null) throw new Exception($"LockTable with id {lockTable.Id} not found.");

            result.TableId = lockTable.TableId;
            result.Reason = lockTable.Reason;
            result.From = lockTable.From;
            result.To = lockTable.To;
            result.IsActive = lockTable.IsActive;

            _RestauranteApiDbContext.SaveChanges();
            return result;
        }
    }
}