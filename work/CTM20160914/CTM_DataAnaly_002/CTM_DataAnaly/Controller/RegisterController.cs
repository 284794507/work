using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CTM_DataAnaly.Controller
{
    public class User_Info
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
    }
    [RoutePrefix("api/register")]
    public class RegisterController : ApiController
    {
        public static List<User_Info> Model = new List<User_Info>()
        {
            new User_Info {Id=1,Name="aal",Info="ewfw" },
            new User_Info {Id=2,Name="sdf",Info="esfsdfwfw" }
        };

        [HttpGet]
        [Route("get1")]
        public IEnumerable<User_Info> Get()
        {
            Console.WriteLine("get1");
            return Model;
        }

        [HttpPost]
        [Route("post1")]
        public HttpResponseMessage Post345(User_Info value)
        {
            Model.Add(new User_Info
            {
                Id = value.Id,
                Info = value.Info,
                Name = value.Name,
            });
            Console.WriteLine(value.Info);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("sfsdfsdsdf", Encoding.UTF8, "text/plain")
            };
        }

        [HttpPost]
        [Route("post2")]
        public HttpResponseMessage Post213(User_Info value)
        {
            Model.Add(new User_Info
            {
                Id = value.Id,
                Info = value.Info,
                Name = value.Name,
            });
            Console.WriteLine(value.Info);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("sfffffdsf", Encoding.UTF8, "text/plain")
            };
        }
    }
}
