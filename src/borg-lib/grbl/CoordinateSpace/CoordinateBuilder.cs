using Borg.Machine;

namespace Yuni.CoordinateSpace
{
    public class CoordinateBuilder {

        private Coordinate _model = new Coordinate();

        public Coordinate Build() {
            return _model;
        }

        public CoordinateBuilder WithCoord(string axis, double val) {
            if (axis == "X")
                return WithX(val);
            if (axis == "Y")
                return WithY(val);
            if (axis == "Z")
                return WithZ(val);
            if (axis == "A")
                return WithA(val);
            if (axis == "B")
                return WithB(val);
            if (axis == "E")
                return WithE(val);

            throw new InvalidAxisException($"Axis {axis} is not valid");
        }
        
        public CoordinateBuilder WithX(double x) {
            _model.X = x;
            return this;
        }
        public CoordinateBuilder WithY(double y) {
            _model.Y = y;
            return this;
        }
        public CoordinateBuilder WithZ(double z) {
            _model.Z = z;
            return this;
        }
        public CoordinateBuilder WithA(double a) {
            _model.A = a;
            return this;
        }
        public CoordinateBuilder WithB(double b) {
            _model.B = b;
            return this;
        }
        public CoordinateBuilder WithE(double e) {
            _model.E = e;
            return this;
        }

    }
}