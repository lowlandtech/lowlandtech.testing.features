@lowlandtech.cli @feature @VCHIP-3372-UC01 @ScenarioResultReporter
Feature: Report scenario and run results from the CLI
  As a developer or agent
  I want a CLI to generate coverage docs and report scenario results to multiple targets
  So that docs, FlowItems, Graph nodes, and Forge tasks stay in sync

  @VCHIP-3372-SC001
  Scenario: Show help
    When I run "ltr --help"
    Then the usage shows "coverage generate" (UAC: VCHIP-3372-UAC001)
    And the usage shows "report scenario" (UAC: VCHIP-3372-UAC002)
    And the usage shows "report run" (UAC: VCHIP-3372-UAC003)

  @VCHIP-3372-SC002
  Scenario: Generate coverage report from assembly
    Given a compiled test assembly exists
    When I run 'ltr coverage generate --assembly "./bin/Debug/net9.0/Tests.dll" --out "./docs/coverage.md" --title "Test Coverage"'
    Then "./docs/coverage.md" is created (UAC: VCHIP-3372-UAC010)
    And the file contains "Feature Coverage Report" or the custom title (UAC: VCHIP-3372-UAC011)

  @VCHIP-3372-SC003
  Scenario: Report single scenario to disk
    Given a normalized result payload for "VCHIP-0001-SC01" with status "passed"
    When I run 'ltr report scenario --id VCHIP-0001-SC01 --status passed --to disk --out ./docs'
    Then "./docs/vchip-0001/VCHIP-0001-SC01.md" is created (UAC: VCHIP-3372-UAC020)
    And it contains "status: passed" (UAC: VCHIP-3372-UAC021)

  @VCHIP-3372-SC004
  Scenario: Report single scenario to multiple targets
    Given a normalized result payload for "VCHIP-0001-SC01"
    When I run 'ltr report scenario --id VCHIP-0001-SC01 --status failed --to disk,flow,graph,forge --out ./docs'
    Then the disk file exists (UAC: VCHIP-3372-UAC030)
    And Flow sink upserts a ScenarioResult item linked to the Scenario (UAC: VCHIP-3372-UAC031)
    And Graph sink upserts a ScenarioResult node and edges (UAC: VCHIP-3372-UAC032)
    And Forge sink creates/updates a Task with status failed (UAC: VCHIP-3372-UAC033)

  @VCHIP-3372-SC005
  Scenario: Idempotent upsert
    Given an existing ScenarioResult for "VCHIP-0001-SC01@{commit}:{runId}"
    When I re-run the same report command
    Then sinks perform an upsert without duplicates (UAC: VCHIP-3372-UAC040)

  @VCHIP-3372-SC006
  Scenario: Report a full test run from TRX
    Given a TRX file at "./TestResults/results.trx"
    When I run 'ltr report run --from ./TestResults/results.trx --to disk,graph --out ./docs'
    Then envelopes are created for each discovered scenario (UAC: VCHIP-3372-UAC050)
    And disk receives one file per scenario id (UAC: VCHIP-3372-UAC051)
    And graph receives one node per scenario result (UAC: VCHIP-3372-UAC052)
