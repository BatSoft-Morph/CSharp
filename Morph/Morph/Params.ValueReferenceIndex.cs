namespace Morph.Params
{
    public class ValueReferenceIndex : ValueReference
    {
        public ValueReferenceIndex(string typeName, int id, Value index)
          : base(typeName, id)
        {
            _index = index;
        }

        private readonly Value _index;
        public Value Index
        {
            get => _index;
        }
    }
}