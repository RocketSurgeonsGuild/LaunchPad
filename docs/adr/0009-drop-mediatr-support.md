# LaunchPad drops MediatR support

LaunchPad 2.0 no longer supports MediatR. The API generators lose their built-in MediatR profile, and nothing in the repository references the MediatR package: not the libraries (which stopped in #3143), not the samples, and not the tests. The five built-in mediator profiles that remain are martinothamar/Mediator, ZeroAlloc.Mediator, ChannelMediator, Immediate.Handlers and Foundatio.Mediator. ZeroAlloc.Mediator, which the samples already use, replaces MediatR as the reference server of the functional tests (API versioning, union responses and the exported OpenAPI, SDL and `.proto` specs). The AOT test stays on martinothamar/Mediator.

MediatR 13 and later are commercially licensed. LaunchPad could only keep MediatR by testing against 12.x, the last Apache-licensed line, which ages without fixes, or by testing against a version its users need a license for. Neither is worth it when five maintained, freely licensed mediators are supported, most of them source generated and AOT-safe, which MediatR isn't.

## Considered Options

- **Support any MediatR version.** Test the profile against 12.x and 13+, and let the application choose and license it. Rejected: it keeps a commercial dependency in LaunchPad's test matrix and Renovate updates, for a mediator that can't take part in native AOT.
- **Stay on 12.x.** Pin `[12,13)`. Rejected: it commits LaunchPad to an unmaintained line.
- **A MediatR runtime integration package**, like the `ZeroAlloc.*` packages. Rejected for the same reasons.
- **Keep a migration path:** a ready-to-copy `[assembly: MediatorProfile]` recipe, or a diagnostic pointing to one. Rejected: a custom profile can still describe MediatR, but LaunchPad doesn't document or test that path.

## Consequences

An application that uses MediatR with the API generators gets the "no mediator profile" error (LPAD1002) and must move to a supported mediator or describe MediatR itself with `[assembly: MediatorProfile]` and `[MediatorRequest]`. The 2.0.0 migration notes (#3078) say so. The FluentValidation integration packages, whose main job was a MediatR pipeline behavior, are removed with it (#3154). The work is tracked by #3215.
