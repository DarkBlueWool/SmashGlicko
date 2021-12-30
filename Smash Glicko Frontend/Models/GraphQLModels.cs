namespace Smash_Glicko_Frontend.Models.GraphQL.EventGet
{
    public class EventResponseType
    {
        public EventType Event { get; set; }
    }

    public class EventType
    {
        public string Name { get; set; }
        
        public uint Id { get; set; }
        public SetType Sets { get; set; }
    }

    public class SetType
    {
        public List<NodeType> Nodes { get; set; }
    }

    public class NodeType
    {
        public string DisplayScore { get; set; }

        public List<SlotType> Slots { get; set; }
    }

    public class SlotType 
    { 
        public EntrantType Entrant { get; set; }

    }

    public class EntrantType
    {
        public string Id { get; set; } 
    }

}