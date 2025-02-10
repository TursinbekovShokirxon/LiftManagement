using LiftManagement.HelloBlazorServer.Db;
using LiftManagement.HelloBlazorServer.Models.Lift;
using Microsoft.EntityFrameworkCore;

namespace Samples.HelloBlazorServer.Services
{
    public class LiftManagementService : IComputeService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public LiftManagementService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }


        [ComputeMethod(AutoInvalidationDelay = 0.1)]
        public virtual async Task<ElevatorQueue[]> GetQueue(CancellationToken cancellationToken = default)
        {
            using (var scope = _scopeFactory.CreateScope()) {
                var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var queues = _db.Queues.Select(x => x).ToArray();

                return queues;
            }
        }
        [ComputeMethod(AutoInvalidationDelay = 0.1)]
        public virtual async Task<Elevator> GetStatus(CancellationToken cancellationToken = default)
        {
            using (var scope = _scopeFactory.CreateScope()) {
                var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var elevator = await _db.Elevator.FirstOrDefaultAsync(cancellationToken);

                if (elevator == null) {
                    elevator = new Elevator {
                        CurrentFloor = 1,
                        Status = "Стоит",
                        Id = 1
                    };
                    _db.Elevator.Add(elevator);
                    await _db.SaveChangesAsync(cancellationToken);
                }

                // Проверяем очередь вызовов лифта
                var nextRequest = await _db.Queues
                    .OrderBy(q => q.RequestedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (nextRequest != null) {
                    if (elevator.CurrentFloor < nextRequest.CurrentFloor) {
                        elevator.Status = "Поднимается";
                    }
                    else if (elevator.CurrentFloor > nextRequest.CurrentFloor) {
                        elevator.Status = "Спускается";
                    }
                    else {
                        elevator.Status = "Ожидает";
                    }
                }
                else {
                    elevator.Status = "Стоит";
                }

                await _db.SaveChangesAsync(cancellationToken);
                return elevator;
            }
        }


        [ComputeMethod(AutoInvalidationDelay = 5)]
        public virtual async Task<int> ElevatorMovement(CancellationToken cancellationToken = default)
        {
            using (var scope = _scopeFactory.CreateAsyncScope()) {
                var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var queues = _db.Queues.OrderBy(x => x.RequestedAt).ToList();

                var elevator = _db.Elevator.FirstOrDefault();

                if (elevator == null | queues == null)
                    return 0;

                int elevatorCurrentStage = elevator.CurrentFloor;
                ElevatorQueue currentRequest = null;

                foreach (var request in queues) {
                    if (elevatorCurrentStage == request.CurrentFloor) {
                        // Лифт уже на текущем этаже, начинаем с этого запроса
                        currentRequest = request;
                        break;
                    }
                }
                if (currentRequest == null) {

                    currentRequest = queues.FirstOrDefault();
                }

                if (currentRequest != null) {
                    int elevatorTargetStage = currentRequest.CurrentFloor;

                    // Лифт двигается к целевому этажу
                    if (elevatorCurrentStage != elevatorTargetStage) {
                        if (elevatorCurrentStage < elevatorTargetStage) {
                            elevatorCurrentStage++;
                        }
                        else if (elevatorCurrentStage > elevatorTargetStage) {
                            elevatorCurrentStage--;
                        }
                        elevator.CurrentFloor = elevatorCurrentStage;
                        await _db.SaveChangesAsync();
                    }


                    if (elevator.CurrentFloor == elevatorTargetStage) {
                        // Удаляем выполненный запрос
                        _db.Queues.Remove(currentRequest);
                        await _db.SaveChangesAsync();

                        // Проверяем, есть ли еще запросы
                        var nextRequest = queues.FirstOrDefault(q => q.CurrentFloor == elevator.CurrentFloor);


                        if (nextRequest != null) {
                            currentRequest = nextRequest;
                        }
                    }
                }
                return elevatorCurrentStage;
            }
        }

        public async Task CallElevator(int currentFloor, int requestedFloor)
        {

            var elevator = await GetStatus();

            if (elevator.CurrentFloor != currentFloor) {
                using (var scope = _scopeFactory.CreateScope()) {
                    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    _db.Queues.Add(new ElevatorQueue() {
                        RequestedAt = DateTime.Now,
                        CurrentFloor = currentFloor,
                    });
                    await _db.SaveChangesAsync();
                }
            }
            return;
        }


    }
}