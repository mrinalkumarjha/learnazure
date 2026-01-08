using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using testApi.Models;

namespace testApi.Controllers
{
    /// <summary>
    /// Service bus is enterprise message broaker.  here we get two option either we can create Service bus queue or we can create Service Bus Topic also here. topic will be subscribed
    /// by many subscriber. when in case of topic when a publisher send message all the module which is subscribed to topic will get message.
    /// in service bus queue we manually receive message but in topic you subscribe to it. once you subscribe you will automatically get message.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceBusController : ControllerBase
    {
        private string queueName = "servicebustestqueue";
        private string storageWriteConnectionString = "Endpoint=sb://servicebusnamespacemrinal.servicebus.windows.net/;SharedAccessKeyName=SendPolicy;SharedAccessKey=AnOG7bOTR+vWpuJR83xE/2ZrA7NH7j7h7+ASbHyVcls=;EntityPath=servicebustestqueue";

        public ServiceBusController()
        {
            
        }

        [HttpPost("queue/send")]
        public async Task<IActionResult> sendmessagetoqueue(string message)
        {

            try
            {
                await SendMessage(message);
                return Ok("message sent to service bus");
            }
            catch
            {

                throw;
            }


        }

        [NonAction]
        private async Task SendMessage(string message)
        {
            ServiceBusClient serviceBusClient = new ServiceBusClient(storageWriteConnectionString);
            ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);

            //using ServiceBusMessageBatch serviceBusMessageBatch = await serviceBusSender.CreateMessageBatchAsync();
            //{
                //foreach (var item in collection)
                //{

                //}
                ServiceBusMessage serviceBusMessage = new ServiceBusMessage(message);
                serviceBusMessage.ContentType = "application/text";
            // serviceBusMessageBatch.TryAddMessage(serviceBusMessage);

            //}

            await serviceBusSender.SendMessageAsync(serviceBusMessage);
            await serviceBusSender.DisposeAsync();
            await serviceBusClient.DisposeAsync();
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

        [HttpGet("queue/peekmessage")]
        public async Task<IActionResult> read(int noOfMessage)
        {

            try
            {
                var peekMessage = await PeekMessage(noOfMessage);
                //string encoded = peekMessage.Body.ToString();
               // string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                return Ok(peekMessage);
            }
            catch
            {

                throw;
            }


        }

        private async Task<string> PeekMessage(int noOfMessage)
        {
            ServiceBusClient serviceBusClient = new ServiceBusClient(storageWriteConnectionString);
            ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName);
            StringBuilder sb = new StringBuilder();
            var messages = await serviceBusReceiver.PeekMessagesAsync(maxMessages: noOfMessage);
            foreach (var item in messages)
            {
                sb.Append(item.Body);
            }
            return sb.ToString();
        }

        [HttpGet("queue/receivemessage")]
        public async Task<IActionResult> receive(int noOfMessage)
        {

            try
            {
                var Message = await ReceiveMessage(noOfMessage);
               // string encoded = peekMessage.Body.ToString();
               // string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                return Ok(Message);
            }
            catch
            {

                throw;
            }


        }

        private async Task<string> ReceiveMessage(int noOfMessage)
        {
            ServiceBusClient serviceBusClient = new ServiceBusClient(storageWriteConnectionString);
            ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions { ReceiveMode=ServiceBusReceiveMode.ReceiveAndDelete});
            StringBuilder sb = new StringBuilder();
            var messages = await serviceBusReceiver.ReceiveMessagesAsync(maxMessages: noOfMessage);
            foreach (var item in messages)
            {
                sb.Append(item.Body);
            }
            return sb.ToString();
        }
    }
}
