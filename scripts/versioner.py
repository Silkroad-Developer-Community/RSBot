"Tool to define a version for releases"

import os
import re
import argparse


def get_version_from_csproj(csproj_path):
    with open(csproj_path, "r", encoding="utf-8") as f:
        content = f.read()

    # Try to find InformationalVersion or AssemblyVersion
    match = re.search(r"<InformationalVersion>(.*?)</InformationalVersion>", content)
    if not match:
        match = re.search(r"<AssemblyVersion>(.*?)</AssemblyVersion>", content)

    if match:
        return match.group(1).strip()
    return None


def update_csproj_version(csproj_path, new_version):
    with open(csproj_path, "r", encoding="utf-8") as f:
        content = f.read()

    # Update both InformationalVersion and AssemblyVersion
    content = re.sub(
        r"<InformationalVersion>.*?</InformationalVersion>",
        f"<InformationalVersion>{new_version}</InformationalVersion>",
        content,
    )
    content = re.sub(
        r"<AssemblyVersion>.*?</AssemblyVersion>", f"<AssemblyVersion>{new_version}</AssemblyVersion>", content
    )

    with open(csproj_path, "w", encoding="utf-8") as f:
        f.write(content)


def update_iss_version(iss_path, new_version):
    with open(iss_path, "r", encoding="utf-8") as f:
        content = f.read()

    # Update #define AppVersion
    new_content = re.sub(r'#define AppVersion ".*?"', f'#define AppVersion "{new_version}"', content)

    with open(iss_path, "w", encoding="utf-8") as f:
        f.write(new_content)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--version", help="Specific version to set in both csproj and iss")
    parser.add_argument("--nightly", help="Suffix the project version with -nightly.<date>")
    args = parser.parse_args()

    csproj = os.path.join("Application", "RSBot", "RSBot.csproj")
    iss = os.path.join("scripts", "OasisBot.iss")

    if args.version:
        version = args.version
        print(f"Setting version to: {version}")
        update_csproj_version(csproj, version)
        update_iss_version(iss, version)
        print("Updated RSBot.csproj and OasisBot.iss")
    elif args.nightly:
        base_version = get_version_from_csproj(csproj)
        if base_version:
            version = f"{base_version}-nightly.{args.nightly}"
            print(f"Setting nightly version to: {version}")
            # We only update the ISS file for nightlies to keep the CSPROJ clean for the next dev cycle
            update_iss_version(iss, version)
            print("Updated OasisBot.iss with nightly version")
        else:
            print("Could not find version in csproj")
            exit(1)
    else:
        version = get_version_from_csproj(csproj)
        if version:
            print(f"Syncing version from csproj: {version}")
            update_iss_version(iss, version)
            print("Updated OasisBot.iss")
        else:
            print("Could not find version in csproj")
            exit(1)

    # Output to GitHub Actions environment if available
    if "GITHUB_OUTPUT" in os.environ:
        with open(os.environ["GITHUB_OUTPUT"], "a") as f:
            f.write(f"version={version}\n")
