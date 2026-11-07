import { defineCollection } from "astro:content";
import { docsLoader } from "@astrojs/starlight/loaders";
import { docsSchema } from "@astrojs/starlight/schema";
import { glob } from "astro/loaders";
import { z } from "astro/zod";
import { changelogsLoader } from "starlight-changelogs/loader";
import { starlightTagsExtension } from "starlight-tags/schema";

export const collections = {
    docs: defineCollection({
        loader: docsLoader(),
        schema: docsSchema({ extend: starlightTagsExtension }),
    }),
    /**
     * The C# API reference: one `<PackageId>.json` per package, written to `src/data/api/` from the build output by the
     * `docs:api` mise task (the Rocket.Surgery.ApiReference tool). The pages under `src/pages/api/` render it.
     */
    packages: defineCollection({
        loader: glob({ pattern: "**/*.json", base: "./src/data/api" }),
        schema: z
            .object({
                $schema: z.string().optional(),
                schemaVersion: z.string().optional(),
                package: z.object({
                    name: z.string(),
                    version: z.string(),
                    targetFramework: z.string(),
                    sourceRepository: z.string().optional(),
                    sourceCommit: z.string().optional(),
                }),
                apiHash: z.string().optional(),
                types: z.array(z.any()),
            })
            .passthrough(),
    }),
    changelogs: defineCollection({
        loader: changelogsLoader([
            {
                provider: "github",
                base: "changelog",
                owner: "RocketSurgeonsGuild",
                repo: "LaunchPad",
                token: import.meta.env.GH_API_TOKEN,
            },
        ]),
    }),
};
