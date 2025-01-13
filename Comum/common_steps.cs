using System;
using RestSharp;
using TechTalk.SpecFlow;

namespace Comum {

    [Binding]
    public class common_steps {

        private static RestClient _client;
        private static RestResponse _response;

        public static RestResponse GetResponse() => _response;

        public static void SetResponse(RestResponse response) {
            _response = response;
        }

        [Given(@"Host")]
        public void Host() {
            _client = new RestClient("https://serverest.dev");
        }

        public static RestClient GetClient() => _client;

        [StepDefinition(@"The status code must be (.*)")]
        public void TheStatusCodeMustBe(int statusCode) {
            if (_response == null) {
                throw new InvalidOperationException("A resposta não foi inicializada.");
            }

            if ((int)_response.StatusCode != statusCode) {
                throw new Exception($"O código de status esperado era {statusCode}, mas foi obtido {(int)_response.StatusCode}.");
            }
        }
    }

}
