using Borg.Machine;

namespace Yuni.CoordinateSpace
{
    public class CoordinateTranslatorFactory {
        public ICoordinateTranslator GetTranslator(Coordinate offset) {
            return new CoordinateTranslator(offset);
        }
    }
    
    public interface ICoordinateTranslator
    {
        Coordinate Translate(Coordinate originalCoordinate) ;
    }
    public class CoordinateTranslator : ICoordinateTranslator {
        private readonly Coordinate _offset;

        public CoordinateTranslator(Coordinate offset) {
            _offset = offset;
        }

        public Coordinate Translate(Coordinate originalCoordinate) {
            var coord = new Coordinate();
            coord.X = _offset.X.Value + originalCoordinate.X.Value;
            coord.Y = _offset.Y.Value + originalCoordinate.Y.Value;
            coord.Z = _offset.Z.Value + originalCoordinate.Z.Value;
            return coord;
        }
    }
}