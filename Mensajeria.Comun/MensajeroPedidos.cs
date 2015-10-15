using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace Mensajeria.Comun
{
    public class MensajeroPedidos
    {

        private readonly MessageQueue _queue;
        private readonly MessageQueue _queueMonitoreo;

        public static MensajeroPedidos CreaMensajero(string rutaQueue, int timeoutMinutos, string rutaQueueMonitoreo = null)
        {
            if (MessageQueue.Exists(rutaQueue))
            {
                // Valida
                MessageQueue queue = new MessageQueue(rutaQueue, QueueAccessMode.Send);

                // Valida cola de monitoreo
                MessageQueue queueMonitoreo = null;
                if (rutaQueueMonitoreo != null)
                {
                    if (MessageQueue.Exists(rutaQueueMonitoreo))
                    {
                        queueMonitoreo = new MessageQueue(rutaQueueMonitoreo);
                    }
                }

                // Crea y Configura
                var mensajero = new MensajeroPedidos(queue, queueMonitoreo)
                {
                    TimeoutMensajes = TimeSpan.FromMinutes(timeoutMinutos)
                };

                // Regresa
                return mensajero;
            }
            else
            {
                throw new Exception("La cola de recepción de pedidos no se encontró");
            }
        }

        // Constructor
        private MensajeroPedidos(MessageQueue queue, MessageQueue queueMonitoreo)
        {
            _queue = queue;
            _queueMonitoreo = queueMonitoreo;
        }

        // Propiedades del mensajero
        public TimeSpan TimeoutMensajes { get; set; }

        public void Enviar(Pedido pedido)
        {
            // Crea y configura mensaje
            Message mensaje = new Message(pedido, new XmlMessageFormatter())
            {
                // Timeout en caso de no encontrase el servidor
                TimeToBeReceived = TimeoutMensajes,
                // QoS - este uso es solo demostrativo
                Priority = pedido.EsUrgente ? MessagePriority.Highest : MessagePriority.Normal,
                // Etiquetar mensaje:
                Label = string.Format("Pedido{0}_{1}", pedido.EsUrgente ? "Urgente" : string.Empty, pedido.Codigo)
            };

            // Definir cola de monitore si se definió alguna
            if (_queueMonitoreo != null)
                mensaje.AdministrationQueue = _queueMonitoreo;

            // Envía
            _queue.Send(mensaje);
        }

    }
}
