using RestSharp;
using Bogus;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Comum;

namespace StepDefinitions {

    [Binding]
    public class login_po {

        private RestResponse _response;
        private readonly Faker _faker = new Faker();
        private string name = new Faker().Name.FullName();
        private string email = new Faker().Internet.Email();
        private string password = new Faker().Internet.Password();

        [Given(@"User (.+)")]
        public void User(string userType) {

            bool isAdmin = userType.Equals("admin", StringComparison.OrdinalIgnoreCase);
            var body = new {
                nome = name,
                email = email,
                password = password,
                administrador = isAdmin.ToString().ToLower()
            };

            var client = common_steps.GetClient();

            var request = new RestRequest("/usuarios", Method.Post)
                .AddJsonBody(body);

            var response = client.Execute(request);
            common_steps.SetResponse(response);

            Console.WriteLine("User creation response: " + response.Content);
        }

        [When(@"Submit login as (.+)")]
        public void SubmitLogin(string userType) {
            string logEmail = "";
            string logPassword = "";

            if (userType.Equals("admin") || userType.Equals("regular")) {
                logEmail = email;
                logPassword = password;

            } else if (userType.Equals("no registration")) {
                logEmail = email;
                logPassword = password;

            } else if (userType.Equals("null value")) {
                logEmail = null;
                logPassword = null;
            }

            var body = new {
                email = logEmail,
                password = logPassword
            };

            var client = common_steps.GetClient();
            if (client == null) {
                throw new InvalidOperationException("RestClient is not initialized.");
            }

            var request = new RestRequest("/login", Method.Post)
                .AddJsonBody(body);

            var response = client.Execute(request);
            common_steps.SetResponse(response);

            Console.WriteLine("Login response: " + response.Content);
        }

        [Then(@"Message (.+) appear")]
        public void MessageLoginAppear(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "login":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Login realizado com sucesso"));
                    break;
                case "invalid":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Email e/ou senha inválidos"));
                    break;
                case "of null":
                    Assert.That(responseBody["email"]?.ToString(), Is.EqualTo("email deve ser uma string"));
                    Assert.That(responseBody["password"]?.ToString(), Is.EqualTo("password deve ser uma string"));
                    break;
                case "empty":
                    Assert.That(responseBody["email"]?.ToString(), Is.EqualTo("email não pode ficar em branco"));
                    Assert.That(responseBody["password"]?.ToString(), Is.EqualTo("password não pode ficar em branco"));
                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

    }
}
