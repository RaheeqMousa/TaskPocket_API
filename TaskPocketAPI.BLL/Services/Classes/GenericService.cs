using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure;
using TaskPocket.BLL.Services.Interfaces;
using TaskPocket.DAL.Models;
using TaskPocket.DAL.Data;
using TaskPocket.DAL.Repository.Interfaces;
using TaskPocket.DAL.Repository.Classes;
using Mapster;

namespace TaskPocket.BLL.Services.Classes
{
    public class GenericService<TRequest, TResponse, TEntity> : IGenericService<TRequest, TResponse, TEntity>
        where TEntity : BaseModel
    {
        private readonly IGenericRepository<TEntity> genericRepository;

        public GenericService(IGenericRepository<TEntity> grepo)
        {
            genericRepository = grepo;
        }

        public int Create(TRequest request)
        {
            var entity = request.Adapt<TEntity>();
            var e = genericRepository.Add(entity);
            return e.Id;
        }

        public int Delete(int id)
        {
            var entity = genericRepository.GetById(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }
            return genericRepository.Remove(entity);
        }

        public IEnumerable<TResponse> GetAll(bool onlyActive = false)
        {
            var entity = genericRepository.GetAll();
            return entity.Adapt<IEnumerable<TResponse>>();
        }

        public TResponse? GetById(int id)
        {
            return genericRepository.GetById(id) is null ? default : genericRepository.GetById(id).Adapt<TResponse>();
        }

        public bool ToggleStatus(int id)
        {
            var entity = genericRepository.GetById(id);
            if (entity is null)
            {
                return false; // Entity not found
            }
            entity.Status = (entity.Status == Status.Active) ? Status.Inactive : Status.Active;
            genericRepository.Update(entity);
            return true;

        }

        public int Update(int id, TRequest request)
        {
            var entity = genericRepository.GetById(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }


            return genericRepository.Update(request.Adapt<TEntity>());
        }
    }
}
