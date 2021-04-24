#if !NET45

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Abstractions;
using System;
using System.Threading.Tasks;
using RestClient.Net.DependencyInjection;
using System.Net.Http;

namespace RestClient.Net.UnitTests
{

    [TestClass]
    public class MicrosoftDependencyInjectionTests2
    {

        [TestMethod]
        public void TestDIMapping()
        {
            HttpClient? expectedHttpClient = null;

            var serviceCollection = new ServiceCollection()
                .AddSingleton<ISomeService, SomeService>()
                .AddRestClient((o) => { });

            _ = serviceCollection.AddHttpClient("RestClient", (c) => expectedHttpClient = c);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<ISomeService>();

            if (someService.Client is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual("RestClient", client.Name);
            Assert.IsTrue(ReferenceEquals(expectedHttpClient, client.lazyHttpClient.Value));
        }


    }

    public interface ISomeService
    {
        IClient Client { get; }
        Task<string> GetAsync();
    }

    public class SomeService : ISomeService
    {
        public IClient Client { get; }

        public SomeService(IClient client) => Client = client;

        public async Task<string> GetAsync() => await Client.GetAsync<string>();
    }

}
#endif