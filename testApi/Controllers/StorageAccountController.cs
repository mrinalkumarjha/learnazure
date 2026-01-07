using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using testApi.Models;

namespace testApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageAccountController : ControllerBase
    {
        private string queueName = "testqueue";
        private string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=mrinalstorage;AccountKey=Gfb1firOy89BhmtLQBEjCkG4OQy23yWWAq3evxKe2zXTbFM8al4jgpPpzS1Qc5VSmWJI4B6TOhUi+AStqN+Njw==;EndpointSuffix=core.windows.net";
        public StorageAccountController()
        {
                
        }

        [HttpPost("queue/send")]
        public async Task<IActionResult> sendmessagetoqueue(string message)
        {

            try
            {
                await SendMessage(message);
                return Ok("message sent");
            }
            catch
            {

                throw;
            }


        }

        [HttpPost("queue/sendobject")]
        public async Task<IActionResult> sendmessagetoqueue(List<Course> courses)
        {

            try
            {
                 await SendMessage(JsonSerializer.Serialize(courses));

               
                return Ok("message sent");
            }
            catch
            {

                throw;
            }


        }
        /// <summary>
        /// this pick but does not delete
        /// </summary>
        /// <returns></returns>
        [HttpGet("queue/peekmessage")]
        public async Task<IActionResult> read()
        {

            try
            {
                var peekMessage = await PeekMessage();
                string encoded = peekMessage.Body.ToString();
                return Ok( new { messageid = peekMessage.MessageId,  body = encoded });
            }
            catch
            {

                throw;
            }


        }

        /// <summary>
        /// this reveive and make message invisible for amount of time default is 30 sec
        /// </summary>
        /// <returns></returns>
        [HttpGet("queue/receivemessage")]
        public async Task<IActionResult> receive()
        {

            try
            {
                var peekMessage = await ReceiveMessage();
                string encoded = peekMessage.Body.ToString();
                return Ok(new { messageid = peekMessage.MessageId, body = encoded });
            }
            catch
            {

                throw;
            }


        }

        [NonAction]
        public async Task SendMessage(string message)
        {
            QueueClient queueClient = new QueueClient(storageConnectionString, queueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
            await queueClient.SendMessageAsync(message);
        }

        [NonAction]
        public async Task<PeekedMessage> PeekMessage()
        {
            QueueClient queueClient = new QueueClient(storageConnectionString, queueName);
            PeekedMessage peekMessage = await queueClient.PeekMessageAsync();
            return peekMessage;
            
        }

        [NonAction]
        public async Task<QueueMessage> ReceiveMessage()
        {
            QueueClient queueClient = new QueueClient(storageConnectionString, queueName);
            QueueMessage msg = await queueClient.ReceiveMessageAsync();
            await queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
            return msg;

        }


    }
}
