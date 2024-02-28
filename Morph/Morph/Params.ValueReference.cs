namespace Morph.Params
{
    public class ValueReference : ValueObject
    {
        public ValueReference(string typeName, int id)
          : base(typeName)
        {
            _id = id;
        }

        private readonly int _id;
        public int ID
        {
            get => _id;
        }
    }
}