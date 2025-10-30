using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;

namespace OrderProducer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IProducer<Null, string> _producer;
        private const string Topic = "new-orders";

        public OrdersController(IConfiguration config)
        {
            // 1. Configuración del Producer
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"] // ej: "localhost:9092"
            };
            // 2. Crear el productor (Null para la clave, string para el valor del mensaje)
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {
            // 3. Serializar el objeto Order a JSON string
            var message = JsonSerializer.Serialize(order);

            try
            {
                // 4. Enviar el mensaje al Topic
                var dr = await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = message });

                Console.WriteLine($"✅ Pedido {order.OrderId} enviado a Kafka. Offset: {dr.Offset}");

                return Ok(new { Message = $"Pedido {order.OrderId} aceptado y encolado." });
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"❌ Error al enviar: {e.Error.Reason}");
                return StatusCode(500, "Error al encolar el pedido en Kafka.");
            }
        }
    }
}