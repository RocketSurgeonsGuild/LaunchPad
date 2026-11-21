# Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc

- Package: `Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc`
- Version: `2.0.1-pr.1286`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Grpc/Joints)
- NuGet: [Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc)

## Namespaces

### Rocket.Surgery.LaunchPad.Grpc.Joints

- [ZeroAllocGrpcJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint.md) -- `class` -- Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into an `StatusCode.InvalidArgument` status, and the denial of a stream request, which the mediator pipeline throws, into the status of its case. Implements the `Joints.IServiceJoint`
