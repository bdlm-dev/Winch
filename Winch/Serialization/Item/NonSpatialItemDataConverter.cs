using System.Collections.Generic;

namespace Winch.Serialization.Item;

public class NonSpatialItemDataConverter : ItemDataConverter
{
    private readonly Dictionary<string, FieldDefinition> _definitions = new()
    {
        { "showInCabin", new(false, null)}
    };

    public NonSpatialItemDataConverter()
    {
        AddDefinitions(_definitions);
    }
}
