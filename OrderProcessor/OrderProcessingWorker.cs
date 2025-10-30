using Confluent.Kafka;
using Models;
using System.Text.Json;

namespace OrderProcessor
{
    public class OrderProcessingWorker : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private const string Topic = "new-orders";

        public OrderProcessingWorker(IConfiguration configuration)
        {
            // 1. Configuración del Consumer
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = "order-processors-group", // Grupo para balanceo de carga
                AutoOffsetReset = AutoOffsetReset.Earliest // Empieza a leer desde el inicio si es nuevo
            };

            // 2. Crear el consumidor
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(Topic);
            Console.WriteLine($"👂 Consumer escuchando el topic '{Topic}'...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 3. Esperar y consumir el mensaje
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var orderJson = consumeResult.Message.Value;

                    // 4. Deserializar y procesar
                    var order = JsonSerializer.Deserialize<Order>(orderJson);

                    Console.WriteLine($"\n--- Pedido Recibido ---");
                    Console.WriteLine($"ID: {order.OrderId}");
                    Console.WriteLine($"Artículo: {order.ItemName}");
                    Console.WriteLine($"Procesando...");

                    // Simulación de trabajo
                    Task.Delay(1000).Wait();
                    Console.WriteLine($"--- Pedido {order.OrderId} PROCESADO con éxito ---");

                    // 5. Commit (opcional, si no se usa auto-commit)
                    // _consumer.Commit(consumeResult);
                }
                catch (ConsumeException e) when (e.Error.IsFatal)
                {
                    // Manejo de errores fatales
                    Console.WriteLine($"❌ Error fatal del consumidor: {e.Error.Reason}");
                    break;
                }
                catch (OperationCanceledException)
                {
                    // El servicio se está deteniendo
                    break;
                }
            }

            _consumer.Close();
            return Task.CompletedTask;
        }
    }
}