namespace Furball.Engine.Engine.Helpers {
    public class ValueContainer<pValueType> {
        public pValueType Value;

        public ValueContainer(pValueType value) {
            this.Value = value;
        }

        public static implicit operator pValueType(ValueContainer<pValueType> valueContainer) => valueContainer.Value;
        public static implicit operator ValueContainer<pValueType>(pValueType value) => new ValueContainer<pValueType>(value);
    }
}
