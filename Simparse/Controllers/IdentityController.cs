using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simparse.Collections;
using Simparse.Models.Identity;
using Simparse.Identity;
using Simparse.Models;

namespace Simparse.Backend.Controllers
{

    /// <summary>
    /// Simparse User Identity Controller for the Simparse API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {

        private readonly ISimparseUserStore _userStore;
        private readonly IUserCollection _userCollection;

        /// <summary>
        /// Identity Controller requires access to the user store and user data collections
        /// </summary>
        /// <param name="userStore"></param>
        /// <param name="userCollection"></param>
        public IdentityController(ISimparseUserStore userStore, IUserCollection userCollection)
        {
            _userStore = userStore;
            _userCollection = userCollection;
        }

        /// <summary>
        /// Login Request entry point that will respond with user id and refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login(SimparseLoginRequest request)
        {
            try
            {

                var response = await _userStore.LoginUser(request.Email, request.Password);

                ApplicationUser user = await _userCollection.GetUserByIdentityId(response.LocalId);

                if (response.Registered)
                {
                    return Ok(new { Token = response.RefreshToken, IdentityId = user.Id });
                }
                else
                {
                    return BadRequest("Unknown user");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Register request entry point that will respond with user id and refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var response = await _userStore.CreateUser(request.Email, request.Password);

                ApplicationUser user = new ApplicationUser()
                {
                    Email = request.Email,
                    DateCreated = DateTime.UtcNow,
                    IdentityId = response.LocalId
                };

                await _userCollection.CreateUser(user);

                return Ok(new { Token = response.RefreshToken });

            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Validate an existing token and return a new token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("validatetoken")]
        public async Task<IActionResult> ValidateToken(ValidateTokenRequest request)
        {
            try
            {
                var response = await _userStore.ValidateToken(request.Token);

                var user = await _userCollection.GetUserByIdentityId(response.user_id);

                if (string.IsNullOrWhiteSpace(response.refresh_token))
                {
                    return BadRequest();
                }

                return Ok(new { Token = response.refresh_token, User = user });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost, Route("PostUserData")]
        public async Task<IActionResult> PostUserData(ApplicationUser user)
        {
            await _userCollection.UpdateUser(user);
            return Ok();
        }

        //reset password
        //    https://cloud.google.com/identity-platform/docs/reference/rest/v1/accounts/resetPassword

    }
}