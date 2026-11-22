# Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc

- Package: `Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc`
- Version: `2.0.1-pr.1294`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Grpc/Joints)
- NuGet: [Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc)

## Namespaces

### Rocket.Surgery.LaunchPad.Grpc.Joints

- [ZeroAllocGrpcJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint.md) -- `class` -- Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into an `StatusCode.InvalidArgument` status, and the denial of a stream request, which the mediator pipeline throws, into the status of its case. Implements the `Joints.IServiceJoint`
