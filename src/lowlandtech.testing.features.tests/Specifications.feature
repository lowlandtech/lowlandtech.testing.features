Feature: Simple scenario title

  @VCHIP-4001-SC001
  @VCHIP-4001-UAC001
  Scenario: Outcome happened
    Given Given a simple context
    When When an action occurs
    Then Then an outcome is expected


Feature: NodeId scenario

  @vy.test.nodeid
  Scenario: NodeId included
    Given NodeId scenario
    When When testing NodeId
    Then Then NodeId should be included


Feature: TaskId scenario

  @VCHIP-4001-TK001
  Scenario: TaskId included
    Given TaskId scenario
    When When testing TaskId
    Then Then TaskId should be included


Feature: UseCaseId scenario

  @VCHIP-4001-UC001
  Scenario: UseCaseId included
    Given UseCaseId scenario
    When When testing UseCaseId
    Then Then UseCaseId should be included
