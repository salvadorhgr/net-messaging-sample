using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mensajeria.Comun.Annotations;

namespace Mensajeria.Comun
{
    public class Pedido: INotifyPropertyChanged
    {

        #region Privados

        private string _codigo;
        private string _nombreCliente;
        private DateTime _fecha;
        private ObservableCollection<ItemPedido> _items;
        private bool _esUrgente;

        #endregion

        public Pedido()
        {
            Items = new ObservableCollection<ItemPedido>();
        }

        public string Codigo
        {
            get { return _codigo; }
            set
            {
                if (value == _codigo) return;
                _codigo = value;
                OnPropertyChanged();
            }
        }

        public string NombreCliente
        {
            get { return _nombreCliente; }
            set
            {
                if (value == _nombreCliente) return;
                _nombreCliente = value;
                OnPropertyChanged();
            }
        }

        public DateTime Fecha
        {
            get { return _fecha; }
            set
            {
                if (value.Equals(_fecha)) return;
                _fecha = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ItemPedido> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public bool EsUrgente
        {
            get { return _esUrgente; }
            set
            {
                if (value == _esUrgente) return;
                _esUrgente = value;
                OnPropertyChanged();
            }
        }

        #region NotifyProperty

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}