import { apiDocumentCacheKey } from "@utils/api-cache-key";

import { markdownResponse } from "@utils/api-markdown-shared";
import { renderCSharpMemberKindMarkdown } from "@utils/csharp-api-markdown";
import type { PackageMember, PackageType } from "@utils/packages";
import { getPackageDocument, getPackages, memberKindOrder, memberKindSlugs, packageSlug, typeSlugOf } from "@utils/packages";
import type { APIRoute } from "astro";

export const prerender = true;

type RouteProps = {
    memberKind: string;
    packageName: string;
    type: PackageType;
};

type StaticPath = {
    cacheKey: string;
    params: { memberKind: string; package: string; type: string };
    props: RouteProps;
};

export async function getStaticPaths(): Promise<StaticPath[]> {
    const packages = await getPackages();
    const paths: StaticPath[] = [];

    for (const entry of packages) {
        const pkg = entry.data;
        const pkgSlug = packageSlug(pkg.package.name);
        const cacheKey = apiDocumentCacheKey(pkg);

        for (const type of pkg.types) {
            const typeSlug = typeSlugOf(type);
            if (!typeSlug) {
                continue;
            }

            const members = type.members ?? [];
            for (const memberKind of memberKindOrder) {
                const memberKindSlug = memberKindSlugs[memberKind];
                if (!memberKindSlug || !members.some((member: PackageMember) => member.kind === memberKind)) {
                    continue;
                }

                paths.push({
                    cacheKey,
                    params: {
                        memberKind: memberKindSlug,
                        package: pkgSlug,
                        type: typeSlug,
                    },
                    props: {
                        memberKind,
                        packageName: pkg.package.name,
                        type,
                    },
                });
            }
        }
    }

    return paths;
}

export const GET: APIRoute = async ({ props }) => {
    const base = import.meta.env.BASE_URL.replace(/\/$/, "");
    const routeProps = props as RouteProps;
    const pkg = await getPackageDocument(routeProps.packageName);

    return markdownResponse(renderCSharpMemberKindMarkdown(pkg, routeProps.type, routeProps.memberKind, pkg.types, base));
};
