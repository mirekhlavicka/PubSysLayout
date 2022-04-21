using FluentFTP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace PubSysLayout.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CodeController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> GetCode(string path)
        {
            FtpClient client = new FtpClient("10.255.26.66", 2121, "", "");
            client.DataConnectionType = FtpDataConnectionType.PASV;
            await client.ConnectAsync();

            client.Download(out byte[] bytes, path.Replace("~", "/m.navigovat.cz"));

            client.Disconnect();

            var encoding = KlerksSoft.TextFileEncodingDetector.DetectTextByteArrayEncoding(bytes);

            string contents = null;

            if (/*bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF*/encoding != null)
            {
                contents = /*Encoding.UTF8*/encoding.GetString(bytes);
            }
            else
            {
                contents = Encoding.GetEncoding(1250).GetString(bytes);
            }
            return contents;
        }


        public class UploadCode
        { 
            public string Path { get; set; }
            public string Code { get; set; }
        }

        [HttpPut]
        public async Task<IActionResult> PutCode(UploadCode uploadCode)
        {
            //if (id != layoutDefinition.IdLayoutdefinition)
            //{
            //    return BadRequest();
            //}

            FtpClient client = new FtpClient("10.255.26.66", 2121, "", "");
            client.DataConnectionType = FtpDataConnectionType.PASV;
            await client.ConnectAsync();

            client.Upload(/*Encoding.UTF8.GetBytes(uploadCode.Code)*/uploadCode.Code.ToBytes(Encoding.UTF8), uploadCode.Path.Replace("~", "/m.navigovat.cz"));

            client.Disconnect();

            return NoContent();
        }
    }

    public static class StreamExtensions
    {
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            using (var stream = new MemoryStream())
            using (var sw = new StreamWriter(stream, encoding))
            {
                sw.Write(value);
                sw.Flush();
                return stream.ToArray();
            }
        }
    }
}
