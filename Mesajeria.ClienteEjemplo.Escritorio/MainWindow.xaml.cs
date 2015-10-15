using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Messaging;
using System.Windows;
using Mensajeria.Comun;

namespace Mesajeria.ClienteEjemplo.Escritorio
{
    public partial class MainWindow : Window
    {
        private readonly Pedido _pedido;
        private readonly MessageQueue _queueRecepcion;
        private bool _escuchando;

        public MainWindow()
        {
            InitializeComponent();
            _pedido = new Pedido();
            FormaPedido.DataContext = _pedido;

            PedidosRecibidosDataGrid.DataContext = this;

            PedidosRecibidos = new ObservableCollection<Pedido>();

            var recepcionFormatName = @"trinityx\private$\pedidos";
            if (!MessageQueue.Exists(recepcionFormatName))
            {
                MessageQueue.Create(recepcionFormatName);
            }

            _queueRecepcion = new MessageQueue(recepcionFormatName);
            // Handler de recepcion de mensajes
            _queueRecepcion.ReceiveCompleted += QueueRecepcion_ReceiveCompleted;

            _escuchando = false;

        }

        private void QueueRecepcion_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            e.Message.Formatter = new XmlMessageFormatter(new String[]
            {
                "Mensajeria.Comun.Pedido,Mensajeria.Comun",
                "Mensajeria.Comun.ItemPedido,Mensajeria.Comun"
            });
            this.Dispatcher.Invoke(() =>
            {
                PedidosRecibidos.Add((Pedido) e.Message.Body);
            });

            // Volver a escuchar
            if (Escuchando) 
                _queueRecepcion.BeginReceive();

        }

        public bool Escuchando
        {
            get { return _escuchando; }
            set
            {
                _escuchando = value;
                if (_escuchando)
                {
                    // Comenzar a escuchar
                    _queueRecepcion.BeginReceive();
                }
            }
        }

        public ObservableCollection<Pedido> PedidosRecibidos { get; set; }

        public MensajeroPedidos EnvioPedidos { get; set; }

        private void BotonObtenQueues_Click(object sender, RoutedEventArgs e)
        {
            // ******  Obtener Queues

            var hostName = HostsTextBox.Text;
            if (string.IsNullOrWhiteSpace(hostName))
                return;

            var queues = new List<MessageQueue>();
            queues.AddRange(MessageQueue.GetPrivateQueuesByMachine(hostName));

            // ** Necesario ser parte de un dominio:
            // queues.AddRange(MessageQueue.GetPublicQueuesByMachine(hostName));

            QueuesDataGrid.ItemsSource = queues;

            if (queues.Count > 0)
            {
                var columnaNombre = QueuesDataGrid.Columns.First(c => c.Header.ToString() == "QueueName");
                columnaNombre.DisplayIndex = 0;
            }
        }

        private void UsarQueueBoton_Click(object sender, RoutedEventArgs e)
        {
            var pedidosQueue = QueuesDataGrid.SelectedItem as MessageQueue;
            if (pedidosQueue == null)
                return;

            PrincipalTabControl.SelectedItem = PedidosTabitem;

            EnvioPedidos = MensajeroPedidos.CreaMensajero(String.Format("{0}\\{1}",pedidosQueue.MachineName, pedidosQueue.QueueName), 2, null);
        }

        private void BotonLimpiar_Click(object sender, RoutedEventArgs e)
        {
            _pedido.Codigo = string.Empty;
            _pedido.NombreCliente = string.Empty;
            _pedido.EsUrgente = false;
            _pedido.Items.Clear();
        }

        private void BotonLlenar_Click(object sender, RoutedEventArgs e)
        {
            _pedido.Codigo = "1";
            _pedido.NombreCliente = "Lorem ipsum S.A.";
            _pedido.EsUrgente = false;
            _pedido.Items.Clear();
            _pedido.Items.Add(new ItemPedido {Cantidad = 3, CodigoProducto = "ABC001", Descripcion = "Lorem Ipsum"});
            _pedido.Items.Add(new ItemPedido
            {
                Cantidad = 9,
                CodigoProducto = "GGH045",
                Descripcion = "Gran producto no. 6"
            });
            _pedido.Items.Add(new ItemPedido
            {
                Cantidad = 1,
                CodigoProducto = "NCC1701J",
                Descripcion = "Nave intergalactica NCC-1701-J"
            });
        }

        private void BotonEnviar1_Click(object sender, RoutedEventArgs e)
        {
            EnvioPedidos.Enviar(_pedido);
        }

        private void BotonEnviar100_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < 100; i++)
            {
                EnvioPedidos.Enviar(_pedido);
            }
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            Escuchando = true;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            Escuchando = false;
        }
    }
}