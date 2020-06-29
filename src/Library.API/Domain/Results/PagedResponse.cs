using Library.API.Domain.Parameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Domain.Results
{
    public class PagedResponse<T> : Response<IEnumerable<T>> where T : class
    {
        [JsonProperty("pagination", Order = 2)]
        public Pagination Pagination { get; }

        public PagedResponse()
        {
            this.Pagination = new Pagination();
        }

        public PagedResponse(IEnumerable<T> data):this()
        {
            this.Data = data;
        }

        public PagedResponse(IEnumerable<T> data, BasePaginationParams parameters):this()
        {
            ToPagedResponse(data, parameters);           
        }
      
        public PagedResponse(IQueryable<T> data, BasePaginationParams parameters) : this()
        {
            ToPagedResponse(data, parameters);
        }            

        private void ToPagedResponse(IEnumerable<T> data, BasePaginationParams pagination)
        {
            FillPagination(pagination, data.Count());
        }

        private void ToPagedResponse(IQueryable<T> data, BasePaginationParams pagination)
        {
            FillPagination(pagination, data.Count());

            this.Data = data.Skip((pagination.page - 1) * pagination.size).Take(pagination.size).ToList();         
        }

        public PagedResponse<T> ToPagedResponse<TIn>(IEnumerable<TIn> data, BasePaginationParams pagination, Func<object, IEnumerable<T>> convert) where TIn : class
        {
            if (convert == null)
                throw new ArgumentException("Conert function cannot be null");

            FillPagination(pagination, data.Count());

            IEnumerable<TIn> pagedResponse = data.Skip((pagination.page - 1) * pagination.size).Take(pagination.size);                    
            this.Data = convert(pagedResponse.ToList());           
          
            return this;
        }

        public PagedResponse<T> ToPagedResponse<TIn>(IQueryable<TIn> data, BasePaginationParams pagination, Func<object, IEnumerable<T>> convert) where TIn : class
        {
            if (convert == null)
                throw new ArgumentException("Convert function cannot be null");

            FillPagination(pagination, data.Count());

            IEnumerable<TIn> pagedResponse = data.Skip((pagination.page - 1) * pagination.size).Take(pagination.size);                        
            this.Data = convert(pagedResponse.ToList());

            return this;
        }

        private void FillPagination(BasePaginationParams parameters, int totalCount)
        {
            this.Pagination.CurrentPage = parameters.page;
            this.Pagination.PageSize = parameters.size;
            this.Pagination.TotalCount = totalCount;
            this.Pagination.TotalPages = (int)Math.Ceiling(totalCount / (double) parameters.size);

            if (this.Pagination.CurrentPage > this.Pagination.TotalPages)
            {
                throw new ArgumentException("The current page cannot be greater than total pages size");
            }
        }
    }
}
