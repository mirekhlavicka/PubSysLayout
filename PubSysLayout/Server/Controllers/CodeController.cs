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
        private readonly IConfiguration _configuration;

        public CodeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetCode(string ftp, string path)
        {
            string[] tmp = ftp.Split('/');
            string[] tmp1 = _configuration.GetSection("FTP").GetValue<string>(tmp[0]).Split(',');
            path = path.Replace("~", $"/{tmp[1]}");

            FtpClient client = new FtpClient(tmp1[0], Int32.Parse(tmp1[1]), tmp1[2], tmp1[3]);
            client.DataConnectionType = FtpDataConnectionType.PASV;
            await client.ConnectAsync();

            client.Download(out byte[] bytes, path);

            client.Disconnect();

            Encoding encoding = Encoding.UTF8;
            string contents = null;

            if (bytes != null && bytes.Length > 0)
            {
                encoding = KlerksSoft.TextFileEncodingDetector.DetectTextByteArrayEncoding(bytes);

                if (encoding != null)
                {
                    contents = encoding.GetString(bytes);
                }
                else
                {
                    contents = Encoding.GetEncoding(1250).GetString(bytes);
                }
            }

            return contents;
        }

        public class UploadCode
        { 
            public string Path { get; set; }
            public string Code { get; set; }
        }

        [HttpPut]
        public async Task<IActionResult> PutCode(string ftp, UploadCode uploadCode)
        {
            string[] tmp = ftp.Split('/');
            string[] tmp1 = _configuration.GetSection("FTP").GetValue<string>(tmp[0]).Split(',');
            string path = uploadCode.Path.Replace("~", $"/{tmp[1]}");


            FtpClient client = new FtpClient(tmp1[0], Int32.Parse(tmp1[1]), tmp1[2], tmp1[3]);
            client.DataConnectionType = FtpDataConnectionType.PASV;
            await client.ConnectAsync();

            client.Upload(uploadCode.Code.ToBytes(Encoding.UTF8), path, existsMode: FtpRemoteExists.Overwrite, createRemoteDir: true);

            client.Disconnect();

            return NoContent();
        }

        [HttpGet("ftp")]
        public async Task<ActionResult<IEnumerable<string>>> FindFTP(string db)
        {
            var res = new List<string>();

            var ftplist = _configuration.GetSection("FTP").Get<Dictionary<string, string>>();

            foreach (var ftpl in ftplist.Keys)
            {
                var tmp = ftplist[ftpl].Split(',');

                FtpClient client = new FtpClient(tmp[0], Int32.Parse(tmp[1]), tmp[2], tmp[3]);
                client.DataConnectionType = FtpDataConnectionType.PASV;
                await client.ConnectAsync();

                var l = client.GetListing("/"/*, FtpListOption.*/);

                foreach (var d in l.Where(li => li.Type == FtpFileSystemObjectType.Directory))
                {
                    if (client.FileExists(d.FullName + "/web.config"))
                    {
                        client.Download(out byte[] bytes, d.FullName + "/web.config");

                        if (bytes != null)
                        {
                            var cfg = Encoding.UTF8.GetString(bytes);

                            if (cfg.Contains(db, StringComparison.InvariantCultureIgnoreCase))
                            {
                                res.Add(ftpl + "/" + d.Name);
                            }
                        }
                    }
                }

                client.Disconnect();
            }

            return res;
        }

        [HttpGet("list")]
        public async Task<ActionResult<object[]>> GetListing(string ftp, string path)
        {
            string[] tmp = ftp.Split('/');
            string[] tmp1 = _configuration.GetSection("FTP").GetValue<string>(tmp[0]).Split(',');
            path = path.Replace("~", $"/{tmp[1]}");

            FtpClient client = new FtpClient(tmp1[0], Int32.Parse(tmp1[1]), tmp1[2], tmp1[3]);
            client.DataConnectionType = FtpDataConnectionType.PASV;
            await client.ConnectAsync();

            FtpListItem[] l = client.GetListing(path);

            client.Disconnect();

            return l.Select(i => new 
            {
                Type = (int)i.Type,
                i.Name,
                i.Size,
                i.Modified,
                Extension = i.Type == 0 ? Path.GetExtension(i.Name) : ""
            }).ToArray();
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
