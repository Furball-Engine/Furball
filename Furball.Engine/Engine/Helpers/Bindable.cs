using System;

namespace Furball.Engine.Engine.Helpers {
    public class Bindable <pT> : IDisposable {
        public event EventHandler<pT> OnChange;

        private pT _value;

        public pT Value {
            get => this._value;
            set {
                if (this._value is not null && this._value.Equals(value)) return;
                
                this._value = value;
                this.OnChange?.Invoke(this, value);
            }
        }

        public Bindable(pT value) => this._value = value;

        public void Dispose() {
            this.OnChange = null;
        }

        public static implicit operator pT(Bindable<pT> bindable) => bindable.Value;
    }
}
