using APITestingAutomationSuite.Model;
using APITestingAutomationSuite.Utilities;
using JsonPath;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace APITestingAutomationSuite.Steps
{
    [Binding]
    public sealed class GoRestSteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private static int newUserId;

        private readonly Settings _settings;
        public GoRestSteps(Settings settings)
        {
            _settings = settings;
        }

        
        [When(@"I perform a GET operation for getting the details of the userid (.*)")]
        public void WhenIPerformAGETOperationForGettingTheDetailsOfTheUserid(int userId)
        {
            _settings.Request = new RestRequest("/public-api/users/{userId}", Method.GET);
            _settings.Request.AddUrlSegment("userId", userId);
            _settings.Response = _settings.RestClient.Execute(_settings.Request);
        }


        [Then(@"I should see that the operation is succesfull")]
        public void ThenIShouldSeeThatTheOperationIsSuccesfull()
        {
            Console.WriteLine($"{_settings.Response.Content}");
            Assert.That(_settings.Response.IsSuccessful, Is.True, "GET operation is not successful");
        }

        [Then(@"I should see the followings matching")]
        public void ThenIShouldSeeTheFollowingsMatching(Table table)
        {
            dynamic data = table.CreateDynamicInstance();

            var result = _settings.Response.DeserializeResponse()["result"];         
            var first_name = new Node(result)["first_name"].AsString;   // This is JsonPath
            var last_name = (string)new Node(result)["last_name"];

            Assert.That(data.first_name.ToString(), Is.EqualTo(first_name), "first_name does not match");
            Assert.That(data.last_name.ToString(), Is.EqualTo(last_name), "last_name does not match");
        }



        [When(@"I perform a POST operation to create a User profile with the body")]
        public void WhenIPerformAPOSTOperationToCreateAUserProfileWithTheBody(Table table)
        {
            dynamic data = table.CreateDynamicInstance();            
            
            _settings.Request = new RestRequest("/public-api/users", Method.POST);
            _settings.Request.RequestFormat = DataFormat.Json;
            _settings.Request.AddBody(new { first_name = data.first_name.ToString(), 
                                            last_name  = data.last_name.ToString(),
                                            gender = data.gender.ToString(),
                                            email = data.email.ToString(),
                                            status = data.status.ToString()
                                          });

            // Different ways of executing ..
            //_settings.Response =_settings.RestClient.Execute(_settings.Request);  // This is simpler way
            //_settings.Response = _settings.RestClient.ExecuteAsyncRequest<User>(_settings.Request).GetAwaiter().GetResult(); // This is more advanced wit the method created in Libraries
            _settings.Response = _settings.RestClient.ExecutePostAsync(_settings.Request).GetAwaiter().GetResult(); // This is with built-in method
            Console.WriteLine($"Response is : { _settings.Response}");
            Console.WriteLine($"ResponseUri is : { _settings.Response.ResponseUri}");


            int lastIndex = _settings.Response.ResponseUri.ToString().LastIndexOf("/users/")+6;
            string actualUserId = _settings.Response.ResponseUri.ToString().Substring(lastIndex + 1);
            Console.WriteLine($"The data is saved to Id {actualUserId}");

            newUserId = actualUserId.Length > 0 ? int.Parse(actualUserId) : 0;
            Console.WriteLine($"The data is saved to Id {newUserId}");


        }


        [When(@"I perform a GET operation for getting all users")]
        public void WhenIPerformAGETOperationForGettingAllUsers()
        {
            _settings.Request = new RestRequest("/public-api/users", Method.GET);
            _settings.Response = _settings.RestClient.Execute(_settings.Request);
            Console.WriteLine($"All the users\n {_settings.Response.Content}");

        }

        [When(@"I perform a DEL operation for deleting the userid (.*)")]
        public void WhenIPerformADELOperationForDeletingTheUserid(int userId)
        {
            _settings.Request = new RestRequest("/public-api/users/{userId}", Method.DELETE);
            _settings.Request.AddUrlSegment("userId", userId);
            _settings.Response=_settings.RestClient.Execute(_settings.Request);
        }


        [When(@"I perform a POST operation to create a User profile with the file ""(.*)""")]
        public void WhenIPerformAPOSTOperationToCreateAUserProfileWithTheFile(string filename)
        {
            _settings.Request = new RestRequest("/public-api/users", Method.POST);
            _settings.Request.RequestFormat = DataFormat.Json;

            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string pathToFile = currentDirectory.Replace(@"\bin\Debug", "") + @"TestData\" + filename;

            User userData = JsonConvert.DeserializeObject<User>(File.ReadAllText(pathToFile));
            _settings.Request.AddBody(userData);

            _settings.Response = _settings.RestClient.ExecutePostTaskAsync(_settings.Request).GetAwaiter().GetResult();

            Console.WriteLine($"Response is : { _settings.Response}");
            Console.WriteLine($"ResponseUri is : { _settings.Response.ResponseUri}");

            int lastIndex = _settings.Response.ResponseUri.ToString().LastIndexOf("/");
            string actualUserId = _settings.Response.ResponseUri.ToString().Substring(lastIndex + 1);
            newUserId = int.Parse(actualUserId);
            
            Console.WriteLine($"The data is saved to Id {newUserId}");

        }


        [When(@"I perform a PUT operation to update the user's profile with the followings")]
        public void WhenIPerformAPUTOperationToUpdateTheUserSProfileWithTheFollowings(Table table)
        {
            dynamic data = table.CreateDynamicInstance();
            
            _settings.Request = new RestRequest("/public-api/users/"+ newUserId, Method.PUT);
            _settings.Request.RequestFormat = DataFormat.Json;

            User user = new User()
            {
                first_name = data.first_name.ToString(),
                last_name = data.last_name.ToString(),
                gender = data.gender.ToString(),
                dob = data.dob.ToString(),
                email = data.email.ToString(),
                phone = data.phone.ToString()
            };

            _settings.Request.AddBody(user);

            _settings.Response = _settings.RestClient.Execute(_settings.Request);

        }


        [When(@"I perform a DEL operation for deleting the user")]
        public void WhenIPerformADELOperationForDeletingTheUser()
        {
            WhenIPerformADELOperationForDeletingTheUserid(newUserId);
        }

        [When(@"I perform a GET operation for getting the details of the user")]
        public void WhenIPerformAGETOperationForGettingTheDetailsOfTheUser()
        {
            WhenIPerformAGETOperationForGettingTheDetailsOfTheUserid(newUserId);
        }


        [Then(@"The dob property should be equal to ""(.*)""")]
        public void ThenTheDobPropertyShouldBeEqualTo(string expectedValue)
        {
            var result = _settings.Response.DeserializeResponse()["result"];
            var dob = new Node(result)["dob"].AsString.Trim();   // This is JsonPath
            Assert.That(dob, Is.EqualTo(expectedValue.Trim()), "dob is not matching");
        }



    } // end of class
} // end of namespace
