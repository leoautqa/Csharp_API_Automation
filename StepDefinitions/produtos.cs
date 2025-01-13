using RestSharp;
using Bogus;
using Comum;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Gherkin;
using AventStack.ExtentReports.Gherkin.Model;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Net;

namespace StepDefinitions {

    [Binding]
    public class produtos {

        private RestResponse _response;

        private readonly Faker _faker = new Faker();
        private string name = new Faker().Name.FullName();
        private string email = new Faker().Internet.Email();
        private string password = new Faker().Internet.Password();
        private string _id;
        private string _token;
        private string prodId;

        [When(@"Get list products")]
        public void get_list_products() {
            var request = new RestRequest("/produtos", Method.Get);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            //Console.WriteLine("User creation response: " + _response.Content);
        }

        [Given(@"An user (.+)")]
        public void an_user(string userType) {
            bool type = false;
            if (userType == "admin") {
                type = true;
            }

            var payload = new {
                nome = name,
                email = email,
                password = password,
                administrador = type.ToString().ToLower()
            };

            var request = new RestRequest("/usuarios", Method.Post);
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            try {
                _id = JObject.Parse(_response.Content)["_id"].ToString();
            } catch (Exception ex) { }

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Admin user edit to regular")]
        public void admin_user_edit_to_regular() {
            var payload = new {
                nome = name,
                email = email,
                password = password,
                administrador = false.ToString().ToLower()
            };

            var request = new RestRequest("/usuarios/{id}", Method.Put);
            request.AddUrlSegment("id", _id);
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);
            _id = _response.Content;

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"login account")]
        public void login_account() {

            var payload = new {
                email = email,
                password = password
            };

            var request = new RestRequest("/login", Method.Post);
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            string authorization = JObject.Parse(_response.Content)["authorization"].ToString();

            _token = authorization.Replace("Bearer ", "");

            Console.WriteLine("User creation response: " + _response.Content);
        }

        public void PostProduct(object payload) {
            var request = new RestRequest("/produtos", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_token}");
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            try {
                prodId = JObject.Parse(_response.Content)["_id"].ToString();
            } catch (Exception ex) { }

        }

        [When(@"Creat (.+) product")]
        public void creat_a_product(string character) {
            Random random = new Random();

            string prodName = null;

            if (character == "a") {
                prodName = "Aut test API C# " + random.Next(10001).ToString();
            } else if (character == "exists") {
                prodName = "An existing product";
            }

            var payload = new {
                nome = prodName,
                preco = 30,
                descricao = "Test",
                quantidade = 5
            };

            PostProduct(payload);

            if (character == "exists" && _response.Content.Contains("Cadastro realizado com sucesso")) {
                PostProduct(payload);
            }

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [Then(@"Post product (.+) message")]
        public void MessageLoginAppear(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Cadastro realizado com sucesso"));
                    break;
                case "exists":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Já existe produto com esse nome"));
                    break;
                case "authorization":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Token de acesso ausente, inválido, expirado ou usuário do token não existe mais"));
                    break;
                case "administrador":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Rota exclusiva para administradores"));
                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

        [StepDefinition(@"Search a product (.+) ID")]
        public void search_a_product_id(String generic) {

            if (generic == "without") {
                prodId = "test";
            }

            var request = new RestRequest("/produtos/{id}", Method.Get);
            request.AddUrlSegment("id", prodId);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Get product message not found")]
        public void get_product_message_not_found() {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Produto não encontrado"));

        }

        [StepDefinition(@"Delete (.*) product")]
        public void delete_product(String generic) {
            if (generic == "no") {
                prodId = "test";
            } else if (generic == "no authorization") {
                _token = "";
            }

            var request = new RestRequest("/produtos/{id}", Method.Delete);
            request.AddHeader("Authorization", $"Bearer {_token}");
            request.AddUrlSegment("id", prodId);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [Then(@"Delete product (.+) message")]
        public void delete_product_message(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Registro excluído com sucesso"));
                    break;
                case "empty":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Nenhum registro excluído"));
                    break;
                case "not permitted":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Não é permitido excluir produto que faz parte de carrinho"));
                    break;
                case "authorization":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Token de acesso ausente, inválido, expirado ou usuário do token não existe mais"));
                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

        [StepDefinition(@"Add product in a shop cart")]
        public void add_product_in_a_shop_cart() {
            var payload = new {
                produtos = new[] {
                    new { 
                        idProduto = prodId,
                        quantidade = 5
                    }
                }
            };

            var request = new RestRequest("/carrinhos", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_token}");
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        public void EditAdminUserToRegular() {
            string payload = $"{{" +
                $"\"nome\": {name}," +
                $"\"email\": {email}," +
                $"\"password\": {password}," +
                $"\"administrador\": \"false\"" +
                $"}}";

            var request = new RestRequest("/usuarios/{_id}", Method.Put);
            request.AddUrlSegment("id", prodId);
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);
        }

        [StepDefinition(@"Edit (.+) product")]
        public void edit_product(string action) {
            string endpoint = "/produtos/{_id}";

            Random random = new Random();

            string randomNumber = " " + random.Next(0, 101);

            string prodName = "Aut test API C# " + randomNumber + " edited";
            object price = 30;
            string desc = "Test Edit";
            object quant = 30;

            switch (action) {
                case "no authorization":
                    _token = "";

                    break;
                case "empty":
                    prodName = "";
                    price = "";
                    desc = "";
                    quant = "";

                    break;
                case "no admin":
                    Console.WriteLine("Feature not recognized. No navigation performed.");

                    break;
                case "no ID":
                    prodId = "";

                    break;
            }

            var payload = new {
                nome = prodName,
                preco = price,
                descricao = desc,
                quantidade = quant
            };

            if (action == "no ID") {
                endpoint = "/produtos/";
            }

            var request = new RestRequest(endpoint, Method.Put);
            request.AddHeader("Authorization", $"Bearer {_token}");
            if (action != "no ID") {
                request.AddUrlSegment("_id", prodId);
            }
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [Then(@"Put product (.+) message")]
        public void put_product_message(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Registro alterado com sucesso"));
                    break;
                case "authorization":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Token de acesso ausente, inválido, expirado ou usuário do token não existe mais"));
                    break;
                case "no admin":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Rota exclusiva para administradores"));
                    break;
                case "empty":
                    Assert.That(responseBody["nome"]?.ToString(), Is.EqualTo("nome não pode ficar em branco"));
                    Assert.That(responseBody["preco"]?.ToString(), Is.EqualTo("preco deve ser um número"));
                    Assert.That(responseBody["descricao"]?.ToString(), Is.EqualTo("descricao não pode ficar em branco"));
                    Assert.That(responseBody["quantidade"]?.ToString(), Is.EqualTo("quantidade deve ser um número"));
                    break;
                case "no ID":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Não é possível realizar PUT em /produtos/. Acesse https://serverest.dev para ver as rotas disponíveis e como utilizá-las."));
                    break;
                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

    }
}
