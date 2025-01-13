@regression @carrinho
Feature: carrinho

 Background: 
    Given Host

 @car01
 Scenario: 01 List cart shop
    When     Get list car shop
    Then     The status code must be 200

 @car02
 Scenario: 02 Register cart
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register a cart
    Then     The status code must be 201
    And      Post cart successful message

 @car03
 Scenario: 03 Register duplicate product in the cart
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register duplicate cart
    Then     The status code must be 400
    And      Post cart duplicate message

 @car04
 Scenario: 04 Register more than one cart
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register twice cart
    Then     The status code must be 400
    And      Post cart twice message

 @car05
 Scenario: 05 Register no product on the cart
    Given    admin user
    And      Sing in account
    And      Register a cart
    Then     The status code must be 400
    And      Post cart no product message

 @car06
 Scenario: 06 Register cart with zero quantity
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register more product cart
    Then     The status code must be 400
    And      Post cart more product message

 @car07
 Scenario: 07 Register cart with no authorization
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register no authorization cart
    Then     The status code must be 401
    And      Post cart no authorization message

 @car08
 Scenario: 08 Search cart by ID
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register a cart
    Then     Search cart by ID
    And      The status code must be 200

  @car09
  Scenario: 09 Search cart without ID
    When     Search cart invalid ID
    Then     The status code must be 400
    And      Get no id message

  @car10
  Scenario: 10 Complete purchase
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register a cart
    And      Complete purchase
    Then     The status code must be 200
    And      Purchase successful message

  @car11
  Scenario: 11 Complete purchase with no cart
    Given    admin user
    And      Sing in account
    When     Complete purchase
    Then     The status code must be 200
    And      Purchase not found message

  @car12
  Scenario: 12 Complete purchase with no authorization
    When     Complete purchase
    Then     The status code must be 401
    And      Purchase authorization message

  @car13
  Scenario: 13 Cancel purchase
    Given    admin user
    And      Sing in account
    When     Register a product
    And      Register a cart
    And      Cancel purchase
    Then     The status code must be 200
    And      Cancel purchase successful message

  @car14
  Scenario: 14 Cancel purchase with no cart
    Given    admin user
    And      Sing in account
    When     Cancel purchase
    Then     The status code must be 200
    And      Cancel purchase not found message

  @car15
  Scenario: 15 Cancel purchase with no authorization
    When     Cancel purchase
    Then     The status code must be 401
    And      Cancel purchase authorization message