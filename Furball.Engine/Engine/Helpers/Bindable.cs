using System;

namespace Furball.Engine.Engine.Helpers {
    public class Bindable<T> : IDisposable {
        public event EventHandler<T> OnChange;

        private T _value;
        public T Value {
            get => this._value;
            set {
                if (this._value.Equals(value)) return;
                
                this._value = value;
                this.OnChange?.Invoke(this, value);
            }
        }
        
        public Bindable(T value) {
            this._value = value;
        }

        public void Dispose() {
            this.OnChange = null;
        }
        
        public static implicit operator T(Bindable<T> bindable) {
            return bindable.Value;
        }
    }
}
