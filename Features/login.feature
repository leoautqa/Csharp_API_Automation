@regression @login
Feature: Login

Background:
	Given Host

@log01
Scenario: 01 Login with admin user
    Given     User admin
    When      Submit login as admin
    Then      The status code must be 200
    And       Message login appear

@log02
Scenario: 02 Login with regular user
    Given     User regular
    When      Submit login as regular
    Then      The status code must be 200
    And       Message login appear

@log03
Scenario: 03 Login with a no registration user
    When      Submit login as no registration
    Then      The status code must be 401
    And       Message invalid appear

@log04
Scenario: 04 Login with out E-mail and password
    When      Submit login as null value
    Then      The status code must be 400
    And       Message of null appear

@log05
Scenario: 05 Login with out E-mail and password
    When      Submit login as empty
    Then      The status code must be 400
    And       Message empty appear