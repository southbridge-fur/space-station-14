/*
This is intended to be used by really any type of tool that
draws it's power source or fuel from a container of some kind.

This includes just about every type of power source in the phsycis sense
including electricity, pneumatics (air), ethanol, welder fuel, etc.

Exposes a couple interfaces
- IsContainerEmpty
- GetCurrentFuelLevel
- Refill
-
*/


namespace Content.Shared.Fuel.Components

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FuelProviderComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public float? wattsMax = 0.0;

    [DataField]
    [AutoNetworkedField]
    public float? newtons = 0;

    [DataField]
    [AutoNetworkedField]
    public string? FuelType = "default";
}
