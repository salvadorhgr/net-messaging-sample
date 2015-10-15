using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mensajeria.Comun.Annotations;

namespace Mensajeria.Comun
{
    public class ItemPedido: INotifyPropertyChanged
    {
        #region Privados
        private string _codigoProducto;
        private string _descripcion;
        private decimal _cantidad; 
        #endregion

        public decimal Cantidad
        {
            get { return _cantidad; }
            set
            {
                if (value == _cantidad) return;
                _cantidad = value;
                OnPropertyChanged();
            }
        }

        public string Descripcion
        {
            get { return _descripcion; }
            set
            {
                if (value == _descripcion) return;
                _descripcion = value;
                OnPropertyChanged();
            }
        }

        public string CodigoProducto
        {
            get { return _codigoProducto; }
            set
            {
                if (value == _codigoProducto) return;
                _codigoProducto = value;
                OnPropertyChanged();
            }
        }

        #region NotifyPropertyChanges
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}