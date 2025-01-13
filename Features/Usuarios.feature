@regression @usuarios
Feature: Usuarios

  Background: 
    Given Host

  @usu01
  Scenario: 01 List users
    When      Get list users
    Then      The status code must be 200

  @usu02
  Scenario: 02 Creat Regular User
    When      Post a regular user
    Then      The status code must be 201
    And       Post registration message appear

  @usu03
  Scenario: 03 Creat Regular user already exists
    When      Post a regular user
    And       Post same user
    Then      The status code must be 400
    And       Post same user message appear

  @usu04
  Scenario: 04 Creat User null
    When      Post a null user
    Then      The status code must be 400
    And       Post null message appear

  @usu05
  Scenario: 05 Creat User empty
    When      Post a empty user
    Then      The status code must be 400
    And       Post empty message appear

  @usu06
  Scenario: 06 Search user by ID
    When      Post a regular user
    And       Search a valid user
    Then      The status code must be 200

  @usu07
  Scenario: 07 Search a invalid user
    When      Search a invalid user
    Then      The status code must be 400
    And       Get not found message appear

  @usu08
  Scenario: 08 Delete an empty user
    When      Delete empty user
    Then      The status code must be 405
    And       Delete not possible message appear

  @usu09
  Scenario: 09 Delete an invalid user
    When      Delete invalid user
    Then      The status code must be 200
    And       Delete not delete message appear

  @usu010
  Scenario: 10 Delete an user
    When      Post a regular user
    And       Delete valid user
    Then      The status code must be 200
    And       Delete successful message appear

  @usu11
  Scenario: 11 Edit an user
  	When     Post a regular user
  	And      Edit regular user
  	Then     The status code must be 200
  	And      Put successful message appear

  @usu12
  Scenario: 12 Edit an user with null
  	When     Post a regular user
  	And      Edit null user
  	Then     The status code must be 400
  	And      Put null message appear

  @usu13
  Scenario: 13 Edit an user with empty
  	When     Post a regular user
  	And      Edit empty user
  	Then     The status code must be 400
  	And      Put empty message appear