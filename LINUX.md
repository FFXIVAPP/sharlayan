# Sharlayan Linux Support — Shelved State

**Branch**: `initial_linuxsupport`
**Last touched**: 9.1.1 (FCS submodule bump only on top of 9.1.0 refactor)
**Status**: Phase 9.1.0 (refactor) shipped on this branch; Phase 9.1.1 (Linux backend) blocked on a live Linux+FFXIV capture, see [§7](#7-what-blocks-resumption).

This document captures the full design intent, technical decisions, open questions, and next steps so the work can be resumed cold without re-deriving everything from chat history. If you're picking this up later — read end-to-end before opening the editor.

---

## 1. Goal

Native Linux support for Sharlayan. The game running under Wine, Proton, Steam, Lutris, or XIVLauncher is still the Windows x64 FFXIV client in memory, so `FFXIVClientStructs` layouts, offsets, and signatures stay valid. What changes is the process and memory access layer: instead of `OpenProcess` / `ReadProcessMemory` / `Process.Modules`, Sharlayan needs a Linux backend that finds `ffxiv_dx11.exe` through `/proc`, parses `/proc/<pid>/maps` to find module regions and base addresses, and reads memory via `process_vm_readv` (with a `pread` on `/proc/<pid>/mem` fallback).

Pillars to preserve:
- FCS as the authoritative source of layouts/signatures (drives easy patch-day upgrades).
- Lumina for sqpack data.
- Same high-level `Reader.*` API on both platforms — downstream apps remain mostly unaware of platform specifics.
- No third-party bridge (no Dalamud-style hosting). Sharlayan stays an independent out-of-process reader.

---

## 2. Inventory of the Windows-bound surface (pre-9.1.0)

The Windows dependency footprint was contained to **two files**:

| Location | Call | Purpose |
|---|---|---|
| `UnsafeNativeMethods.cs` | `OpenProcess` | Get a process handle for ReadProcessMemory |
| `UnsafeNativeMethods.cs` | `CloseHandle` | Release handle on Dispose |
| `UnsafeNativeMethods.cs` | `ReadProcessMemory` (×2 overloads) | Cross-process memory read |
| `UnsafeNativeMethods.cs` | `VirtualQueryEx` | Enumerate writable committed regions for signature scanning |
| `MemoryHandler.cs:47–50` | OpenProcess + handle storage | One-time open at attach |
| `MemoryHandler.cs:105` | CloseHandle | Dispose |
| `MemoryHandler.cs:219, 248` | ReadProcessMemory wrappers | `Read<T>`, `Peek` byte-buffer |
| `MemoryHandler.cs:166–167` | `Process.MainModule.BaseAddress` | Resolves base for offset arithmetic |
| `MemoryHandler.cs:282–336` | `Process.Modules` enumeration | "is this region a system module?" filter for Scanner |
| `Scanner.cs:180` | VirtualQueryEx loop | Walk address space, collect candidate regions |
| `Scanner.cs:221` | ReadProcessMemory loop | Scan regions for byte signatures |

Everything else in Sharlayan (FCS reflection, Lumina integration, `Reader.*`, `Mappers`, `Resolvers`, `Models`, `LuminaLookup`, all 179 unit tests) is platform-neutral C# and runs on Linux today as-is.

---

## 3. What Phase 9.1.0 already shipped on this branch

Refactor with **zero behaviour change** — caught by the existing 179 tests still passing identically.

**New files (`Sharlayan/Memory/`):**
- `IProcessMemoryAccessor.cs` — public OS-neutral interface
- `WindowsProcessMemoryAccessor.cs` — wraps existing kernel32 P/Invokes
- `ProcessMemoryAccessorFactory.cs` — OS-driven selection; non-Windows hosts throw `PlatformNotSupportedException` with a "Phase 9.1.1 will land here" message
- `ScannableRegion.cs` — replaces direct `MEMORY_BASIC_INFORMATION` use outside the Windows-specific accessor

**Refactor sites:**
- `MemoryHandler` ctor → `ProcessMemoryAccessorFactory.Create + Attach`
- `MemoryHandler.Dispose` → `accessor.Dispose`
- `MemoryHandler.Peek` / `GetStructure` → `accessor.ReadBytes`
- `MemoryHandler.GetStaticAddress` → `accessor.GetMainModuleBaseAddress`
- `MemoryHandler.ProcessHandle` property forwards from accessor (preserves callers)
- `Scanner.LoadRegions` → `accessor.EnumerateScannableRegions` + `IsSystemModule` filter
- `Scanner.FindExtendedSignatures` → `accessor.GetMainModuleBase/Size`
- `Scanner.ResolveLocations` RPM → `accessor.ReadBytes`
- Drops the inline MemCommit / PageGuard / Writable bit-flag constants from Scanner (they're now an internal detail of `WindowsProcessMemoryAccessor`)

**Surface preserved**: `UnsafeNativeMethods` stays public (Windows-only; `WindowsProcessMemoryAccessor` uses it as its implementation detail). `MemoryHandler.ProcessHandle` / `IsAttached` / `GetStructure<T>` / `Peek` all unchanged from the consumer's perspective.

Phase 9.1.0 commit on this branch: `6d31d83` (`refactor: extract IProcessMemoryAccessor — Phase 9.1.0 of Linux support`)
Subsequent FCS bump: `48c298c` (`chore: bump FCS submodule 7891a3d2a → 78d04f840`)

---

## 4. Phase 9.1.1+ — what's planned but not yet done

### Linux backend (`LinuxProcessMemoryAccessor`)

**Memory reads**: `process_vm_readv` syscall via libc P/Invoke.
```csharp
[StructLayout(LayoutKind.Sequential)]
private struct iovec {
    public IntPtr iov_base;
    public ulong  iov_len;
}

[DllImport("libc", SetLastError = true)]
private static extern long process_vm_readv(int pid,
    IntPtr local_iov,  ulong liovcnt,
    IntPtr remote_iov, ulong riovcnt,
    ulong flags);
```
Pinned `iovec` per side; returns bytes-read. `errno` on failure (typically `EPERM` from YAMA, `ESRCH` if process gone, `EFAULT` if address invalid). **Fallback**: `pread()` on `/proc/<pid>/mem` (slower, often blocked by `ptrace_scope ≥ 1`).

**Region enumeration**: parse `/proc/<pid>/maps`. Each line:
```
7f8b0c000000-7f8b0c021000 r-xp 00000000 fd:00 12345 /home/user/.../game/ffxiv_dx11.exe
```
Format: `start-end perms offset dev inode path`. Wine maps PE files via mmap so the path appears here as a real Linux path. ~30 lines of C# parser.

**Module discovery for ffxiv_dx11.exe** — see [§5](#5-critical-correction-module-base-formula) for the correct formula.

**Process discovery** (cross-platform helper):
```csharp
foreach (string pidDir in Directory.GetDirectories("/proc")) {
    if (!int.TryParse(Path.GetFileName(pidDir), out int pid)) continue;
    string cmdlineFile = $"/proc/{pid}/cmdline";
    if (!File.Exists(cmdlineFile)) continue;
    string cmdline = File.ReadAllText(cmdlineFile).Replace('\0', ' ');
    if (cmdline.Contains("/ffxiv_dx11.exe", StringComparison.OrdinalIgnoreCase)) {
        yield return pid;
    }
}
```
`/proc/*/comm` is truncated to 15 chars on Linux so prefer `/proc/*/cmdline`.

**sqpack path discovery**: the path in `/proc/<pid>/maps` for `ffxiv_dx11.exe` IS a real Linux path (Wine resolves through dosdevices). `Path.GetDirectoryName(...) + /sqpack` works natively. Lumina's `File.OpenRead` follows symlinks. **No special launcher-aware code needed for the common case.**

### Two-API region split (per [§5](#5-critical-correction-module-base-formula))

`IProcessMemoryAccessor` should split region enumeration into:
- `EnumerateMainModuleScanRegions(ModuleInfo mainModule)` — `r-xp` and `r-p` mappings whose path matches the main module. What every FCS `[StaticAddress]` signature actually needs.
- `EnumerateWritablePrivateRegions()` — `rw-p` mappings, including anonymous (`[heap]`, no-path private), excluding system shared libs and read-only file backing. Preserves the Windows `VirtualQueryEx` walk semantics for any non-FCS signature consumer.

The current 9.1.0 Windows accessor exposes `EnumerateScannableRegions()` (single method matching the existing filter). The two-API split lands alongside Linux because that's when the distinction matters; refactor the Windows side to match at the same time.

### Partial reads

Treat as first-class. The implementation should switch from `bool ReadBytes(...)` to one of:
- `(bool success, int bytesRead) ReadBytes(...)` — explicit
- `int ReadBytes(...)` returning bytes-read, with `< requested` ⇒ failure for primitives

Primitive readers (`GetInt32`, `GetByte`, etc.) treat `bytesRead < requested` as a hard failure (matches RPM behavior). Larger buffer reads — signature scan pages, the SoundManager list walk, FFT byte block, BGM scenes vector — get a `ReadBytesPaged(...)` helper that splits at 4 KB page boundaries and tolerates partial fills, with the caller deciding whether a short read on the final page is acceptable.

### Diagnostics

`AccessDiagnostics` API on the accessor. Linux flow:
1. ✓/✗ FFXIV process found — PID
2. ✓/✗ `/proc/<pid>/status` accessible — readable?
3. ✓/✗ `ffxiv_dx11.exe` mapping in `/proc/<pid>/maps` — base, module size
4. ✓/✗ test `process_vm_readv` of 4 bytes at known offset — success?
5. If (4) failed: read `/proc/sys/kernel/yama/ptrace_scope` and explain
6. ✓/✗ Wine prefix detection — any sqpack discoverable?
7. ✓/✗ Capability check — `CAP_SYS_PTRACE` granted to current process?
8. PID-namespace check — read `/proc/<pid>/status` `NSpid` field. If different from outer PID, surface "running inside or against a sandboxed Steam — unsupported".

### Build / CI / tests

- No project file changes for the core lib — `net10.0` is already cross-platform.
- Add `ubuntu-latest` matrix to `.github/workflows/ci.yml`. All 179 tests should pass on Linux (pure managed reflection).
- New unit tests: `LinuxProcessMemoryAccessorTests` with fixture `/proc/<pid>/maps` text and `errno` mapping.
- Harness: `harness.ps1` stays Windows; add `harness.sh` for Linux. Same harness program — only the launcher differs (no admin/sudo wrapper; ptrace permission check happens inside).
- Release: `release.yml` already produces a single cross-platform `.nupkg` — no changes for distribution.

---

## 5. Critical correction: module base formula

**Earlier draft was wrong**: "first executable mapping" or "lowest start address" is not the right rule.

For mapped PE images, the robust formula is `module_base = mapping.Start - mapping.Offset` (the load bias), then taken as `Min(...)` across **all** mappings of the same file. The PE header mapping at file offset 0 makes this look identical to `Min(Start)` in the easy case, but Wine sometimes maps headers read-only at offset 0 *and* skips them in the executable mapping (offset 0x1000), so `Min(Start)` over `r-x` mappings only would land 0x1000 too high.

**Correct implementation**:
```csharp
long base = long.MaxValue;
long end  = long.MinValue;
foreach (var mapping in maps.Where(m => m.Path.EndsWith("/ffxiv_dx11.exe"))) {
    long bias = mapping.Start - mapping.Offset;
    if (bias < base) base = bias;
    if (mapping.End > end) end = mapping.End;
}
long size = end - base;
```

---

## 6. Architectural rules (do not regress)

### FCS as metadata-only on the external-reader side

FFXIVClientStructs is consumed in Sharlayan **only** as a metadata source:
- `FieldOffsetReader.OffsetOf<T>(name)` reads `[FieldOffset]` attributes via reflection.
- `FFXIVClientStructsSignatureExtractor` reads `[StaticAddress]` attributes.

The Linux backend MUST continue this discipline. Specifically:
- Never call `Instance()` on FCS singleton structs.
- Never dereference an FCS pointer field as if it were live.
- Never invoke `[MemberFunction]` or vfunc wrappers.
- Treat all live data as bytes copied out via `IProcessMemoryAccessor.ReadBytes(...)`, then interpreted using FCS-derived offsets.

A future contributor who introduces an FCS-pointer dereference inside a memory-accessor implementation will silently break Linux. The interface xmldoc on `IProcessMemoryAccessor` already calls this out.

### Capability handling — do NOT setcap on /usr/bin/dotnet

Granting `CAP_SYS_PTRACE` to the shared dotnet host is a security footgun — every framework-dependent .NET app launched through that binary inherits a powerful debugging capability.

**Do recommend**: ship a self-contained publish (`dotnet publish -p:PublishSingleFile=true --self-contained true -r linux-x64`) and let users (or a downstream like Chromatics) apply capabilities to *that* binary only.

**Don't recommend**: blanket setcap on the system dotnet runtime.

README troubleshooting flow: "if the diagnostic reports YAMA blocked, the options are (a) `setcap` your self-contained Sharlayan-consuming binary, (b) `sudo sysctl kernel.yama.ptrace_scope=0` for a session, or (c) launch as a child of the FFXIV process. We do not recommend granting capabilities to `/usr/bin/dotnet`."

### YAMA ptrace_scope (`/proc/sys/kernel/yama/ptrace_scope`)

| Value | Meaning | Sharlayan |
|---|---|---|
| `0` | Classic — anyone same-UID | works |
| `1` (default Debian/Ubuntu) | Only ancestor processes | **fails** unless launched as a child of FFXIV (impractical) or with `CAP_SYS_PTRACE` |
| `2` | CAP_SYS_PTRACE only | works with `setcap cap_sys_ptrace+ep <self-contained-binary>` or root |
| `3` | Disabled | does not work; user must change kernel setting (and **cannot** un-set 3 without reboot) |

### Sandboxing stance

**Flatpak Steam / Snap Steam / sandboxed setups: declare unsupported initially.** Easier to mark experimental later if a clear sandbox-aware path emerges than to over-promise and chase regressions. The diagnostic should detect (`/proc/<pid>/status` `NSpid` mismatch ⇒ different PID namespace ⇒ probably sandboxed) and surface that explicitly rather than a confusing `EPERM`.

---

## 7. What blocks resumption

**Phase 9.1.1 is gated on a live Linux+FFXIV capture.** Without it the design is implementing blind.

**Specifically needed:**
1. `cat /proc/<pid>/maps | grep -i ffxiv` — full output, all mappings of `ffxiv_dx11.exe` and any DLLs (Wine maps Game/*.dll alongside)
2. `cat /proc/<pid>/cmdline | tr '\0' ' '` — confirm the path format
3. `cat /proc/<pid>/status` — interesting fields: `Tgid`, `TracerPid`, `NSpid`, `Uid`
4. `cat /proc/sys/kernel/yama/ptrace_scope` — host setting
5. `getcap` on the wine64 binary actually running FFXIV
6. The output of the existing harness `[1] PROCESS DETECTION` block from a Windows machine reading the same FFXIV install (just to compare base addresses if possible)

Sources to try, in order of likelihood of producing useful output:
- Steam Proton FFXIV
- XIVLauncher native Linux build (`xlcore`)
- Lutris-managed FFXIV

**Once that capture exists**, the design questions left to answer:
- Does Wine relocate the EXE to a different base than Windows ASLR would? (Should be a non-issue because FCS `[StaticAddress]` signatures are RIP-relative, but worth confirming.)
- Is the EXE base in `/proc/<pid>/maps` consistent with what FCS expects to scan? (Should be yes — FCS scans for byte patterns inside the loaded EXE; whatever Wine maps is what scans.)
- Is sqpack reachable via the path in `/proc/<pid>/maps`? (Should be yes — Wine maps the actual file from disk.)
- Are inter-module pointer chains (`Framework → UIModule → ...`) intact? (Should be yes — the pointers stored in memory point to wherever Wine put the modules.)

---

## 8. Phased plan reminder

| Phase | Status | Scope |
|---|---|---|
| **9.1.0** | ✅ shipped on this branch | Refactor: extract `IProcessMemoryAccessor`, move existing kernel32 calls into `WindowsProcessMemoryAccessor`. **Zero behavior change.** |
| **9.1.1** | ⏸️ blocked on live capture | `LinuxProcessMemoryAccessor`: process discovery, `/proc/<pid>/maps` parser with `Min(Start - Offset)` formula, `process_vm_readv` reader with partial-read handling, two-API region split, basic diagnostics. **Linux works for happy path.** |
| **9.1.2** | not started | `AccessDiagnostics` API, ptrace_scope detection, sandbox detection, harness Linux output. README troubleshooting section (no setcap-on-dotnet recommendation). |
| **9.1.3** | not started | CI: ubuntu-latest matrix, integration tests against parser/reader components. Documentation polish. |
| **9.2.0** | stretch | `pread`-on-`/proc/<pid>/mem` fallback if `process_vm_readv` blocked but `/proc/<pid>/mem` readable; self-contained publish helper docs; XIVLauncher integration recommendations. |

---

## 9. Resumption checklist

When picking this back up:

1. `git checkout initial_linuxsupport`
2. Confirm the branch still cleanly rebases onto main: `git fetch origin && git rebase origin/main`. Phase 9.1.0 should not conflict with anything that lands on main; if it does, prefer the main version of `MemoryHandler.cs` / `Scanner.cs` and re-apply the `IProcessMemoryAccessor` delegation on top.
3. Run `dotnet test Sharlayan.Tests/Sharlayan.Tests.csproj` — must be 179/179 passing before any new code lands.
4. Get a live capture per [§7](#7-what-blocks-resumption) before writing `LinuxProcessMemoryAccessor`.
5. Write the parser/reader against a fixture text file FIRST (unit-test-driven) — you don't need a real FFXIV process to validate the `/proc/<pid>/maps` parser logic.
6. Add the two-API region split to `IProcessMemoryAccessor` as part of 9.1.1, and refactor `WindowsProcessMemoryAccessor` to match at the same time.
7. Apply the partial-read API change at the same time. Don't drag the old `bool` return through into Linux code.
8. **Do not** introduce setcap-on-dotnet recommendations anywhere.
9. Mark Flatpak/Snap unsupported in the README and the diagnostic output.

---

## 10. Files to consult when resuming

| File | Why |
|---|---|
| `Sharlayan/Memory/IProcessMemoryAccessor.cs` | The interface; xmldoc explains every method and the FCS-as-metadata rule |
| `Sharlayan/Memory/WindowsProcessMemoryAccessor.cs` | Reference implementation showing exactly what behaviour the Linux side needs to match |
| `Sharlayan/Memory/ProcessMemoryAccessorFactory.cs` | The slot for `LinuxProcessMemoryAccessor` registration |
| `Sharlayan/MemoryHandler.cs` | The consumer side — confirm any new accessor methods are wired through |
| `Sharlayan/Scanner.cs` | The other consumer; the only place `EnumerateScannableRegions` is used today |
| `Sharlayan.Harness/Program.cs` | Harness entry point; `[1]` block needs Linux-aware diagnostic output |
| `harness.ps1` | Windows-only launcher; create `harness.sh` for Linux |
| `CLAUDE.md` | Project conventions (versioning, integrity tests, debug-deploy to Chromatics) |
| `DEPENDENCY.md` | Known FCS dependencies and pinned offsets — add entries for any new field offsets the Linux backend reflects |

---

## 11. Open assumptions worth re-validating once a real Linux box is available

1. **`process_vm_readv` performance**: should be faster than RPM (single syscall, no handle indirection); benchmark against signature scan workload (~50 MB of address space).
2. **PE relocation across launches**: signature scan should adapt automatically since it finds the new base every run, but verify.
3. **Older glibc**: some distros may ship libc without `process_vm_readv` symbol. Probe via `dlsym` and fall back to `/proc/<pid>/mem` if missing. Modern distros are fine.
4. **`Process.MainModule.BaseAddress` on Linux+Wine**: returns the wine64 binary, not ffxiv_dx11.exe. The accessor's `GetMainModuleBaseAddress()` MUST resolve from `/proc/<pid>/maps` — never call the .NET cross-platform API.
5. **Lumina sqpack reads with `LoadMultithreaded = true`**: pure managed file IO, should be platform-agnostic, but smoke-test.

---

## 12. Contact context

User who initiated this work: see `git log` on this branch for the commit author. Original assessment + corrections were exchanged interactively before any code landed; the corrections in [§5](#5-critical-correction-module-base-formula) and [§6](#6-architectural-rules-do-not-regress) override what's in `git log` history.

If in doubt about the scope of "no behaviour change" for Phase 9.1.0: every existing test in `Sharlayan.Tests` must still pass. Any failure during refactor means the refactor was wrong.
