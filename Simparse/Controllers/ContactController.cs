using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simparse.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IContactCollection _collection;

        public ContactController(IContactCollection collection)
        {
            _collection = collection;
        }


        [HttpPost("PostContact")]
        public async Task<IActionResult> PostContact(ContactViewModel vm)
        {

            ContactDataItem data = new ContactDataItem()
            {
                ContactInfo = vm.ContactInfo,
                Content = vm.TextBody,
                Subject = vm.Subject
            };

            await _collection.SaveContact(data);

            return Ok();
        }


        public class ContactViewModel
        {
            public string Subject { get; set; }

            public string TextBody { get; set; }

            public string ContactInfo { get; set; }
        }

    }
}
