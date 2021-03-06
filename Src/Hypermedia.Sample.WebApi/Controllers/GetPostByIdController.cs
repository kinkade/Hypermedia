﻿using System.Linq;
using System.Web.Http;
using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi.Resources;

namespace Hypermedia.Sample.WebApi.Controllers
{
    public class GetPostByIdController : ApiController
    {
        readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">The databsae instance.</param>
        public GetPostByIdController(IDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns the posts with the given id.
        /// </summary>
        /// <param name="id">the ID of the post to return.</param>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpGet, Route("v1/posts/{id}")]
        public IHttpActionResult Execute(int id)
        {
            var post = _database.Posts.GetById(new[] { id }).FirstOrDefault().AsResource();

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post.Populate(_database));
        }
    }
}
