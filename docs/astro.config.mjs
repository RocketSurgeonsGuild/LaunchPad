import { unified } from "@astrojs/markdown-remark";
import starlight from "@astrojs/starlight";
import { defineConfig } from "astro/config";
import starlightAutoDrafts from "starlight-auto-drafts";
import starlightChangelogs from "starlight-changelogs";
import starlightGithubAlerts from "starlight-github-alerts";
import starlightHeadingBadges from "starlight-heading-badges";
import starlightImageZoom from "starlight-image-zoom";
import starlightLinksValidator from "starlight-links-validator";
import starlightLlmsTxt from "starlight-llms-txt";
import starlightPageActions from "starlight-page-actions";
import { starlightIconsPlugin } from "starlight-plugin-icons";
import starlightScrollToTop from "starlight-scroll-to-top";
import starlightSidebarTopics from "starlight-sidebar-topics";
import starlightTags from "starlight-tags";

// In GitHub Actions, the site is served under /LaunchPad; locally we run at the root. The gh-pages branch holds latest main at
// that root, each release under /LaunchPad/v<version> and pull request previews under /LaunchPad/pr-preview/pr-<n>, so CI sets
// DOCS_BASE to the folder a build deploys to. The version switcher reads versions.json from the root.
const root = process.env.DOCS_ROOT || (process.env.GITHUB_ACTIONS ? "/LaunchPad" : "");
const base = process.env.DOCS_BASE || root;

export default defineConfig({
    site: "https://rocketsurgeonsguild.github.io",
    base,
    vite: {
        define: {
            __DOCS_ROOT__: JSON.stringify(root),
        },
    },
    markdown: {
        processor: unified(),
    },
    integrations: [
        starlight({
            title: "LaunchPad",
            description: "A simple starting point for building .NET applications on Specular and Dovetail.",
            social: [
                {
                    icon: "github",
                    label: "GitHub",
                    href: "https://github.com/RocketSurgeonsGuild/LaunchPad",
                },
            ],
            components: {
                // Adds the docs version switcher (src/components/VersionSelect.astro) next to the theme picker.
                ThemeSelect: "./src/components/overrides/ThemeSelect.astro",
            },
            plugins: [
                starlightAutoDrafts(),
                starlightGithubAlerts(),
                starlightSidebarTopics(
                    [
                        {
                            label: "Getting Started",
                            link: "/guides/",
                            icon: "open-book",
                            items: [{ autogenerate: { directory: "guides" } }],
                        },
                        {
                            label: "Concepts",
                            link: "/concepts/",
                            icon: "information",
                            items: [{ autogenerate: { directory: "concepts" } }],
                        },
                        {
                            label: "Changelog",
                            link: "/changelog/",
                            icon: "list-format",
                        },
                    ],
                    {
                        exclude: ["/tags/**", "/changelog/**"],
                    },
                ),
                starlightLinksValidator({
                    errorOnRelativeLinks: false,
                }),
                starlightHeadingBadges(),
                starlightImageZoom(),
                starlightScrollToTop(),
                starlightPageActions({
                    editLink: "https://github.com/RocketSurgeonsGuild/LaunchPad/edit/main/docs/src/content/docs/",
                }),
                starlightIconsPlugin(),
                starlightTags(),
                starlightChangelogs(),
                starlightLlmsTxt(),
            ],
        }),
    ],
});
