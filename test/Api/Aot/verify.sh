#!/usr/bin/env bash
# Publishes Packages.Aot, which roots LaunchPad's packages, and fails on a trimming/AOT warning from LaunchPad or from a dependency
# Packages.Aot/dependency-warnings.txt doesn't list. Then publishes the AOT server, fails on any trimming/AOT warning, and runs a round trip
# against the native binary with curl, as any client generated from its OpenAPI document would.
set -euo pipefail
cd "$(dirname "$0")"
rid="${RID:-$(dotnet --info | awk '/ RID:/ {print $2; exit}')}"
out="$(mktemp -d)"
base=http://127.0.0.1:5098
# (Not kill 0, the whole process group, when the server never started.)
trap '[[ -n "${server_pid:-}" ]] && kill "$server_pid" 2>/dev/null; rm -rf "$out"' EXIT

packages_log="$out/packages.log"
dotnet publish Packages.Aot -c Release -r "$rid" -o "$out/packages" > "$packages_log" 2>&1 || { cat "$packages_log"; exit 1; }
known="$(grep -vE '^\s*(#|$)' Packages.Aot/dependency-warnings.txt)"
unexpected=0
# IL2104/IL3053 name the assembly; every other warning starts with the type or member it is in (or the assembly, for IL2101).
assembly_warning="(IL2104|IL3053): Assembly '([^']+)'"
member_warning='warning IL[0-9]+: ([^ :(]+)'
while IFS= read -r line; do
    if [[ "$line" =~ $assembly_warning ]]; then origin="${BASH_REMATCH[2]}"
    elif [[ "$line" =~ $member_warning ]]; then origin="${BASH_REMATCH[1]}"
    else origin="$line"; fi
    listed=0
    while IFS= read -r prefix; do [[ "$origin" == "$prefix"* ]] && { listed=1; break; }; done <<< "$known"
    if [[ $listed == 0 ]]; then echo "::error::unexpected trimming/AOT warning: $line"; unexpected=1; fi
done < <(grep -E 'warning (IL[0-9]+|SYSLIB1[0-9]+)' "$packages_log" | sed -E 's/ \[[^]]*\]$//' | sort -u)
if [[ $unexpected == 1 ]]; then echo "::error::Packages.Aot published with warnings from LaunchPad, or from a dependency Packages.Aot/dependency-warnings.txt doesn't list"; exit 1; fi
echo "LaunchPad's packages publish with native AOT without warnings of their own ($rid)"

log="$out/publish.log"
dotnet publish Api.Aot.Server -c Release -r "$rid" -o "$out/server" > "$log" 2>&1 || { cat "$log"; exit 1; }
if grep -E 'warning (IL[0-9]+|SYSLIB1[0-9]+)|analysis warning IL' "$log"; then echo "::error::Api.Aot.Server published with trimming/AOT warnings"; exit 1; fi
"$out/server/Api.Aot.Server" --urls "$base" > "$out/server.log" 2>&1 &
server_pid=$!
for _ in $(seq 1 40); do curl -s -o /dev/null "$base/api/rockets" && break; sleep 0.25; done

fail() { echo "::error::$1"; cat "$out/server.log"; exit 1; }
# Prints the body, then the status code on the last line.
call() { curl -s -w '\n%{http_code}' -H 'Content-Type: application/json' "$@"; }
expect() { local response="$1" status="$2" text="${3:-}"; [[ "$(tail -n1 <<< "$response")" == "$status" ]] || fail "expected $status: $response"; [[ -z "$text" || "$response" == *"$text"* ]] || fail "expected '$text': $response"; }

created="$(call -X POST "$base/api/rockets" -d '{"name":"Falcon"}')"
expect "$created" 201 '"id"'
id="$(sed -n 's/.*"id":"\([0-9a-f-]*\)".*/\1/p' <<< "$created" | head -n1)"
expect "$(call "$base/api/rockets/$id")" 200 '"name":"Falcon"'
expect "$(call -X PUT "$base/api/rockets/$id" -d '{"name":"Falcon 9"}')" 200 '"name":"Falcon 9"'
expect "$(call "$base/api/rockets?name=Falcon%209")" 200 "\"id\":\"$id\""
expect "$(call -X DELETE "$base/api/rockets/$id")" 204
expect "$(call "$base/api/rockets/$id")" 404
if grep -q 'fail:' "$out/server.log"; then fail "server logged failures"; fi
echo "native AOT round trip OK ($rid)"
