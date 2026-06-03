# Arquitectura de RestauranteApi

## Resumen
RestauranteApi es una API RESTful desarrollada en ASP.NET Core (.NET 10) que gestiona zonas, secciones, mesas, turnos, clientes, reservas, bloqueos de mesas y lista de espera para un restaurante. Está organizada en capas: Controllers (API), Services (lógica de negocio) y DataBase (persistencia con Entity Framework Core InMemory).

## Diagrama lógico (texto)
API (Controllers) -> Services -> DataBase (RestauranteApiDbContext / InMemory)

## Capas y responsabilidades
- Controllers: Exponen endpoints HTTP y mapean Entities a DTOs.
- Services: Implementan la lógica de negocio y validaciones, accediendo a RestauranteApiDbContext.
- DataBase: RestauranteApiDbContext contiene DbSets, configuración de relaciones y datos iniciales (HasData) para zones, sections, tables, clients, turns, reservations, locktables, waitinglists y reservation histories.

## Configuración principal
- Program.cs configures DI para todos los servicios y registra RestauranteApiDbContext. Usa AddJsonOptions para serializar enums como string.
- Base de datos: InMemory (MyDBTest). En Program.cs se ejecuta context.Database.EnsureCreated() para aplicar seeds al inicio.

## Endpoints principales
Nota: Las rutas se indican como: VERBO PATH -> Descripción / DTO

- Clients (Controllers/ClientController.cs)
  - GET /api/clients -> Obtiene todos los clientes. Response: ClientResponseDto[]
  - GET /api/clients/{id} -> Obtiene cliente por id. Response: ClientResponseDto
  - GET /api/clients/{id}/reservations -> Reservas de un cliente. Response: ReservationResponseDto[]
  - POST /api/clients -> Crea cliente. Body: ClientCreateDto
  - PUT /api/clients/{id} -> Actualiza cliente. Body: ClientCreateDto
  - DELETE /api/clients/{id} -> Elimina cliente

- Reservations (Controllers/ReservationController.cs)
  - GET /api/reservations -> Lista reservas. Response: ReservationResponseDto[]
  - GET /api/reservations/{id} -> Obtener reserva. Response: ReservationResponseDto
  - GET /api/reservations/{id}/historical -> Historial de cambios. Response: ReservationHistoryResponseDto[]
  - POST /api/reservations -> Crear reserva. Body: ReservationCreateDto
  - PUT /api/reservations/{id} -> Actualizar reserva. Body: ReservationUpdateDto
  - PATCH /api/reservations/{id}/cancel -> Cancelar reserva
  - PATCH /api/reservations/{id}/attend -> Marcar como atendida

- Tables (Controllers/TableController.cs)
  - GET /api/tables -> Lista mesas. Response: TableResponseDto[]
  - GET /api/tables/{id} -> Obtener mesa. Response: TableResponseDto
  - GET /api/tables/section/{sectionId} -> Mesas por seccion
  - GET /api/tables/zone/{zoneId} -> Mesas por zona
  - GET /api/tables/available?date=&time=&capacity=&zoneId= -> Mesas disponibles (filtro)
  - GET /api/tables/availability/turn/{turnId}?date=&zoneId= -> Disponibilidad por turno
  - GET /api/tables/{id}/availability?date=&time= -> Disponibilidad de mesa
  - POST /api/tables -> Crear mesa. Body: TableCreateDto
  - PUT /api/tables/{id} -> Actualizar mesa. Body: TableCreateDto
  - PATCH /api/tables/{id}/deactivate -> Desactivar mesa
  - DELETE /api/tables/{id} -> Eliminar mesa

- Sections (Controllers/SectionController.cs)
  - GET /api/sections -> Lista secciones
  - GET /api/sections/{id} -> Obtener seccion
  - GET /api/sections/zone/{zoneId} -> Secciones por zona
  - POST /api/sections -> Crear seccion. Body: SectionCreateDto
  - PUT /api/sections/{id} -> Actualizar seccion. Body: SectionCreateDto
  - DELETE /api/sections/{id} -> Eliminar seccion

- Zones (Controllers/ZoneController.cs)
  - GET /api/zones -> Lista zonas
  - GET /api/zones/{id} -> Obtener zona
  - POST /api/zones -> Crear zona. Body: ZoneCreateDto
  - PUT /api/zones/{id} -> Actualizar zona. Body: ZoneCreateDto
  - DELETE /api/zones/{id} -> Eliminar zona

- Turns (Controllers/TurnController.cs)
  - GET /api/turns -> Lista turnos
  - GET /api/turns/{id} -> Obtener turno
  - POST /api/turns -> Crear turno. Body: TurnCreateDto
  - PUT /api/turns/{id} -> Actualizar turno. Body: TurnCreateDto
  - PATCH /api/turns/{id}/activate -> Activar turno
  - PATCH /api/turns/{id}/deactivate -> Desactivar turno
  - DELETE /api/turns/{id} -> Eliminar turno

- LockTables (Controllers/LockTableController.cs)
  - GET /api/lockstable -> Lista bloqueos
  - GET /api/lockstable/{id} -> Obtener bloqueo
  - GET /api/lockstable/table/{tableId} -> Bloqueos por mesa
  - POST /api/lockstable -> Crear bloqueo. Body: LockTableCreateDto
  - PATCH /api/lockstable/{id}/cancel -> Cancelar bloqueo (desactivar)
  - DELETE /api/lockstable/{id} -> Eliminar bloqueo

- WaitingList (Controllers/WaitingListController.cs)
  - GET /api/waitinglist -> Lista entradas
  - GET /api/waitinglist/{id} -> Obtener entrada
  - GET /api/waitinglist/pending -> Entradas pendientes
  - POST /api/waitinglist -> Crear entrada. Body: WaitingListCreateDto
  - PATCH /api/waitinglist/{id}/assign -> Asignar tabla (Body: WaitingListAssignDto)
  - PATCH /api/waitinglist/{id}/cancel -> Cancelar entrada

## Modelos principales (Entities)
- Zone: Id, Name, ICollection<Section> Sections
- Section: Id, Name, ZoneId, Zone, ICollection<Table> Tables
- Table: Id, Number, Capacity, IsActive, SectionId, Section, ICollection<Reservation>, ICollection<LockTable>
- Turn: Id, Name, StartTime, EndTime, IsActive, ICollection<Reservation>
- Client: Id, Name, LastName, Phone, ICollection<Reservation>, ICollection<WaitingList>
- Reservation: Id, Date (DateOnly), ReservationTime (TimeSpan), GuestCount, CreatedAt, ClientId, TableId, TurnId, Status (enum), navigation props, ICollection<ReservationHistory>
- ReservationHistory: Id, PrevState, NewState, ChangedAt, ReservationId
- LockTable: Id, Reason, From, To, IsActive, TableId
- WaitingList: Id, ReqDate, DesiredDay, DesiredTime, GuestCount, PreferZone, Status, ClientId

## DTOs (resumen)
- ClientCreateDto, ClientResponseDto
- ReservationCreateDto, ReservationUpdateDto, ReservationResponseDto, ReservationHistoryResponseDto
- TableCreateDto, TableResponseDto
- SectionCreateDto, SectionResponseDto
- ZoneCreateDto, ZoneResponseDto
- TurnCreateDto, TurnResponseDto
- LockTable DTOs
- WaitingList DTOs

## Reglas de negocio destacadas
- Al crear una reserva: el cliente y la mesa deben existir; la mesa debe estar activa; debe existir un turno activo para la hora; la mesa debe tener capacidad suficiente; no debe existir otra reserva activa para la misma mesa, fecha y turno; la mesa no debe estar bloqueada.
- Al actualizar reserva: sólo reservas con estado Active pueden modificarse; se valida conflicto con otras reservas.
- Al cancelar o marcar como atendida: sólo reservas Active pueden cambiarse y se registra un ReservationHistory.
- Las relaciones entre entidades usan DeleteBehavior.Restrict en la mayoría para evitar borrados en cascada, salvo en algunos historiales y bloqueos donde sí aplica Cascade.

## Ejecución local
1. Abrir la solución en Visual Studio 2022/2026 o ejecutar dotnet build.
2. Ejecutar la aplicación (F5). La API usa InMemory y carga datos de ejemplo automáticamente.
3. Probar endpoints via Postman o el archivo RestauranteApi.http incluido.

## Extensiones recomendadas
- Añadir Swagger (AddSwaggerGen + UseSwagger) para documentación OpenAPI automática.
- Sustituir InMemory por SQL Server/Postgres en production y usar migraciones.
- Añadir autenticación/autorization si se requiere control de acceso.

## Observaciones
- No se encontró código de autenticación.
- Program.cs registra servicios manualmente y llama a EnsureCreated para InMemory DB.

