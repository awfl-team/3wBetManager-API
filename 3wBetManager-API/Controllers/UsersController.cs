﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                return Ok(await getUserDao().FindAllUser());
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            try
            {
                return Ok(await getUserDao().FindUser(id));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }


        [Route("top50")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                return Ok(await getUserDao().FindBestBetters());
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("token")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserFromToken()
        {
            try
            {
                var token = TokenManager.GetTokenFromRequest(Request);
                var user = TokenManager.ValidateToken(token);
                return Ok(await getUserDao().FindUserByEmailToList(user["email"]));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /*[Route("order/{order:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllByOrder(int order)
        {
            try
            {
                if(order != 1 && order != -1)
                {
                    order = 1;
                }
                return Ok(await getUserDao().FindAllUserByOrder(order));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }*/
        
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> Put(string id, [FromBody] User user)
        {
            try
            {
                var canUpdate  = getUserDao().CanUpdate(id, user, out var errorMessage);
                if (canUpdate == false)
                {
                    return Content(HttpStatusCode.BadRequest, errorMessage);
                }
                getUserDao().UpdateUser(id, user);
                var fullUser = await Singleton.Instance.UserDao.FindUser(id);
                return Ok(TokenManager.GenerateToken(fullUser.Email, fullUser.Role, fullUser.Username));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{id}/visibility")]
        [HttpPut]
        public IHttpActionResult PutIsPrivate(string id, [FromBody] bool isPrivate)
        {
            try
            {
                Singleton.Instance.UserDao.UpdateUserIsPrivate(id, isPrivate);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                getUserDao().DeleteUser(id);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
        
        [Route("{id}/reset")]
        [HttpPut]
        public IHttpActionResult Put(string id)
        {
            try
            {
                getUserDao().ResetUser(id);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        private IUserDao getUserDao()
        {
            return Singleton.Instance.UserDao;
        }
    }
}