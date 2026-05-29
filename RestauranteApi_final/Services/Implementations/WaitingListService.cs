using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;
using RestauranteApi.DataBase;
namespace RestauranteApi.Service.Implementations
{
    public class WaitingListService : IWaitingListService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;
        private readonly IReservationService _reservationService;

        public WaitingListService(RestauranteApiDbContext RestauranteApiDbContext, IReservationService reservationService)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
            _reservationService = reservationService;
        }

        public List<WaitingList> GetAll()
        {
            return _RestauranteApiDbContext.WaitingLists.ToList();
        }

        public WaitingList? GetById(int id)
        {
            return _RestauranteApiDbContext.WaitingLists.Find(id);
        }

        public List<WaitingList> GetPending()
        {
            return _RestauranteApiDbContext.WaitingLists
                .Where(w => w.Status == "Pending")
                .ToList();
        }

        public WaitingList Create(int clientId, DateOnly desiredDay, TimeSpan desiredTime, int guestCount, string? preferZone = null)
        {
            // Validar que el cliente existe
            var client = _RestauranteApiDbContext.Clients.Find(clientId);
            if (client == null) throw new Exception($"Cliente con id {clientId} no encontrado.");

            var entry = new WaitingList
            {
                ClientId = clientId,
                ReqDate = DateTime.Now,
                DesiredDay = desiredDay,
                DesiredTime = desiredTime,
                GuestCount = guestCount,
                PreferZone = preferZone ?? "",
                Status = "Pending"
            };

            _RestauranteApiDbContext.WaitingLists.Add(entry);
            _RestauranteApiDbContext.SaveChanges();
            return entry;
        }

        // R7: Convierte una entrada de la lista de espera en reserva confirmada
        public Reservation AssignTable(int waitingListId, int tableId)
        {
            var entry = _RestauranteApiDbContext.WaitingLists.Find(waitingListId);
            if (entry == null) throw new Exception($"Entrada de lista de espera con id {waitingListId} no encontrada.");
            if (entry.Status != "Pending") throw new Exception("Esta entrada ya fue asignada o cancelada.");

            // Usar ReservationService para crear la reserva con todas sus validaciones
            var reservation = _reservationService.Create(
                entry.ClientId,
                tableId,
                entry.DesiredDay,
                entry.DesiredTime,
                entry.GuestCount
            );

            // Marcar entrada como asignada
            entry.Status = "Assigned";
            _RestauranteApiDbContext.SaveChanges();

            var fullReservation = _reservationService.GetById(reservation.Id);
            return fullReservation!;
        }

        public void Cancel(int id)
        {
            var entry = _RestauranteApiDbContext.WaitingLists.Find(id);
            if (entry == null) throw new Exception($"Entrada de lista de espera con id {id} no encontrada.");
            if (entry.Status != "Pending") throw new Exception("Solo se puede cancelar una entrada en estado Pending.");

            entry.Status = "Cancelled";
            _RestauranteApiDbContext.SaveChanges();
        }
    }
}
