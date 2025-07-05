# Changelog

All notable changes to this project will be documented in this file.

## [2025.7.1] - 2025-07-05

### Added
- Introduced **ScenarioAttribute** with multiple overloads:
  - Simple Given/When/Then.
  - Scenario code and title for BIP-compliant metadata (`VCHIP-*` codes).
  - Flexible steps array for more descriptive scenarios.
- Added **NodeIdAttribute**, **TaskIdAttribute**, and **UseCaseIdAttribute** to associate tests with nodes, tasks, and use cases.
- Added **ThenAttribute** supporting User Acceptance Criteria codes (`VCHIP-XXXX-UACXXX`) for granular coverage reporting.
- Added **WhenTestingFor<T>**, **WhenTestingForAsync<T>**, **WhenUsingDatabase<TContext>**, and **WhenUsingBrowser<TEntryPoint>** base classes to facilitate:
  - Pure logic tests.
  - Async test flows.
  - In-memory and EF Core integration tests.
  - Browser-driven tests with Playwright.

### Changed
- Enhanced coverage report generation via `AssemblyCoverageExtensions`:
  - Generates Markdown reports with scenario metadata.
  - Includes tags for Scenario Codes, NodeIds, TaskIds, and UseCaseIds.
  - Lists all `[Fact]` test methods and their associated `Then` descriptions and codes.

### Added
- Added `ltx` console tool:
  - Command `report` to create Markdown test coverage reports.
  - Supports custom report titles and output paths.

### Documentation
- Created example scenarios demonstrating all attributes and test base classes.
- Updated README.md with:
  - Usage instructions for `ltx`.
  - Example of generated Markdown coverage reports.
  - Installation and usage examples for all key components.

