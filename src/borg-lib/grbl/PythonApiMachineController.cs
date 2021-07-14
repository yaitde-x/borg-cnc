using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utilities;

namespace Borg.Machine
{
    public class RS274EngineStatus
    {
        public RS274Job LastJob { get; set; }
        public RS274Job CurrentJob { get; set; }
    }

    public class RS274Job
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public int Eof { get; set; }
        public int Status { get; set; }
        public string Result { get; set; }
        public int ErrorCount { get; set; }
        public string Block { get; set; }
    }

    public class PythonApiMachineController : IRS274Controller
    {
        private const string url = "http://localhost:6040";
        private readonly IHttpClientFactory _clientFactory;
        private readonly GrblResponseParser _parser;

        public PythonApiMachineController(IHttpClientFactory clientFactory, GrblResponseParser parser)
        {
            _clientFactory = clientFactory;
            _parser = parser;
        }

        private static string GetMachineStateUrl()
        {
            return $"{url}/api/machine/state";
        }

        private static string GetMachineSettingsUrl()
        {
            return $"{url}/api/machine/settings";
        }

        private static string GetRunUrl()
        {
            return $"{url}/api/machine/runblock";
        }

        private static string GetFileUrl()
        {
            return $"{url}/api/machine/runfile";
        }

        private static string GetJobUrl(string jobId)
        {
            return $"{url}/api/machine/job/{jobId}";
        }

        private static string GetEngineStatusUrl()
        {
            return $"{url}/api/machine/engine";
        }

        public async Task<RS274State> GetState()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetMachineStateUrl());
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var stateBuf = await response.Content.ReadAsStringAsync();
                return _parser.ParseStatus(stateBuf);
            }
            else
            {
                throw new MachineOperationException(response.StatusCode.ToString());
            }
        }

        public Task<RS274EngineStatus> GetEngineStatus()
        {
            return Get<RS274EngineStatus>(GetEngineStatusUrl(), (buf) =>
            {
                return buf.FromJson<RS274EngineStatus>();
            });
        }

        public Task<RS274Job> GetJob(string jobId)
        {
            return Get<RS274Job>(GetJobUrl(jobId), (buf) =>
            {
                return buf.FromJson<RS274Job>();
            });
        }

        public Task<GrblSettings> GetSettings()
        {
            return Get<GrblSettings>(GetMachineSettingsUrl(), (buf) =>
            {
                return new GrblSettings(GrblSettingsParser.Parse(buf));
            });
        }

        private async Task<T> Get<T>(string url, Func<string, T> processor)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var buf = await response.Content.ReadAsStringAsync();
                return processor(buf);
            }
            else
            {
                throw new MachineOperationException(response.StatusCode.ToString());
            }
        }
        public class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }

        public Task<RS274Job> Run(RS274Instruction instruction)
        {
            throw new NotImplementedException();
        }
        public async Task<RS274Job> Run(string gCodeBlock)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetRunUrl());
            request.Content = new JsonContent(new { block = gCodeBlock });
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var engineStatus = await response.Content.ReadAsStringAsync();
                return engineStatus.FromJson<RS274Job>();
            }
            else
            {
                throw new MachineOperationException(response.StatusCode.ToString());
            }
        }

        public async Task<RS274State> File(string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetFileUrl());
            request.Content = new JsonContent(new { fileName = fileName });
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var stateBuf = await response.Content.ReadAsStringAsync();
                return _parser.ParseStatus(stateBuf);
            }
            else
            {
                throw new MachineOperationException(response.StatusCode.ToString());
            }
        }
    }
}