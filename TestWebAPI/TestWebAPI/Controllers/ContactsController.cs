using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using TestWebAPI.Models;

namespace TestWebAPI.Controllers
{
    public class ContactsController : ApiController
    {
        private ContactContext db = new ContactContext();
        private string superUserToken = "5F8D275C-356F-48F0-A4B0-F546BC192010";

        public IHttpActionResult GetContacts()
        {
            if (!Request.Headers.Contains("token"))
                return Unauthorized();
            if (Request.Headers.GetValues("token").FirstOrDefault() != superUserToken)
                return Unauthorized();

            return Ok(db.Contacts);
        }

        public IHttpActionResult GetContact(int id)
        {
            if (!Request.Headers.Contains("token"))
                return Unauthorized();

            var contact = db.Contacts.Find(id);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        public IHttpActionResult PutContact(int id, Contact contact)
        {
            if (!Request.Headers.Contains("token"))
                return Unauthorized();

            if (id != contact.Id) return BadRequest();
            db.Entry(contact).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        public IHttpActionResult AddContact(Contact contact)
        {
            try
            {
                if (!Request.Headers.Contains("token"))
                    return Unauthorized();

                if (contact == null) return BadRequest();
                contact.AccessToken = Request.Headers.GetValues("token").FirstOrDefault();
                db.Contacts.Add(contact);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IHttpActionResult DeleteContact(int id)
        {
            if (!Request.Headers.Contains("token"))
                return Unauthorized();
            if (Request.Headers.GetValues("token").FirstOrDefault() != superUserToken)
                return Unauthorized();

            var contact = db.Contacts.Find(id);

            if (contact == null)
                return NotFound();

            db.Contacts.Remove(contact);
            db.SaveChanges();

            return Ok(contact);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}