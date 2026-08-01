# Encoding Converters Core (EncConverters / SIL.Transduction)

A 20+ year old transduction platform, developed by SIL, that converts text between character encodings and — more recently — between languages. It exposes a single stable API, **`IEncConverter`**, behind which many different conversion engines ("transducers") can be plugged in interchangeably.

It was originally built to solve one very specific problem for Bible translation and literacy work: converting text out of legacy 8-bit "Ansi-hack" fonts (custom fonts that mapped, say, Devanagari glyphs onto code page 1252) into Unicode. That problem is largely solved today — everyone has either migrated to Unicode or already has a copy of the sibling GUI project **SILConverters** to do it for them. What keeps this repo alive and under active development is that the same plug-in architecture turned out to be a convenient place to also host **AI-driven machine translation** engines (Azure OpenAI, Google Vertex AI/Gemini, Google Translate, Bing/Microsoft Translator, DeepL, and a self-hosted Meta NLLB model), so client apps that already know how to call `IEncConverter` get translation "for free."

Client applications include Paratext, FieldWorks, LibreOffice/MS Office plugins, and various in-house SIL tools, on both x86 and x64 hosts, via .NET, native C/C++, and COM.

## Architecture

```
                        [Client Applications]
        Paratext · FieldWorks · MS Office · LibreOffice · in-house tools
                                  │
                                  ▼
                  IEncConverter / IEncConverterConfig
                (src/ECInterfaces — the stable public contract)
                                  │
                                  ▼
              EncConverters dispatcher ("SilEncConverters40.dll")
                          (src/EncCnvtrs)
        - discovers implementations via HKLM\SOFTWARE\SIL\SilEncConverters40
        - resolves named mappings via the mappingRegistry.xml repository
        - instantiates the right transducer by ProgID / assembly version
                                  │
        ┌─────────────────────────┴─────────────────────────────┐
        │                                                        │
   Legacy encoding / font transducers                  AI machine-translation transducers
   (native C/C++ engines, P/Invoked)                    (src/EcTranslators, online + self-hosted)
   ├─ Consistent Changes  — src/CcEC                    ├─ Azure OpenAI     — AzureOpenAi
   ├─ TECkit              — built into EncCnvtrs         ├─ Google Vertex AI — VertexAi (Gemini)
   ├─ ICU convert/regex/  — src/IcuEC (+ icu4net)        ├─ Google Translate — GoogleTranslator
   │  transliterate                                      ├─ Bing/MS Translator — BingTranslator
   ├─ Python script       — src/PyScriptEC (Py2 native    ├─ DeepL            — DeepLTranslator
   │  or Py.NET for Py3)                                  └─ Meta NLLB        — NllbTranslator
   ├─ Perl expression     — src/PerlExpressionEC             (self-hosted Docker service;
   ├─ Windows code page   — src/CodePageEC                    supports private fine-tuned models)
   ├─ Font↔ISCII (Indic)  — src/SilIndicEncConverters
   └─ AdaptIt KB / guesser— src/AIGuesserEC, DriveAiEncConverter
                                                          (Note: "AI" in these two names means
                                                           "AdaptIt", a legacy tool — not AI/LLM)
```

Every transducer, native or managed, ultimately implements `IEncConverter` (or the native COM equivalent in `src/EncCnvtrs/lib/ECEncConverter.h`), so the dispatcher and all client apps only ever code against one interface.

## Repository layout

| Path | What it is |
|---|---|
| `src/ECInterfaces` | The `IEncConverter` / `IEncConverterConfig` / `IEncConverters` contracts, the `mappingRegistry` XML schema, shared utilities. Changes here must be mirrored in `EncCnvtrs/lib/ECEncConverter.h`. |
| `src/EncCnvtrs` | The dispatcher (`SilEncConverters40.dll`): converter discovery/instantiation, the mapping-registry repository logic, the built-in TECkit and compound/fallback converters, and the `WebBrowserAdaptor` UI layer (IE / GeckoFX / WebView2). |
| `src/CcEC` | Consistent Changes (CC) transducer — P/Invokes prebuilt `CC32.dll`/`CC64.dll`. |
| `src/IcuEC`, `src/icu4net` | ICU-based conversion, regex, and transliteration (native `.vcxproj` DLLs + C# wrappers); `icu4net` is a separate C++/CLI binding of core ICU classes. |
| `src/PyScriptEC` | Python-script transducer — legacy embedded CPython 2.7 (native) and a newer Python.NET-based Python 3 path. |
| `src/PerlExpressionEC` | Shells out to `perl.exe` to evaluate a Perl expression. |
| `src/CodePageEC` | Thin wrapper over Win32 code-page conversion APIs. |
| `src/SilIndicEncConverters` | Font↔ISCII conversion for Indic scripts, plus a couple of unrelated web-transliteration converters bundled in the same assembly. |
| `src/AIGuesserEC`, `src/DriveAiEncConverter` | Legacy AdaptIt knowledge-base tools (heuristic word-correspondence guessing). Despite the name, unrelated to AI/LLM translation. |
| `src/EcTranslators` | The AI/LLM translation engines (see above) and their shared base classes (`TranslatorConverter`, `PromptExeTranslator`). This is the most actively developed area of the repo. |
| `src/ECDriver` | A flat C API (`ecdriver.h`) for native C/C++ clients that don't want COM interop directly; Windows and Linux variants. |
| `src/SpellingFixer30` | Vernacular spelling-correction plug-in (bad↔good word mapping, valid-vowel-sequence rules for Indic scripts). |
| `src/BackTranslationHelper` | WinForms tool that drives the AI translators to help reviewers check back-translations, with an embedded HTML/JS UI. |
| `src/PtxConverters` | Paratext-specific converter reading PTX project settings. |
| `src/ECFileConverter` | CLI batch file-conversion utility. |
| `src/AppDataMover` | One-time migration tool for moving EncConverters data out of old install locations. |
| `src/ConvertersActivator` | Partially-implemented post-install "activate these bundled converters" UI — see `Design.txt`; not functional. |
| `src/RunTests`, `src/TestEncCnvtrs` | NUnit-style tests, run via a console harness so they also work under Mono on Linux. |
| `installer/` | WiX merge modules, one per native-DLL bundle (CC, TECkit, ICU, Perl, Python, SpellFixer, GeckoFX/Firefox, AdaptIt Guesser). |
| `debian/`, `debian-ec/`, `configure.ac`, `Makefile.in` | Linux/autotools build and Debian packaging. |

## Building

### Windows

```
msbuild "EncConverters 2019.sln" /p:Configuration=Debug /p:Platform=x86
msbuild "EncConverters 2019.sln" /p:Configuration=Debug /p:Platform=x64
```
NuGet restore is required first (`nuget restore "EncConverters 2019.sln"`). **The solution cannot be built `AnyCPU`** — several native DLLs (CC, TECkit, ICU, Python, GuesserEC, SilFont2Iscii) are pinned to a specific bit width, so x86 and x64 must be built as fully separate configurations.

### Linux

```sh
sudo apt-get install automake g++ python-dev libicu-dev mono5-sil icu-dev-fw libteckit-dev
(. environ && ./autogen.sh && make)      # first build
(. environ && make)                      # subsequent builds
```

Tests run under Mono:
```sh
export EC_COMMON_APPLICATION_DATA_PATH="$(pwd)/ec-common"
export MONO_REGISTRY_PATH="${EC_COMMON_APPLICATION_DATA_PATH}/registry"
cp -a src/RunTests/bin/x64/Debug/RunTests.exe output/x64/Debug/ &&
  (. environ && mono output/x64/Debug/RunTests.exe)
```

See also `ReadMe_linux.txt` and `ReadMe_windows.txt`.

### NuGet package

The published package **[Encoding-Converters-Core](https://www.nuget.org/packages/Encoding-Converters-Core/)** is built from `Package.Debug.nuspec` / `Package.Release.nuspec`. To cut a new version: run Release builds for both x86 and x64, bump the version in the nuspec, then `nuget.exe pack`.

## Configuration & discovery

Converters are looked up two ways at runtime:
1. **`HKLM\SOFTWARE\SIL\SilEncConverters40\ConvertersSupported`** — every installed transducer implementation self-registers here (COM ProgID, priority, per-assembly-version overrides).
2. **The mapping registry** — an XML file (`mappingRegistry.xml`, schema in `src/ECInterfaces/SILMappingRegistry.xsd`) holding named, user-configured conversion mappings, normally under `%ProgramData%\SIL\Repository` (Windows) or `/var/lib/encConverters` (Linux), overridable via `EC_COMMON_APPLICATION_DATA_PATH`.

Several transducer config dialogs embed an HTML help/setup page via `WebBrowserAdaptor` (`src/EncCnvtrs`), which can use Internet Explorer (WinForms `WebBrowser`), GeckoFX/XULRunner (bundled, Linux's only option), or WebView2/Edge (the modern default on Windows). See `Handover.md` for the plan to consolidate this.

## Further reading

- [`Handover.md`](Handover.md) — current-state assessment and modernization roadmap for anyone picking up maintenance of this repo.
- [SILConverters developer notes](https://software.sil.org/silconverters/silconverters-developer/) — hints on consuming EncConverters from your own application.
