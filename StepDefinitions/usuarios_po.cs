using RestSharp;
using Bogus;
using Comum;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Gherkin;
using AventStack.ExtentReports.Gherkin.Model;

namespace StepDefinitions {

    [Binding]
    public class usuarios_po {

        private RestResponse _response;

        private readonly Faker _faker = new Faker();
        private string name = new Faker().Name.FullName();
        private string email = new Faker().Internet.Email();
        private string password = new Faker().Internet.Password();
        private string _id;
        private string _token;
        private string userId;


        [When(@"Get list users")]
        public void get_list_users() {
            var request = new RestRequest("/usuarios", Method.Get);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            //Console.WriteLine("User creation response: " + _response.Content);
        }

        [When(@"Post a (.+) user")]
        public void post_a_user(String user) {
            object type = false;

            switch (user) {
                case "admin":
                    type = false;
                    break;

                case "null":
                    name = null;
                    email = null;
                    password = null;
                    type = null;
                    break;

                case "empty":
                    name = "";
                    email = "";
                    password = "";
                    type = "";
                    break;

                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }

            var payload = new {
                nome = name,
                email = email,
                password = password,
                administrador = type?.ToString()?.ToLower() ?? "null"
            };

            var request = new RestRequest("/usuarios", Method.Post);
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            try {
                userId = JObject.Parse(_response.Content)["_id"].ToString();
            } catch (Exception ex) { }

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Post same user")]
        public void post_same_user() {
            post_a_user("regular");
        }

        [StepDefinition(@"Search a (.+) user")]
        public void seatch_a_user(String generic) {
            
            if(generic == "invalid") {
                userId = "test";
            }

            var request = new RestRequest("/usuarios/{id}", Method.Get);
            request.AddUrlSegment("id", userId);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Post (.+) message appear")]
        public void post_message_appear(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "registration":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Cadastro realizado com sucesso"));
                    break;
                case "same user":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Este email já está sendo usado"));
                    break;
                case "null":
                    Assert.That(responseBody["nome"]?.ToString(), Is.EqualTo("nome deve ser uma string"));
                    Assert.That(responseBody["email"]?.ToString(), Is.EqualTo("email deve ser uma string"));
                    Assert.That(responseBody["password"]?.ToString(), Is.EqualTo("password deve ser uma string"));
                    Assert.That(responseBody["administrador"]?.ToString(), Is.EqualTo("administrador deve ser 'true' ou 'false'"));
                    break;
                case "empty":
                    Assert.That(responseBody["nome"]?.ToString(), Is.EqualTo("nome não pode ficar em branco"));
                    Assert.That(responseBody["email"]?.ToString(), Is.EqualTo("email não pode ficar em branco"));
                    Assert.That(responseBody["password"]?.ToString(), Is.EqualTo("password não pode ficar em branco"));
                    Assert.That(responseBody["administrador"]?.ToString(), Is.EqualTo("administrador deve ser 'true' ou 'false'"));
                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

        [StepDefinition(@"Get (.+) message appear")]
        public void get_message_appear(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "not found":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Usuário não encontrado"));
                    break;                
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

        [When(@"Delete (.+) user")]
        public void delete_user(String typeDel) {
            string endpoint = "/usuarios/{id}";

            if (typeDel == "empty") {
                endpoint = "/usuarios/";

            } else if (typeDel == "invalid") {
                userId = "test";

            }

            var request = new RestRequest(endpoint, Method.Delete);

            if (typeDel != "empty") {
                request.AddUrlSegment("id", userId);
            }
                        
            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Delete (.+) message appear")]
        public void delete_message_appear(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "not found":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Não é possível realizar DELETE em /usuarios/. Acesse https://serverest.dev para ver as rotas disponíveis e como utilizá-las."));
                    break;
                case "not delete":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Nenhum registro excluído"));
                    break;
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Registro excluído com sucesso"));
                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

        [StepDefinition(@"Edit (.+) user")]
        public void edit_user(String action) {
            object type = false;

            switch (action) {
                case "regular":
                    name = name.Substring(0, name.Length - 1) + " edit\"";

                    break;
                case "null":
                    name = null;
                    email = null;
                    password = null;
                    type = null;

                    break;
                case "empty":
                    name = "";
                    email = "";
                    password = "";
                    type = "";

                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }

            var payload = new {
                nome = name,
                email = email,
                password = password,
                administrador = type?.ToString()?.ToLower() ?? "null"
            };

            var request = new RestRequest("/usuarios/{id}", Method.Put);
            request.AddUrlSegment("id", userId);
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Put (.+) message appear")]
        public void put_message_appear(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Registro alterado com sucesso"));
                    break;
                case "null":
                    Assert.That(responseBody["nome"]?.ToString(), Is.EqualTo("nome deve ser uma string"));
                    Assert.That(responseBody["email"]?.ToString(), Is.EqualTo("email deve ser uma string"));
                    Assert.That(responseBody["password"]?.ToString(), Is.EqualTo("password deve ser uma string"));
                    Assert.That(responseBody["administrador"]?.ToString(), Is.EqualTo("administrador deve ser 'true' ou 'false'"));
                    break;
                case "empty":
                    Assert.That(responseBody["nome"]?.ToString(), Is.EqualTo("nome não pode ficar em branco"));
                    Assert.That(responseBody["email"]?.ToString(), Is.EqualTo("email não pode ficar em branco"));
                    Assert.That(responseBody["password"]?.ToString(), Is.EqualTo("password não pode ficar em branco"));
                    Assert.That(responseBody["administrador"]?.ToString(), Is.EqualTo("administrador deve ser 'true' ou 'false'"));
                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

    }
}