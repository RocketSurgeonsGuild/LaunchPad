# LaunchPad wiring is built from Dovetail Joints, with Specular for scanning

LaunchPad wired applications with `Rocket.Surgery.Conventions`. Its successors, Dovetail (build-time composition through Joints) and Specular (AOT-safe assembly scanning), replace it, so every LaunchPad convention is re-expressed as a Joint, assembly scanning goes through `ISpecularProvider`, and every `Rocket.Surgery.Conventions*` reference is removed.

We rejected a hybrid that carries both models through a transition: it doubles the wiring surface to maintain and has no clean end point. The cost is a breaking restructure of every package and a vocabulary change from "convention" to "Joint" (see `GLOSSARY.md`).
