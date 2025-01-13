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
    public class carrinho {

        private RestResponse _response;

        private readonly Faker _faker = new Faker();
        private string name = new Faker().Name.FullName();
        private string email = new Faker().Internet.Email();
        private string password = new Faker().Internet.Password();
        private string userID;
        private string _token;
        private string prodId;
        private string cartId;

        [When(@"Get list car shop")]
        public void get_list_car_shop() {
            var request = new RestRequest("/carrinhos", Method.Get);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [Given(@"(.+) user")]
        public void user(String user) {
            bool type = false;

            if (user == "admin") {
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
                userID = JObject.Parse(_response.Content)["_id"].ToString();
            } catch (Exception ex) { }

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Sing in account")]
        public void sing_account() {

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

        public void postProduct(object payload) {
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

        [When(@"Register (.+) product")]
        public void register_product(String caract) {
            String prodName = null;
            Random random = new Random();

            if (caract == "a") {
                prodName = "Aut test API C# " + random.Next(10001).ToString();
            } else if (caract == "exists") {
                prodName = "An existing product";
            }

            var payload = new {
                nome = prodName,
                preco = 30,
                descricao = "Test",
                quantidade = 5
            };

            postProduct(payload);

            if (caract == "exists" && _response.Content.Contains("Cadastro realizado com sucesso")) {
                postProduct(payload);
            }

            Console.WriteLine("User creation response: " + _response.Content);
        }

        public void postCart(object payload) {
            var request = new RestRequest("/carrinhos", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_token}");
            request.AddJsonBody(payload);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            try {
                cartId = JObject.Parse(_response.Content)["_id"].ToString();
            } catch (Exception ex) { }

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Register (.+) cart")]
        public void register_cart(String action) {
            int quant = 1;

            if (action == "more product") {
                quant = 30;
            } else if (action == "no authorization") {
                _token = "";
            }

            if(prodId == null) {
                prodId = "teste";
            }

            var payload = new {
                produtos = action.Equals("duplicate") ?
                new[]{
                    new { idProduto = prodId, quantidade = quant },
                    new { idProduto = prodId, quantidade = quant }
                } :
                new[]
                {
                    new { idProduto = prodId, quantidade = quant }
                }
            };

            postCart(payload);

            string responseBody = _response.Content;

            if (action.Equals("twice") && responseBody.Contains("Cadastro realizado com sucesso")) {
                postCart(payload);
            }
        }

        [StepDefinition(@"Post cart (.+) message")]
        public void put_product_message(string message) {

            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (message) {
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Cadastro realizado com sucesso"));
                    break;

                case "duplicate":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Não é permitido possuir produto duplicado"));
                    break;

                case "twice":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Não é permitido ter mais de 1 carrinho"));
                    break;

                case "no product":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Produto não encontrado"));
                    break;

                case "more product":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Produto não possui quantidade suficiente"));
                    break;

                case "no authorization":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Token de acesso ausente, inválido, expirado ou usuário do token não existe mais"));
                    break;

                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

        [StepDefinition(@"Search cart (.+) ID")]
        public void search_cart_id(String generec) {
            if (generec == "invalid") {
                cartId = "invalid";
            }

            var request = new RestRequest("/carrinhos/{id}", Method.Get);
            request.AddUrlSegment("id", cartId);

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Get no id message")]
        public void get_no_id_message() {
            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Carrinho não encontrado"));
        }

        [StepDefinition(@"Complete purchase")]
        public void complete_purchase() {
            var request = new RestRequest("/carrinhos/concluir-compra", Method.Delete);
            request.AddHeader("Authorization", $"Bearer {_token}");

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Purchase (.+) message")]
        public void purchase_message(String purchase) {
            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (purchase) {
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Registro excluído com sucesso"));
                    break;

                case "not found":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Não foi encontrado carrinho para esse usuário"));
                    break;

                case "twice":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Token de acesso ausente, inválido, expirado ou usuário do token não existe mais"));
                    break;                                  

                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }

        [StepDefinition(@"Cancel purchase")]
        public void cancel_purchase() {
            var request = new RestRequest("/carrinhos/cancelar-compra", Method.Delete);
            request.AddHeader("Authorization", $"Bearer {_token}");

            var client = common_steps.GetClient();

            _response = client.Execute(request);

            common_steps.SetResponse(_response);

            Console.WriteLine("User creation response: " + _response.Content);
        }

        [StepDefinition(@"Cancel purchase (.+) message")]
        public void cancel_purchase_message(String purchase) {
            var responseBody = JObject.Parse(common_steps.GetResponse().Content);

            switch (purchase) {
                case "successful":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Registro excluído com sucesso. Estoque dos produtos reabastecido"));
                    break;

                case "not found":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Não foi encontrado carrinho para esse usuário"));
                    break;

                case "twice":
                    Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Token de acesso ausente, inválido, expirado ou usuário do token não existe mais"));
                    break;

                default:
                    Console.WriteLine("Feature not recognized. No navigation performed.");
                    break;
            }
        }
    }
}