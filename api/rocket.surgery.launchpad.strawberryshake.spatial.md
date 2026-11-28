# Rocket.Surgery.LaunchPad.StrawberryShake.Spatial

- Package: `Rocket.Surgery.LaunchPad.StrawberryShake.Spatial`
- Version: `2.0.1-beta.469`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.main.Sha.accb8cccadcfe11a9558e1e30ee2e6f231346f67/src/StrawberryShake.Spatial)
- NuGet: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.StrawberryShake.Spatial)

## Namespaces

### Rocket.Surgery.LaunchPad.StrawberryShake.Spatial

- [CoordinatesSerializer](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/coordinatesserializer.md) -- `class` -- Serializes HotChocolate's `Coordinates` scalar, a list of positions of arbitrary depth (the coordinates of a `GeoJSONMultiPolygonType`, for example)
- [GeometrySerializer](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/geometryserializer.md) -- `class` -- General purpose Geometry Serializer
- [PositionSerializer](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/positionserializer.md) -- `class` -- Serializes HotChocolate's `Position` scalar, a GeoJSON position ( `[x, y]` or `[x, y, z]`), as a `Geometries.Coordinate`

### Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints

- [StrawberryShakeSpatialJoint](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint.md) -- `class` -- Adds the spatial serializers to StrawberryShake
