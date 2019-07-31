using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using PaymentAPI.Entities;
using PaymentAPI.Requests;
using PaymentAPI.Validators;
using RestSharp;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        protected ISessionFactory sessionFactory;
        public PaymentController()
        {
            sessionFactory = NHibernateHelper.CreateSessionFactory();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Payment> Get(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var paymentRepository = new Repository<Payment>(session);
                return paymentRepository.GetById(id);
            }
        }

        // POST api/values
        [HttpPost()]
        public ActionResult Post([FromBody] PaymentRequest paymentRequest)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var paymentRepository = new Repository<Payment>(session);
                    var paymentValidator = new paymentValidation();
                    var errors = new List<string>();
                    string productPath = ($"http://localhost:5002/api/product/{paymentRequest.ProductId}/detail");
                    string currencyPath = ($"https://free.currconv.com/api/v7/convert?q={paymentRequest.Currency}_TRY&compact=ultra&apiKey=a6da3cf59017c67ac2a2");

                    var result = paymentValidator.Validate(paymentRequest);
                    if (result.IsValid)
                    {
                        try
                        {
                            var currency = ConsumeAPI(currencyPath);
                            var product = ConsumeAPI(productPath);
                            if (currency == null && product == null)
                            {
                                return BadRequest("Currency veya Product null olamaz!!");
                            }
                            var data = (JObject)JsonConvert.DeserializeObject(currency);
                            var dövizKuru = data["USD_TRY"].Value<decimal>();// 1 USD = ?TRY 

                            var payment = new Payment
                            {
                                Name = paymentRequest.Name,
                                Balance = paymentRequest.Balance,
                                Currency = paymentRequest.Currency
                            };
                            var responseProduct = new Product();
                            responseProduct = JsonConvert.DeserializeObject<Product>(product);

                            if (payment.Balance - responseProduct.Price > 0) //yetersiz bakiye kontrol
                            {
                                payment.Balance -= (responseProduct.Price * dövizKuru);
                                payment.TransactionDate = DateTime.Now;
                                payment.ExchangeRatio = dövizKuru;
                                payment.ProductPrice = (responseProduct.Price * dövizKuru);
                            }
                            else
                            {
                                return BadRequest("Yetersiz bakiye");
                            }
                            paymentRepository.Save(payment);
                            transaction.Commit();

                            return Ok(new
                            {
                                ProductName = responseProduct.Name,
                                ProductPrice = payment.ProductPrice,
                                NewBalance = payment.Balance
                            });
                        }
                        catch (System.Exception ex)
                        {
                            transaction.Rollback();
                            System.Console.WriteLine(ex.Message);
                            BadRequest(ex.Message);
                        }
                    }
                    foreach (var i in result.Errors)
                    {
                        errors.Add($"ERROR! {i.PropertyName} : {i.ErrorMessage}");
                    }
                    return BadRequest(errors);
                }
            }
        }
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var paymentRepository = new Repository<Payment>(session);
                    var target = paymentRepository.GetById(id);
                    paymentRepository.Delete(target);

                    transaction.Commit();
                }
            }
        }

        public string ConsumeAPI(string path)
        {
            var client = new RestClient(path);
            var request = new RestRequest(Method.GET);
            //request.AddParameter("Content-Type", "application/json", ParameterType.HttpHeader);

            var response = client.Execute(request);
            System.Console.WriteLine(response.Headers);

            return response.Content;
        }
    }
}
