@regression @Produtos
Feature: Produtos

Background:
	Given Host

 @pro01
 Scenario: 01 List Product
	When     Get list products
	Then     The status code must be 200

@pro02
Scenario: 02 Creat a product
    Given    An user admin
    And      login account
    When     Creat a product
    Then     The status code must be 201
    And      Post product successful message

@pro03
Scenario: 03 Create a product that already exists
    Given    An user admin
    And      login account
    When     Creat exists product
    Then     The status code must be 400
    And      Post product exists message

@pro04
Scenario: 04 Create a product with no authorization
    When     Creat exists product
    Then     The status code must be 401
    And      Post product authorization message

@pro05
Scenario: 05 Create a product with regular user
    Given    An user regular
    And      login account
    When     Creat exists product
    Then     The status code must be 403
    And      Post product administrador message

@pro06
Scenario: 06  Search for a product by ID
    Given    An user admin
    And      login account
    When     Creat a product
    Then     Search a product by ID
    And      The status code must be 200

@pro07
Scenario: 07 Search for a product without ID
    When    Search a product without ID
    And     The status code must be 400
    And     Get product message not found

@pro08
Scenario: 08  Delete product
    Given    An user admin
    And      login account
    When     Creat a product
    Then     Delete a product
    And      The status code must be 200
    And      Delete product successful message

@pro09
Scenario: 09  Delete Product without ID
    Given    An user admin
    And      login account
    Then     Delete no product
    And      The status code must be 200
    And      Delete product empty message

@pro10
Scenario: 10  Delete product in a shop cart
    Given    An user admin
    And      login account
    When     Creat a product
    And      Add product in a shop cart
    Then     Delete a product
    And      The status code must be 400
    And      Delete product not permitted message

@pro11
Scenario: 11  Delete product with no authorization
    Given    An user admin
    And      login account
    When     Creat a product
    And      Delete no authorization product
    Then     The status code must be 401
    And      Delete product authorization message

@pro12
Scenario: 12  Delete product with regular user
    Given    An user admin
    And      login account
    When     Creat a product
    And      Admin user edit to regular
    When     login account
    And      Delete a product
    Then     The status code must be 403
    And      Delete product no admin message

  @pro13
  Scenario: 13  Edit product
    Given    An user admin
    And      login account
    When     Creat a product
    And      Edit a product
    Then     The status code must be 200
    And      Put product successful message

  @pro14
  Scenario: 14  Edit product with no authorization
    Given    An user admin
    And      login account
    When     Creat a product
    And      Edit no authorization product
    Then     The status code must be 401
    And      Put product authorization message

  @pro15
  Scenario: 15  Edit product with regular user
    Given    An user admin
    And      login account
    When     Creat a product
    And      Admin user edit to regular
    When     login account
    And      Edit a product
    Then     The status code must be 403
    And      Put product no admin message

  @pro16
  Scenario: 16  Edit empty product
    Given    An user admin
    And      login account
    When     Creat a product
    And      Edit empty product
    Then     The status code must be 400
    And      Put product empty message

  @pro17
  Scenario: 17  Edit no ID product
    Given    An user admin
    And      login account
    When     Creat a product
    And      Edit no ID product
    Then     The status code must be 405
    And      Put product no ID message