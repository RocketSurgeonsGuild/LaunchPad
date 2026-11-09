#!/usr/bin/env bash
# Publishes the AOT server and client, fails on any trimming/AOT warning, then runs a round trip between the two native binaries.
set -euo pipefail
cd "$(dirname "$0")"
rid="${RID:-$(dotnet --info | awk '/ RID:/ {print $2; exit}')}"
out="$(mktemp -d)"
trap 'kill "${server_pid:-0}" 2>/dev/null || true; rm -rf "$out"' EXIT
for project in Api.Aot.Server Api.Aot.Client; do
    log="$out/$project.log"
    dotnet publish "$project" -c Release -r "$rid" -o "$out/$project" > "$log" 2>&1 || { cat "$log"; exit 1; }
    if grep -E 'warning (IL[0-9]+|SYSLIB1[0-9]+)|analysis warning IL' "$log"; then echo "::error::$project published with trimming/AOT warnings"; exit 1; fi
done
"$out/Api.Aot.Server/Api.Aot.Server" --urls http://127.0.0.1:5098 > "$out/server.log" 2>&1 &
server_pid=$!
for _ in $(seq 1 40); do curl -s -o /dev/null http://127.0.0.1:5098/api/rockets && break; sleep 0.25; done
result="$("$out/Api.Aot.Client/Api.Aot.Client" http://127.0.0.1:5098/)"
echo "$result"
grep -q 'after delete: null (404)' <<< "$result" || { echo "::error::round trip failed"; cat "$out/server.log"; exit 1; }
if grep -q 'fail:' "$out/server.log"; then echo "::error::server logged failures"; cat "$out/server.log"; exit 1; fi
echo "native AOT round trip OK ($rid)"
