namespace Business.Mappings.Base
{
    public interface IMapDefinition
    {
        public Type SourceType { get; }
        public Type DestinationType { get; }
    }
}
