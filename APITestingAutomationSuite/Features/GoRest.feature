Feature: GoRest
	Some basic crud operations

@post
Scenario: 01_Verify POST operation with a file for User
	When I perform a POST operation to create a User profile with the file "userData.json"
	Then I should see that the operation is succesfull

@put 
Scenario: 02_Verify PUT operation for the same user
	When I perform a PUT operation to update the user's profile with the followings
	| first_name | last_name | gender | dob        | email                   | phone        |
	| Richard    | White     | male   | 1990-01-01 | richard.white@email.com | 444.444.4444 |
	Then I should see that the operation is succesfull
	And I should see the followings matching
	| first_name | last_name |
	| Richard    | White    |

@get
Scenario: 03_Return the details of the userid.
    When I perform a GET operation for getting the details of the user
	Then I should see that the operation is succesfull
	Then The dob property should be equal to "1990-01-01"

@delete
Scenario: 04_Verify DEL operation for the user
	When I perform a DEL operation for deleting the user
	Then I should see that the operation is succesfull

@getall
Scenario: 05_Return the details of all users
	When I perform a GET operation for getting all users
	Then I should see that the operation is succesfull

@get
Scenario:06_Return the details of the userid and verify the first_name and last_name
	When I perform a GET operation for getting the details of the userid 2056
	Then I should see that the operation is succesfull
	And I should see the followings matching
	| first_name | last_name |
	| Smith    | Brokere    |

#@post
#Scenario: Verify POST operation with a table for User 
#	When I perform a POST operation to create a User profile with the body
#		| first_name | last_name | gender | email                 | status |
#		| James2      | Brown     | male   | Brown12.brown@email.com | active |
#	Then I should see that the operation is succesfull

#@delete
#Scenario: Verify DEL operation for the given userid
#	When I perform a DEL operation for deleting the userid 2055
#	Then I should see that the operation is succesfull
